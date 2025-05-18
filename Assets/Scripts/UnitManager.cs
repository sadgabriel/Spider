using System.Collections.Generic;
using UnityEngine;

public class UnitManager : MonoBehaviour
{
    public static UnitManager Instance { get; private set; }

    private List<Enemy> enemies = new List<Enemy>();

    [SerializeField] private Player playerPrefab;
    [SerializeField] private Enemy enemyPrefab;

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
    }

    public void InitializePlayer()
    {
        if (Player == null)
        {
            Player = Instantiate(playerPrefab, MapManager.Instance.Origin);
            Player.Initialize(MapManager.Instance.StartPillar);
        }
    }

    public void SpawnWave(int count)
    {
        for (int i = 0; i < count; i++)
        {
            SpawnEnemyAtRandomSpawner();
        }
    }

    public void SpawnEnemyAtRandomSpawner()
    {
        List<Node> spawners = MapManager.Instance.Spawners;
        if (spawners.Count > 0)
        {
            List<Node> emptySpawners = spawners.FindAll(node => !node.IsOccupied);
            SpawnEnemy(emptySpawners[Random.Range(0, emptySpawners.Count)]);
        }
    }

    public void SpawnEnemy(Node node)
    {   
        if (node == null || node.IsOccupied) return;
        Enemy enemy = Instantiate(enemyPrefab, MapManager.Instance.Origin);
        enemy.Initialize(node, Player);
        Enemies.Add(enemy);
    }

    public void RemoveDestroyedEnemies()
    {
        enemies.RemoveAll(enemy => enemy == null || enemy.gameObject == null);
    }
}
