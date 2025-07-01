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

        var enemies = enemyWaveDatas[waveIndex].Enemies;
        float totalProportion = 0f;

        foreach (var enemy in enemies)
        {
            if (enemy.Proportion < 0f)
            {
                Debug.LogError($"Negative proportion for enemy type {enemy.Type} in wave {waveIndex}: {enemy.Proportion}");
                continue;
            }

            totalProportion += enemy.Proportion;
        }

        if (totalProportion == 0f)
        {
            Debug.LogError("Total enemy proportion is zero.");
            return enemyProportions;
        }

        foreach (var enemy in enemies)
        {
            enemyProportions[enemy.Type] = enemy.Proportion / totalProportion;
        }

        return enemyProportions;
    }

    public bool IsBossWave(int waveIndex)
    {
        if (enemyWaveDatas == null || enemyWaveDatas.Count <= waveIndex)
        {
            Debug.LogError("enemyWaveData is not setted.");
            return false;
        }
        return enemyWaveDatas[waveIndex].IsBossWave;
    }
}