using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Create new Waves Setting")]
public class Waves : ScriptableObject
{
    public Wave[] wavesArray;

    private void Awake()
    {
        
    }

    public void ResetCurrentWaveTemporaryData(int currentWaveNumber)
    {
        foreach (EnemyType enemyType in wavesArray[currentWaveNumber].enemyTypes)
        {
            enemyType.currentEnemyCount = enemyType.EnemyCount;
            enemyType.nextEnemySpawnTime = 0f;
        }
    }

    [System.Serializable]
    public class EnemyType
    {
        public GameObject Enemy;
        public float TimeBetweenSpawn;
        public int EnemyCount;

        [HideInInspector]
        public int currentEnemyCount;
        [HideInInspector]
        public float nextEnemySpawnTime;

        public void ResetNextSpawnTime()
        {
            nextEnemySpawnTime = Time.time + TimeBetweenSpawn;
        }
        public bool ReadyToSpawn()
        {
            return Time.time >= nextEnemySpawnTime && currentEnemyCount > 0;
        }

    }

    [System.Serializable]
    public class Wave
    {
        public int moneyReward;
        public EnemyType[] enemyTypes;
    }
}
