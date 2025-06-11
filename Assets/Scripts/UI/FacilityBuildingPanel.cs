using System.Collections.Generic;
using UnityEditor.Search;
using UnityEngine;

class FacilityBuildingPanel : Panel<FacilityData>
{
    private static FacilityData selectedFacilityData;

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
        gameObject.SetActive(true);
    }

    public void ReturnToFacilitySelection()
    {
        GameStateManager.Instance.SetUIState(UIState.FacilitySelection, null);
    }

    private void HandleMouseButtonDown(int button, Vector3 position, GameObject clickedGO)
    {
        if (button == 0 && clickedGO != null && clickedGO.CompareTag("Pillar") && selectedFacilityData != null)
        {
            Pillar pillar = clickedGO.GetComponent<Pillar>();
            if (pillar.Size == selectedFacilityData.size)
            {
                FacilityManager.Instance.BuildFacility(pillar, selectedFacilityData.facilityType);

                GameStateManager.Instance.ResetUIState();
            }
        }
    }
}