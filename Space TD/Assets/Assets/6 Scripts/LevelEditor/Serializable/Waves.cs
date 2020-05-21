using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "create new Wave")]
public class Waves : ScriptableObject
{
    public Wave[] wavesArray;

    public void ResetCurrentWaveTemporaryData(int currentWaveNumber)
    {
        foreach (EnemyType enemyType in wavesArray[currentWaveNumber].enemyTypes)
        {
            enemyType.currentEnemyCount = enemyType.EnemyCount;
            enemyType.nextEnemySpawnTime = 0f;
        }
    }

    [System.Serializable]
    public class Wave
    {
        public List<EnemyType> enemyTypes;
    }

    [System.Serializable]
    public class EnemyType
    {
        public GameObject Enemy;
        public int Health;
        public float MoveSpeed;
        public int MoneyRewardPerUnit;
        public float TimeBetweenSpawn;
        public int EnemyCount;

        [HideInInspector]
        public int currentEnemyCount;
        [HideInInspector]
        public float nextEnemySpawnTime;
    }
}
