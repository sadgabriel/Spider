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

    private Node delayedTargetNode = null;

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

            if (delayedTargetNode != null)
            {
                if (delayedTargetNode == Player.Instance.CurrentNode)
                {
                    delayedTargetNode = null;
                    return;
                }
                
                MovePlayerTo(delayedTargetNode);
                delayedTargetNode = null;
            }
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

        List<bool> isDoneList = new();

        List<Enemy> enemies = UnitManager.Instance.Enemies;
        for (int i = 0; i < enemies.Count; i++)
        {
            int index = i;
            isDoneList.Add(false);

            IEnumerator wrapped = Utils.DoRunAndNotify(enemies[index].DoAct(), () => isDoneList[index] = true);
            StartCoroutine(wrapped); 
        }
        
        yield return new WaitUntil(() => isDoneList.All(done => done));

        UnitManager.Instance.RemoveDestroyedEnemies();

        yield return DoSpawnEnemies();

        GameStateManager.Instance.EndEnemyTurn();
    }

    private IEnumerator DoSpawnEnemies()
    {
        if (GameStateManager.Instance.TurnCount % waveInterval == 0)
        {
            if (WaveIndex >= EnemyWaveManager.Instance.MaxWaves)
            {
                GameStateManager.Instance.SetGameState(GameState.GameCleared);
                GameStateManager.Instance.SetUiState(UiState.GameCleared, null);
                yield break;
            }

            int maxSpawners = EnemyWaveManager.Instance.GetMaxSpawners(WaveIndex);
            int currentSpawners = UnitManager.Instance.Enemies.Count(e => e is Spawner);
            UnitManager.Instance.SpawnSpawners(Mathf.Max(0, maxSpawners - currentSpawners));

            if (EnemyWaveManager.Instance.IsBossWave(WaveIndex))
            {
                UnitManager.Instance.SpawnBoss();
            }
        }

        bool spawnWithSpawnerIsDone = false;
        bool spawnWithBossIsDone = false;

        IEnumerator wrappedSpawnWithSpawner = Utils.DoRunAndNotify(UnitManager.Instance.DoSpawnEnemyWithSpawner(EnemyWaveManager.Instance.GetEnemyProportions(WaveIndex)), () => spawnWithSpawnerIsDone = true);
        IEnumerator wrappedSpawnWithBoss = Utils.DoRunAndNotify(UnitManager.Instance.DoSpawnEnemyWithBoss(), () => spawnWithBossIsDone = true);

        StartCoroutine(wrappedSpawnWithSpawner);
        StartCoroutine(wrappedSpawnWithBoss);
        
        yield return new WaitUntil(() => spawnWithSpawnerIsDone && spawnWithBossIsDone);
    }

    private void HandleMouseButtonDown(int button, Vector3 position, GameObject clickedGO)
    {
        if (button == 0)
        {
            if (GameStateManager.Instance.IsPlayerTurn && GameStateManager.Instance.IsIdleGameState && GameStateManager.Instance.IsIdleUiState)
            {
                Node targetNode = Utils.GetNodeFromGameObject(clickedGO);

                MovePlayerTo(targetNode);
            }
            else if (GameStateManager.Instance.IsEnemyTurn)
            {
                if (delayedTargetNode == null)
                {
                    delayedTargetNode = Utils.GetNodeFromGameObject(clickedGO);
                }
            }
        }
    }

    private void MovePlayerTo(Node targetNode)
    {
        if (targetNode != null && GameStateManager.Instance.CurrentGameState == GameState.Idle)
        {
            if (Player.Instance.TryMoveTo(targetNode))
            {
                GameStateManager.Instance.EndPlayerTurn();
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
