using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEditor.Search;
using UnityEngine;

public class FacilityBuildingPanel : Panel<FacilityData>
{
    [SerializeField] private UnityEngine.UI.Image facilityIcon;
    [SerializeField] private TMPro.TextMeshProUGUI facilityNameText;
    [SerializeField] private TMPro.TextMeshProUGUI facilityDescriptionText;

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
        selectedFacilityData = data;
        facilityIcon.sprite = Sprite.Create(data.IconTexture, new Rect(0, 0, data.IconTexture.width, data.IconTexture.height), new Vector2(0.5f, 0.5f));
        facilityNameText.text = data.Name;
        facilityDescriptionText.text = data.Description;
        gameObject.SetActive(true);
    }

    public void ReturnToFacilitySelection()
    {
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

        if (pillar.Size == selectedFacilityData.Size)
        {
            Facility builtFacility = FacilityManager.Instance.BuildFacility(pillar, selectedFacilityData.FacilityType);
            if (builtFacility != null)
            {
                FinishFacilityBuilding();
            }
        }
    }

    private void FinishFacilityBuilding()
    {
        GameStateManager.Instance.ResetUiState();
        UnitSystem.Instance.NotifyFacilityBuilt();
    }
}