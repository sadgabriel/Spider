using System.Collections.Generic;
using UnityEditor.Search;
using UnityEngine;

class FacilityBuildingPanel : Panel<FacilityData>
{
    public override void Show(FacilityData data)
    {
        gameObject.SetActive(true);
    }

    public void ReturnToFacilitySelection()
    {
        UIManager.Instance.HideAllPanels();
        UIManager.Instance.ShowPanel<List<FacilityData>>(PanelType.FacilitySelection, null);
    }
}