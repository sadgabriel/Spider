using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class UnitSystem : MonoBehaviour
{
    public static UnitSystem Instance { get; private set; }

    [SerializeField] private int waveInterval = 5;
    [SerializeField] private int enemyCount = 1;

    private bool isHandlingLevelUp = false;
    private Queue<int> pendingLevelUps = new();

    private void Awake()
    {
        Instance = this;
    }

    public void Initialize()
    {
        UnitManager.Instance.InitializePlayer();
        UnitManager.Instance.SpawnWave(enemyCount);
        GameStateManager.Instance.OnTurnChange += HandleTurnChange;
        InputManager.Instance.OnMouseButtonDown += HandleMouseButtonDown;
        Player.Instance.OnLevelUp += HandleLevelUp;
    }

    public void NotifyFacilityBuilt()
    {
        isHandlingLevelUp = false;
        TryProcessNextLevelUp();
    }

    private void HandleTurnChange(TurnState newTurn)
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
        UnitManager.Instance.RemoveDestroyedEnemies();
        foreach (var enemy in UnitManager.Instance.Enemies)
        {
            if (enemy.gameObject.activeSelf)
            {
                enemy.Act();
            }
        }
        UnitManager.Instance.RemoveDestroyedEnemies();

        if (GameStateManager.Instance.TurnCount % waveInterval == 0)
        {
            UnitManager.Instance.SpawnWave(enemyCount);
        }

        UnitManager.Instance.SpawnEnemyWithSpawner();

        GameStateManager.Instance.EndEnemyTurn();
    }

    private void HandleMouseButtonDown(int button, Vector3 position, GameObject clickedGO)
    {
        if (button == 0)
        {
            if (GameStateManager.Instance.IsPlayerTurn && GameStateManager.Instance.IsIdleGameState && GameStateManager.Instance.IsIdleUiState)
            {
                Node targetNode = clickedGO?.GetComponent<Node>();

                if (targetNode == null)
                {
                    Facility facility = clickedGO?.GetComponentInParent<Facility>();
                    targetNode = facility?.CurrentPillar;
                }

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
