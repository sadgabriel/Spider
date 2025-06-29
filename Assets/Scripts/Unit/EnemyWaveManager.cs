using System.Collections.Generic;
using UnityEngine;

class EnemyWaveManager : MonoBehaviour
{
    public static EnemyWaveManager Instance { get; private set; }

    [SerializeField] private List<EnemyWaveData> enemyWaveDatas;

    public int MaxWaves => enemyWaveDatas.Count;
    
    private void Awake()
    {
        Instance = this;
    }

    public int GetMaxSpawners(int waveIndex)
    {
        if (waveIndex < 0 || waveIndex >= enemyWaveDatas.Count)
        {
            Debug.LogError($"Invalid wave index: {waveIndex}");
            return 0;
        }
        return enemyWaveDatas[waveIndex].MaxSpawners;
    }

    public Dictionary<EnemyType, float> GetEnemyProportions(int waveIndex)
    {
        if (waveIndex < 0 || waveIndex >= enemyWaveDatas.Count)
        {
            Debug.LogError($"Invalid wave index: {waveIndex}");
            return new Dictionary<EnemyType, float>();
        }

        Dictionary<EnemyType, float> enemyProportions = new();
        
        var enemies = enemyWaveDatas[waveIndex].enemies;
        float totalProportion = 0f;

        foreach (var enemy in enemies)
        {
            if (enemy.proportion < 0f)
            {
                Debug.LogError($"Negative proportion for enemy type {enemy.type} in wave {waveIndex}: {enemy.proportion}");
                continue;
            }

            totalProportion += enemy.proportion;
        }

        if (totalProportion == 0f)
        {
            Debug.LogError("Total enemy proportion is zero.");
            return enemyProportions;
        }

        foreach (var enemy in enemies)
        {
            enemyProportions[enemy.type] = enemy.proportion / totalProportion;
        }

        return enemyProportions;
    }
}