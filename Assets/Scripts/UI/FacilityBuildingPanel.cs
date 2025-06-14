using System.Collections.Generic;
using UnityEditor.Search;
using UnityEngine;

public class FacilityBuildingPanel : Panel<FacilityData>
{
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
            Debug.LogWarning($"Clicked object tagged as Pillar but has no Pillar component.");
            return;
        }

        if (pillar.Size == selectedFacilityData.size)
        {
            FacilityManager.Instance.BuildFacility(pillar, selectedFacilityData.facilityType);
            GameStateManager.Instance.ResetUiState();
        }
        else
        {
            Debug.Log($"Pillar size mismatch: required {selectedFacilityData.size}, but found {pillar.Size}.");
        }
    }
}