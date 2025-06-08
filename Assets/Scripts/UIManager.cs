using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

class UIManager : MonoBehaviour
{
    public static UIManager Instance { get; private set; }

    [SerializeField] private StatusPanel statusPanel;
    [SerializeField] private FacilitySelectionPanel facilitySelectionPanel;
    [SerializeField] private FacilityBuildingPanel facilityBuildingPanel;

    private Dictionary<PanelType, IPanel> panels = new();

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }

        panels[PanelType.Status] = statusPanel;
        panels[PanelType.FacilitySelection] = facilitySelectionPanel;
        panels[PanelType.FacilityBuilding] = facilityBuildingPanel;
    }

    public void Initialize()
    {
        ShowPanel(PanelType.Status, default(NoData));

        Player.Instance.OnLevelUp += HandleLevelUp;
    }

    public void ShowPanel<T>(PanelType panelType, T data)
    {
        panels.TryGetValue(panelType, out IPanel panel);
        if (panel != null)
        {
            if (panel is Panel<T> typedPanel)
            {
                typedPanel.Show(data);
            }
            else
            {
                Debug.LogWarning($"Panel of type {panelType} does not match the expected type {typeof(T)}.");
            }
        }
        else
        {
            Debug.LogError($"Panel of type {panelType} not found.");
        }
    }

    public void HidePanel(PanelType panelType)
    {
        panels.TryGetValue(panelType, out IPanel panel);
        if (panel != null)
        {
            panel.Hide();
        }
        else
        {
            Debug.LogError($"Panel of type {panelType} not found.");
        }
    }

    public void HideAllPanels()
    {
        foreach (var panel in panels.Values)
        {
            panel.Hide();
        }
    }

    private void HandleLevelUp(int newLevel)
    {
        GameStateManager.Instance.SetFacilitySelectionUIState();
        HideAllPanels();

        List<FacilityData> facilityDataList = FacilityManager.Instance.GetAllFacilityData();

        List<FacilityData> candidateFacilities = facilityDataList.OrderBy(data => UnityEngine.Random.value).Take(3).ToList();
        ShowPanel(PanelType.FacilitySelection, candidateFacilities);
    }
}
