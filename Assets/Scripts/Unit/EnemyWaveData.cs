using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(menuName = "EnemyWaveData")]
public class EnemyWaveData : ScriptableObject
{
    [System.Serializable]
    public struct EnemySpawnEntry
    {
        public EnemyType type;
        public float proportion;
    }

    public int MaxSpawners;
    public List<EnemySpawnEntry> enemies;
}