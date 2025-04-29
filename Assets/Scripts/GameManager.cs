using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    // public static GameManager Instance { get; private set; }

    // [SerializeField] private MapManager mapManager;
    // [SerializeField] private PlayerController playerPrefab;
    // [SerializeField] private EnemyController enemyPrefab;
    // [SerializeField] private int waveInterval = 5;

    // private PlayerController player;
    // private List<EnemyController> enemies = new();

    // private TurnState currentTurn;
    // private int turnCount = 0;
    // private int waveNumber = 1;

    // private void Awake()
    // {
    //     Instance = this;
    // }

    // private void Start()
    // {
    //     InitializeGame();
    // }

    // private void InitializeGame()
    // {
    //     mapManager.GenerateMap();
    //     InitializePlayer();
    //     SpawnInitialEnemies();
    //     currentTurn = TurnState.PlayerTurn;
    // }

    // private void InitializePlayer()
    // {
    //     Node startNode = mapManager.GetStartNode();
    //     player = Instantiate(playerPrefab);
    //     player.SetStartNode(startNode, mapManager.GetPlayerYOffset());
    // }

    // private void SpawnInitialEnemies()
    // {
    //     foreach (var node in mapManager.GetNodes())
    //     {
    //         if (node != player.CurrentNode && node.Neighbors.Count > 0 && Random.value < 0.05f)
    //         {
    //             SpawnEnemy(node);
    //         }
    //     }
    // }

    // private void SpawnWave(int count)
    // {
    //     for (int i = 0; i < count; i++)
    //     {
    //         SpawnEnemyAtRandomSpawner();
    //     }
    // }

    // private int GetEnemyCountForWave(int wave)
    // {
    //     return 2 + wave;
    // }

    // private void SpawnEnemyAtRandomSpawner()
    // {
    //     var availableSpawners = MapManager.Instance
    //         .GetSpawnerNodes()
    //         .Where(node => !node.IsOccupied)
    //         .ToList();

    //     if (availableSpawners.Count == 0) return;

    //     Node spawnNode = availableSpawners[Random.Range(0, availableSpawners.Count)];
    //     SpawnEnemy(spawnNode);
    // }

    // private void SpawnEnemy(Node spawnNode)
    // {
    //     EnemyController enemy = Instantiate(enemyPrefab);
    //     enemy.SetStartNode(spawnNode, mapManager.GetPlayerYOffset());
    //     enemy.Initialize(player);
    //     enemies.Add(enemy);
    // }

    // public bool IsPlayerTurn()
    // {
    //     return currentTurn == TurnState.PlayerTurn;
    // }

    // public void EndPlayerTurn()
    // {
    //     if (currentTurn != TurnState.PlayerTurn) return;

    //     turnCount++;

    //     if (turnCount % waveInterval == 0)
    //     {
    //         SpawnWave(GetEnemyCountForWave(waveNumber));
    //         waveNumber++;
    //     }

    //     currentTurn = TurnState.EnemyTurn;
    //     StartCoroutine(EnemyTurnRoutine());
    // }

    // private IEnumerator EnemyTurnRoutine()
    // {
    //     Debug.Log("�� �� ����");

    //     var activeEnemies = new List<EnemyController>(enemies);

    //     foreach (var enemy in activeEnemies)
    //     {
    //         if (enemy != null)
    //         {
    //             enemy.Act();
    //             yield return new WaitForSeconds(0.05f);
    //         }
    //     }

    //     enemies.RemoveAll(e => e == null);

    //     Debug.Log("�� �� ����");
    //     currentTurn = TurnState.PlayerTurn;
    // }

    // public void GameOver()
    // {
    //     Debug.Log("Game Over");
    // }
}
