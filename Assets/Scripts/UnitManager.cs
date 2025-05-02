using System.Collections.Generic;
using UnityEngine;

public class UnitManager : MonoBehaviour
{
    public static UnitManager Instance { get; private set; }

    [SerializeField] private Player playerPrefab;
    [SerializeField] private Enemy enemyPrefab;

    public Player Player { get; private set; }
    public List<Enemy> Enemies { get; private set; } = new List<Enemy>();

    private void Awake()
    {
        Instance = this;
    }

    public void InitializePlayer()
    {
        if (Player == null)
        {
            Player = Instantiate(playerPrefab);
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
        Enemy enemy = Instantiate(enemyPrefab);
        enemy.Initialize(node, Player);
        Enemies.Add(enemy);
    }    
}
