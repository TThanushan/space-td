using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Waves
{
    public string name;

    public Wave[] wavesArray;

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
