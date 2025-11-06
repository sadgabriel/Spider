using System.Collections.Generic;
using UnityEngine;

public class FacilityBuildingPanel : Panel<FacilityData>
{
    [SerializeField] private UnityEngine.UI.Image facilityIcon;
    [SerializeField] private TMPro.TextMeshProUGUI facilityNameText;
    [SerializeField] private TMPro.TextMeshProUGUI facilityDescriptionText;

    private int leftCount;
    private List<Facility> builtFacilities = new List<Facility>();

    private FacilityData selectedFacilityData;

    private void OnEnable()
    {
        InputManager.Instance.OnMouseButtonDown += HandleMouseButtonDown;
    }

    private void OnDisable()
    {
        InputManager.Instance.OnMouseButtonDown -= HandleMouseButtonDown;
    }

    public override void Show(FacilityData data)
    {
        builtFacilities.Clear();
        selectedFacilityData = data;
        facilityIcon.sprite = Sprite.Create(data.IconTexture, new Rect(0, 0, data.IconTexture.width, data.IconTexture.height), new Vector2(0.5f, 0.5f));
        facilityNameText.text = data.Name;
        facilityDescriptionText.text = data.BuildEffectDescription;
        leftCount = data.Count;
        gameObject.SetActive(true);
    }

    public void ReturnToFacilitySelection()
    {
        foreach (Facility facility in builtFacilities)
        {
            FacilityManager.Instance.DestroryFacility(facility);
        }
        builtFacilities.Clear();
        GameStateManager.Instance.SetUiState(UiState.FacilitySelection, null);
    }

    private void HandleMouseButtonDown(int button, Vector3 position, GameObject clickedGO)
    {
        if (button != 0 || clickedGO == null || selectedFacilityData == null)
            return;

        if (!clickedGO.CompareTag("Pillar"))
            return;

        Pillar pillar = clickedGO.GetComponent<Pillar>();
        if (pillar == null)
        {
            Debug.LogError($"Clicked object tagged as Pillar but has no Pillar component.");
            return;
        }

        if (pillar.Size != selectedFacilityData.Size)
        {
            UiManager.Instance.ShowNotification("Invaild Pillar Size", $"This facility must be built on {selectedFacilityData.Size.ToString().ToLower()} pillar.");
            return;
        }

        if (pillar.HasFacility)
        {
            UiManager.Instance.ShowNotification("Pillar Occupied", "This pillar already has a facility built on it.");
            return;
        }
        
        Facility builtFacility = FacilityManager.Instance.BuildFacility(pillar, selectedFacilityData.FacilityType);
        if (builtFacility != null)
        {
            leftCount--;
            builtFacilities.Add(builtFacility);
            if (leftCount <= 0)
            {
                FinishFacilityBuilding();
            }
        }
    }

    private void FinishFacilityBuilding()
    {
        builtFacilities.Clear();
        GameStateManager.Instance.ResetUiState();
        UnitSystem.Instance.NotifyFacilityBuilt();
    }
}