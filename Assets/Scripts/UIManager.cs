using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance { get; private set; }

    [SerializeField] private Panel statusPanel;
    [SerializeField] private FacilitySelectionPanel facilitySelectionPanel;

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
    }

    public void Initialize()
    {
        if (statusPanel != null)
        {
            statusPanel.Show();
        }

        Player.Instance.OnLevelUp += HandleLevelUp;
    }

    private void HandleLevelUp(int newLevel)
    {
        GameStateManager.Instance.SetFacilitySelectionUIState();
        ToggleFacilitySelectionPanel(true);
    }

    private void ToggleFacilitySelectionPanel(bool show)
    {
        if (facilitySelectionPanel != null)
        {
            if (show)
            {
                List<FacilityData> facilityDataList = FacilityManager.Instance.GetAllFacilityData();

                List<FacilityData> candidateFacilities = facilityDataList.OrderBy(data => Random.value).Take(3).ToList();
                facilitySelectionPanel.SetLeftChoice(candidateFacilities[0]);
                facilitySelectionPanel.SetMiddleChoice(candidateFacilities[1]);
                facilitySelectionPanel.SetRightChoice(candidateFacilities[2]);

                facilitySelectionPanel.Show();
            }
            else
            {
                facilitySelectionPanel.Hide();
            }
        }
    }
}
