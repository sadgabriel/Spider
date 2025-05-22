using UnityEngine;

public class UnitSystem : MonoBehaviour
{
    public static UnitSystem Instance { get; private set; }

    [SerializeField] private int waveInterval = 5;
    [SerializeField] private int enemyCount = 1;

    private void Awake()
    {
        Instance = this;
    }

    public void Initialize()
    {
        UnitManager.Instance.InitializePlayer();
        UnitManager.Instance.SpawnWave(enemyCount);
        GameStateManager.Instance.OnTurnChanged += HandleTurnChanged;
        InputManager.Instance.OnMouseClickedWhenIdle += HandleMouseClickedWhenIdle;
        InputManager.Instance.OnMouseClickedWhenSpecialAction += HandleMouseClickedWhenSpecialAction;
    }

    private void HandleTurnChanged(TurnState newTurn)
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

    private void HandleMouseClickedWhenIdle(int button, Vector3 position, GameObject clickedGO)
    {
        if (button == 0) 
        {
            if (GameStateManager.Instance.IsPlayerTurn())
            {
                Node targetNode = clickedGO.GetComponent<Node>();
                if (UnitManager.Instance.TryMovePlayerTo(targetNode))
                {
                    GameStateManager.Instance.EndPlayerTurn();
                }
            }
        }
    }

    private void HandleMouseClickedWhenSpecialAction(int button, Vector3 position, GameObject clickedGO)
    {
        if (button == 0)
        {
            Node targetNode = clickedGO.GetComponent<Node>();
            if (UnitManager.Instance.TryUseSpecialAction(targetNode))
            {
                GameStateManager.Instance.EndPlayerTurn();
            }
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
}
