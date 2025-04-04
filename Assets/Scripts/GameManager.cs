using NUnit.Framework;
using System.Collections.Generic;
using System.Collections;
using UnityEngine;

public enum TurnState
{
    PlayerTurn,
    EnemyTurn
}

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    [SerializeField] private MapManager mapManager;
    [SerializeField] private PlayerController playerPrefab;
    [SerializeField] private EnemyController enemyPrefab;

    private PlayerController player;
    private List<EnemyController> enemies = new();

    private TurnState currentTurn;

    private void Awake()
    {
        Instance = this;
    }
    void Start()
    {
        InitializeGame();
    }

    private void InitializeGame()
    {
        mapManager.GenerateMap();
        
        InitializePlayer();
        SpawnInitialEnemies();

        currentTurn = TurnState.PlayerTurn;
    }

    private void InitializePlayer()
    {
        Node startNode = mapManager.GetStartNode();
        player = Instantiate(playerPrefab);
        player.SetStartNode(startNode, mapManager.GetPlayerYOffset());
    }
    private void SpawnInitialEnemies()
    {
        List<Node> nodes = mapManager.GetNodes();
        foreach (Node node in nodes)
        {
            if (node != player.currentNode && node.neighbors.Count > 0 && Random.value < 0.05f)
            {
                SpawnEnemy(node);
            }
        }
    }

    private void SpawnEnemy(Node spawnNode)
    {
        EnemyController enemy = Instantiate(enemyPrefab);
        enemy.SetStartNode(spawnNode, mapManager.GetPlayerYOffset());
        enemy.Initialize(player);
        enemies.Add(enemy);
    }

    public void GameOver()
    {
        Debug.Log("Game Over");
    }

    public bool IsPlayerTurn()
    {
        return currentTurn == TurnState.PlayerTurn;
    }

    public void EndPlayerTurn()
    {
        if (currentTurn != TurnState.PlayerTurn)
        {
            return;
        }

        currentTurn = TurnState.EnemyTurn;
        StartCoroutine(EnemyTurnRoutine());
    }

    private IEnumerator EnemyTurnRoutine()
    {
        Debug.Log("적 턴 시작");

        List<EnemyController> enemiesCopy = new(enemies);

        foreach (EnemyController enemy in enemiesCopy)
        {
            if (enemy != null)
            enemy.Act();
            yield return new WaitForSeconds(0.25f);
        }

        enemies.RemoveAll(enemy => enemy == null);

        Debug.Log("적 턴 종료");

        currentTurn = TurnState.PlayerTurn;
    }
}
