using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

class UIManager : MonoBehaviour
{
    public static UIManager Instance { get; private set; }

    [SerializeField] private IdlePanel IdlePanel;
    [SerializeField] private FacilitySelectionPanel facilitySelectionPanel;
    [SerializeField] private FacilityBuildingPanel facilityBuildingPanel;

    private Dictionary<UIState, IPanel> UIStatePanelMap = new();

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

        UIStatePanelMap[UIState.Idle] = IdlePanel;
        UIStatePanelMap[UIState.FacilitySelection] = facilitySelectionPanel;
        UIStatePanelMap[UIState.FacilityBuilding] = facilityBuildingPanel;
    }

    public void Initialize()
    {
        GameStateManager.Instance.OnUIStateChange += HandleUIStateChange;
    }

    public void HideAllPanels()
    {
        foreach (var panel in UIStatePanelMap.Values)
        {
            panel.Hide();
        }
    }
    private void HandleUIStateChange(UIState state, object data)
    {
        HideAllPanels();
        UIStatePanelMap.TryGetValue(state, out IPanel panel);
        if (panel != null)
        {
            panel.Show(data);
        }
        else
        {
            Debug.LogError("There is no matching panel");
        }
    }
}
