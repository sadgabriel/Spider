using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System.Collections;

public enum EnemyType
{
    Pursuer,
    Spawner,
    Runner,
    Corroder,
    Boss,
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

    public IEnumerator DoSpawnEnemyWithBoss()
    {
        Boss boss = Enemies.OfType<Boss>().FirstOrDefault();
        if (boss != null && boss.IsReadyToSpawn)
        {
            List<bool> isDoneList = new List<bool>();

            foreach (Node spawnPoint in boss.SpawnPoints)
            {
                if (!spawnPoint.IsOccupied)
                {
                    isDoneList.Add(false);
                    int index = isDoneList.Count - 1;
                    IEnumerator wrapped = Utils.DoRunAndNotify(DoSpawnEnemyWithMotion(spawnPoint, EnemyType.Runner, boss.CurrentNode), () => isDoneList[index] = true);
                    StartCoroutine(wrapped);
                }
            }

            yield return new WaitUntil(() => isDoneList.All(done => done));

            boss.IsReadyToSpawn = false;
        }
    }

    public IEnumerator DoSpawnEnemyWithSpawner(Dictionary<EnemyType, float> enemyProportions)
    {
        List<Spawner> spawners = Enemies.Where(enemy => enemy is Spawner)
                                         .Cast<Spawner>()
                                         .ToList();
        if (spawners.Count > 0)
        {
            List<List<bool>> isDoneList = new List<List<bool>>();

            foreach (Spawner spawner in spawners)
            {
                if (spawner.IsReadyToSpawn)
                {
                    isDoneList.Add(new List<bool>());

                    List<Node> spawnPoints = spawner.SpawnPoints;
                    foreach (Node spawnPoint in spawnPoints)
                    {
                        if (!spawnPoint.IsOccupied)
                        {
                            float randomValue = Random.value;
                            EnemyType enemyType = enemyProportions.Keys.FirstOrDefault();
                            float cumulativeProbability = 0f;
                            foreach (var entry in enemyProportions)
                            {
                                cumulativeProbability += entry.Value;
                                if (randomValue <= cumulativeProbability)
                                {
                                    enemyType = entry.Key;
                                    break;
                                }
                            }

                            isDoneList.Last().Add(false);
                            int spawnerIndex = isDoneList.Count - 1;
                            int spawnIndex = isDoneList.Last().Count - 1;

                            IEnumerator wrapped = Utils.DoRunAndNotify(DoSpawnEnemyWithMotion(spawnPoint, enemyType, spawner.CurrentNode), () => isDoneList[spawnerIndex][spawnIndex] = true);
                            StartCoroutine(wrapped);
                        }
                    }

                    spawner.ResetSpawnTimer();
                }
            }

            yield return new WaitUntil(() => isDoneList.All(list => list.All(done => done)));
        }
    }

    public void SpawnSpawners(int count)
    {
        for (int i = 0; i < count; i++)
        {
            SpawnEnemyAtRandomSpawnPoint(EnemyType.Spawner);
        }
    }

    public void SpawnBoss()
    {
        SpawnEnemyAtRandomSpawnPoint(EnemyType.Boss);
    }

    public void SpawnEnemyAtRandomSpawnPoint(EnemyType type)
    {
        List<Node> spawnPoints = Map.Instance.SpawnPoints;

        if (spawnPoints.Count > 0)
        {
            List<Node> emptySpawnPoints = spawnPoints.FindAll(node => !node.IsOccupied);
            SpawnEnemyWithoutMotion(emptySpawnPoints[Random.Range(0, emptySpawnPoints.Count)], type);
        }
    }

    public void SpawnEnemyWithoutMotion(Node node, EnemyType type, Node spawnerNode = null)
    {
        if (node == null || node.IsOccupied) return;
        Enemy enemy = Instantiate(enemyPrefabs[type], Map.Instance.transform);
        enemy.Initialize(node, Player);
        Enemies.Add(enemy);

        if (spawnerNode != null && enemy is Mover mover)
        {
            Node targetNode = node.Neighbors.FirstOrDefault(n => n != spawnerNode);
            mover.SetTargetPillar(targetNode);
        }
    }

    public IEnumerator DoSpawnEnemyWithMotion(Node node, EnemyType type, Node spawnerNode, float duration = 0.2f)
    {
        if (node == null || node.IsOccupied) yield break;

        if (spawnerNode == null)
        {
            SpawnEnemyWithoutMotion(node, type, spawnerNode);
            yield break;
        }

        Enemy enemy = Instantiate(enemyPrefabs[type], Map.Instance.transform);
        enemy.Initialize(node, Player);
        Enemies.Add(enemy);

        if (enemy is Mover mover)
        {
            Node targetNode = node.Neighbors.FirstOrDefault(n => n != spawnerNode);
            mover.SetTargetPillar(targetNode);
        }

        float elapsed = 0f;
        while (elapsed < duration)
        {
            Vector3 startPosition = enemy.CalcUnitPosition(spawnerNode);
            Vector3 endPosition = enemy.CalcUnitPosition(node);

            float t = elapsed / duration;
            enemy.transform.position = Vector3.Lerp(startPosition, endPosition, t);

            elapsed += Time.deltaTime;
            yield return null;
        }
    }

    public void RemoveDestroyedEnemies()
    {
        enemies.RemoveAll(enemy => enemy == null || enemy.gameObject == null || enemy.IsDead);
    }
}
