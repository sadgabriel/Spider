using System;
using System.Collections.Generic;
using System.Linq;
using Michsky.MUIP;
using UnityEngine;
using UnityEngine.UI;

[System.Serializable]
public class PanelEntry
{
    public UiState uiState;
    public MonoBehaviour panel;

    public IPanel GetPanel()
    {
        if (panel is IPanel ipanel)
        {
            return ipanel;
        }
        else
        {
            Debug.LogError($"Assigned panel does not implement IPanel. Check assignment in UiManager.");
            return null;
        }
    }
}

public class UiManager : MonoBehaviour
{
    [SerializeField] private List<PanelEntry> panelEntries;
    [SerializeField] private GameObject notification;

    public static UiManager Instance { get; private set; }

    private Dictionary<UiState, IPanel> uiStatePanelMap = new();

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

        uiStatePanelMap = panelEntries.ToDictionary(entry => entry.uiState, entry => entry.GetPanel());
    }

    public void Initialize()
    {
        SetHandlers();
    }

    public void ShowNotification(string title, string description)
    {
        NotificationManager notificationManager = notification.GetComponent<NotificationManager>();
        notificationManager.title = title;
        notificationManager.description = description;
        notificationManager.UpdateUI();
        notificationManager.OpenNotification();
    }

    private void SetHandlers()
    {
        GameStateManager.Instance.OnUiStateChange += HandleUiStateChange;
    }

    public void HideAllPanels()
    {
        foreach (var panel in uiStatePanelMap.Values)
        {
            panel.Hide();
        }
    }

    private void HandleUiStateChange(UiState currentState, object uiData)
    {
        HideAllPanels();

        if (uiStatePanelMap.TryGetValue(currentState, out IPanel nextPanel))
        {
            nextPanel.Show(uiData);
        }
        else
        {
            Debug.LogError($"No matching panel found for UiState: {currentState}. Please assign it in the inspector.");
        }
    }
}
