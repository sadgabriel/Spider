using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

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

    private void Start()
    {
        InitializeGame();
    }

    private void InitializeGame()
    {
        Map.Instance.GenerateMap();
        UnitSystem.Instance.Initialize();
        SpecialActionManager.Instance.AddSpecialAction(new Sprint()); // To Be Removed
        Facility testFacility = FacilityManager.Instance.InstallFacility(Map.Instance.StartPillar, FacilityType.Test); // To Be Removed
        testFacility.UpgradeLevel = 3; // To Be Removed
    }
}
