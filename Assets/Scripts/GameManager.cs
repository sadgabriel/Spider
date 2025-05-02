using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    [SerializeField] private int waveInterval = 5;

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        InitializeGame();
    }

    private void InitializeGame()
    {
        MapManager.Instance.GenerateMap();
        UnitManager.Instance.InitializePlayer();
        UnitManager.Instance.SpawnWave(5);
        TurnSystem.Instance.OnTurnChanged += HandleTurnChanged;
    }

    private void HandleTurnChanged(TurnState newTurn)
    {
        if (newTurn == TurnState.EnemyTurn)
        {
            StartEnemyTurn();
        }
    }

    private void StartEnemyTurn()
    {
        foreach (var enemy in UnitManager.Instance.Enemies)
        {
            enemy.Act();
        }

        if (TurnSystem.Instance.TurnCount % waveInterval == 0)
        {
            UnitManager.Instance.SpawnWave(5);
        }

        TurnSystem.Instance.EndEnemyTurn();
    }
}
