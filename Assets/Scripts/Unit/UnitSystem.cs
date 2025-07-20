using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class UnitSystem : MonoBehaviour
{
    public static UnitSystem Instance { get; private set; }

    [SerializeField] private int waveInterval = 5;

    public int WaveIndex => GameStateManager.Instance.TurnCount / waveInterval;

    private bool isHandlingLevelUp = false;
    private readonly Queue<int> pendingLevelUps = new();

    private void Awake()
    {
        Instance = this;
    }

    public void Initialize()
    {
        UnitManager.Instance.InitializePlayer();
        UnitManager.Instance.SpawnSpawners(EnemyWaveManager.Instance.GetMaxSpawners(0));
        
        if (EnemyWaveManager.Instance.IsBossWave(0))
        {
            UnitManager.Instance.SpawnBoss();
        }
        
        GameStateManager.Instance.OnTurnChange += HandleTurnChange;
        InputManager.Instance.OnMouseButtonDown += HandleMouseButtonDown;
        Player.Instance.OnLevelUp += HandleLevelUp;
    }

    public void NotifyFacilityBuilt()
    {
        isHandlingLevelUp = false;
        TryProcessNextLevelUp();
    }

    private void HandleTurnChange(TurnState newTurn, int turnCount)
    {
        if (newTurn == TurnState.EnemyTurn)
        {
            StartEnemyTurn();
        }
        else if (newTurn == TurnState.PlayerTurn)
        {
            Player.Instance.RegenerateHp();
            Player.Instance.RegenerateStamina();
        }
    }

    private void StartEnemyTurn()
    {
        if (GameStateManager.Instance.CurrentGameState == GameState.GameCleared ||
            GameStateManager.Instance.CurrentGameState == GameState.GameOver)
        {
            return;
        }

        StartCoroutine(DoEnemyTurn());
    }

    private IEnumerator DoEnemyTurn()
    {
        UnitManager.Instance.RemoveDestroyedEnemies();
        foreach (var enemy in UnitManager.Instance.Enemies)
        {
            if (enemy.gameObject.activeSelf)
            {
                //yield return enemy.DoAct();
                enemy.Act();
            }
        }
        UnitManager.Instance.RemoveDestroyedEnemies();

        SpawnEnemies();

        yield return new WaitForSeconds(0.5f);

        GameStateManager.Instance.EndEnemyTurn();
    }
    
    private void SpawnEnemies()
    {
        if (GameStateManager.Instance.TurnCount % waveInterval == 0)
        {
            if (WaveIndex >= EnemyWaveManager.Instance.MaxWaves)
            {
                Debug.Log("Game Completed! No more waves.");
                GameStateManager.Instance.SetGameState(GameState.GameCleared);
                return;
            }

            int maxSpawners = EnemyWaveManager.Instance.GetMaxSpawners(WaveIndex);
            int currentSpawners = UnitManager.Instance.Enemies.Count(e => e is Spawner);
            UnitManager.Instance.SpawnSpawners(Mathf.Max(0, maxSpawners - currentSpawners));

            if (EnemyWaveManager.Instance.IsBossWave(WaveIndex))
            {
                UnitManager.Instance.SpawnBoss();
            }
        }

        UnitManager.Instance.SpawnEnemyWithSpawner(EnemyWaveManager.Instance.GetEnemyProportions(WaveIndex));
        UnitManager.Instance.SpawnEnemyWithBoss();
    }

    private void HandleMouseButtonDown(int button, Vector3 position, GameObject clickedGO)
    {
        if (button == 0)
        {
            if (GameStateManager.Instance.IsPlayerTurn && GameStateManager.Instance.IsIdleGameState && GameStateManager.Instance.IsIdleUiState)
            {
                Node targetNode = Utils.GetNodeFromGameObject(clickedGO);

                if (targetNode != null && GameStateManager.Instance.CurrentGameState == GameState.Idle)
                {
                    if (Player.Instance.TryMoveTo(targetNode))
                    {
                        GameStateManager.Instance.EndPlayerTurn();
                    }
                }
            }
        }
    }

    private void HandleLevelUp(int newLevel)
    {
        pendingLevelUps.Enqueue(newLevel);
        TryProcessNextLevelUp();
    }

    private void TryProcessNextLevelUp()
    {
        if (isHandlingLevelUp || pendingLevelUps.Count == 0) return;

        isHandlingLevelUp = true;
        int levelToHandle = pendingLevelUps.Dequeue();

        List<FacilityData> facilityDataList = FacilityManager.Instance.GetAllAvailableFacilityData();
        List<FacilityData> candidateFacilities;

        if (facilityDataList.Count > 3)
        {
            candidateFacilities = facilityDataList.OrderBy(_ => UnityEngine.Random.value).Take(3).ToList();
        }
        else
        {
            candidateFacilities = facilityDataList;
        }

        GameStateManager.Instance.SetUiState(UiState.FacilitySelection, candidateFacilities);
    }
}
