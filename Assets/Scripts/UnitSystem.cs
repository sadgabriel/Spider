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
