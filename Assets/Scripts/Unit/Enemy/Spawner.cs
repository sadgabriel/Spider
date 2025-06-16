using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Spawner : Enemy
{
    [SerializeField] private int spawnInterval = 5;
    [SerializeField] private int enemyCount = 3;

    private int spawnTimer;

    public List<Node> SpawnPoints
    {
        get
        {
            List<Node> spawnPoints = new List<Node>();
            foreach (Node neighbor in CurrentNode.Neighbors)
            {
                if (!neighbor.IsOccupied)
                {
                    spawnPoints.Add(neighbor);
                }
            }

            return spawnPoints.OrderBy(n => Random.value).Take(enemyCount).ToList();
        }
    }

    public bool IsReadyToSpawn => spawnTimer >= spawnInterval;

    protected override void Awake()
    {
        base.Awake();
        spawnTimer = spawnInterval - 2;
    }
    public override void Act()
    {
        base.Act();

        spawnTimer++;
        if (spawnTimer >= spawnInterval - 1)
        {
            State = EnemyState.Alerted;
        }
    }

    public void ResetSpawnTimer()
    {
        spawnTimer = 0;
        State = EnemyState.Idle;
    }
}
