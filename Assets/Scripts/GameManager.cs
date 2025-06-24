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
        if (Map.Instance == null) Debug.LogError("Map is missing!");
        if (UnitSystem.Instance == null) Debug.LogError("UnitSystem is missing!");
        if (UiManager.Instance == null) Debug.LogError("UIManager is missing!");

        Map.Instance.Initialize();
        UnitSystem.Instance.Initialize();
        UiManager.Instance.Initialize();
    }
}
