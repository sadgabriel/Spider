using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(menuName = "EnemyWaveData")]
public class EnemyWaveData : ScriptableObject
{
    [System.Serializable]
    public struct EnemySpawnEntry
    {
        public EnemyType Type;
        public float Proportion;
    }

    public int MaxSpawners;
    public List<EnemySpawnEntry> Enemies;
    public bool IsBossWave;
}