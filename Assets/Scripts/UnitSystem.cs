using System.Collections.Generic;
using UnityEngine;

public class UnitSystem : MonoBehaviour
{
    public static UnitSystem Instance { get; private set; }

    [SerializeField] private int waveInterval = 5;
    [SerializeField] private int enemyCount = 1;

    private static HashSet<SpecialAction.SpecialAction> specialActions = new HashSet<SpecialAction.SpecialAction>();

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
        
        specialActions.Add(new SpecialAction.Sprint());
        foreach (var action in specialActions)
        {
            action.Activate();
        }
    }

    private void HandleTurnChange(TurnState newTurn)
    {
        if (newTurn == TurnState.EnemyTurn)
        {
            StartEnemyTurn();
        }
        else if (newTurn == TurnState.PlayerTurn)
        {
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

        GameStateManager.Instance.EndEnemyTurn();
    }

    private void HandleMouseButtonDown(int button, Vector3 position, GameObject clickedGO)
    {
        if (button == 0)
        {
            if (GameStateManager.Instance.IsPlayerTurn && GameStateManager.Instance.IsIdle)
            {
                Node targetNode = clickedGO.GetComponent<Node>();

                if (GameStateManager.Instance.CurrentState == GameState.Idle)
                {
                    if (Player.Instance.TryMoveTo(targetNode))
                    {
                        GameStateManager.Instance.EndPlayerTurn();
                    }
                }
            }
        }
    }
}
