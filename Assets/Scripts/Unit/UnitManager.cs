using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public enum EnemyType
{
    Pursuer,
    Spawner,
    Runner
}

[System.Serializable]
public class EnemyEntry
{
    public EnemyType type;
    public Enemy prefab;
}

public class UnitManager : MonoBehaviour
{
    public static UnitManager Instance { get; private set; }

    private List<Enemy> enemies = new List<Enemy>();

    [SerializeField] private Player playerPrefab;

    [SerializeField] private List<EnemyEntry> enemyEntries;
    private Dictionary<EnemyType, Enemy> enemyPrefabs;

    public Player Player { get; private set; }
    public List<Enemy> Enemies
    {
        get
        {
            RemoveDestroyedEnemies();
            return enemies;
        }
        private set
        {
            enemies = value;
        }
    }

    private void Awake()
    {
        Instance = this;
        enemyPrefabs = enemyEntries.ToDictionary(entry => entry.type, entry => entry.prefab);
    }

    public void InitializePlayer()
    {
        if (Player == null)
        {
            Player = Instantiate(playerPrefab, Map.Instance.transform);
            Player.Initialize(Map.Instance.StartPillar);
        }
    }

    public void SpawnEnemyWithSpawner()
    {
        List<Spawner> spawners = Enemies.Where(enemy => enemy is Spawner)
                                         .Cast<Spawner>()
                                         .ToList();
        if (spawners.Count > 0)
        {
            foreach (Spawner spawner in spawners)
            {
                if (spawner.IsReadyToSpawn)
                {
                    List<Node> spawnPoints = spawner.SpawnPoints;
                    foreach (Node spawnPoint in spawnPoints)
                    {
                        if (!spawnPoint.IsOccupied)
                        {
                            SpawnEnemy(spawnPoint, Random.value > 0.2 ? EnemyType.Pursuer : EnemyType.Runner);
                        }
                    }

                    spawner.ResetSpawnTimer();
                }
            }
        }
    }

    public void SpawnWave(int count)
    {
        for (int i = 0; i < count; i++)
        {
            SpawnEnemyAtRandomSpawnPoint(EnemyType.Spawner);
        }
    }

    public void SpawnEnemyAtRandomSpawnPoint(EnemyType type)
    {
        List<Node> spawnPoints = Map.Instance.SpawnPoints;
        if (spawnPoints.Count > 0)
        {
            List<Node> emptySpawnPoints = spawnPoints.FindAll(node => !node.IsOccupied);
            SpawnEnemy(emptySpawnPoints[Random.Range(0, emptySpawnPoints.Count)], type);
        }
    }

    public void SpawnEnemy(Node node, EnemyType type)
    {
        if (node == null || node.IsOccupied) return;
        Enemy enemy = Instantiate(enemyPrefabs[type], Map.Instance.transform);
        enemy.Initialize(node, Player);
        Enemies.Add(enemy);
    }

    public void RemoveDestroyedEnemies()
    {
        enemies.RemoveAll(enemy => enemy == null || enemy.gameObject == null || enemy.IsDestroyed);
    }
}
