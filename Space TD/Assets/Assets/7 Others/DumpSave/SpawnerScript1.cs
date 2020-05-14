using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnerScript1 : MonoBehaviour {

    public static SpawnerScript1 instance;

    public Transform spawnPosition;

    public EnemyType[] enemyTypes;

    public BossType[] bossTypes;



    public int numberOfWaves;

    public int enemiesRemainingAlive;
    
    public int currentWaveNumber = 0;

//    [HideInInspector]
    public int enemiesNumberToSpawn;
//    [HideInInspector]
    public int enemiesRemainingToSpawn;

//    [HideInInspector]
    public float nextWaveTime;

    PoolObject poolScript;

    public bool allWavesDone = false;

    PlayerStatsScript playerStats;

    void Awake()
    {
        if (instance == null)
            instance = this;
        playerStats = PlayerStatsScript.instance;
        poolScript = GameObject.FindGameObjectWithTag("Data").GetComponent<PoolObject>();
    }

    void Start () 
    {
        nextWaveTime = 10;

        if (numberOfWaves > 0)
            NextWaveFunc();  
    }

    void Update () 
    {
        if (playerStats.IsGamePaused)
            return;

        if (UIScript.instance.playerLoose)
            return;

        foreach (EnemyType _currentEnemyType in enemyTypes)
        {
            SpawnEnemiesFunc(_currentEnemyType);

        }

        NextWaveFunc();

        if (nextWaveTime > 0 && enemiesRemainingAlive <= 0 && enemiesRemainingToSpawn <= 0)
            nextWaveTime -= Time.deltaTime;
    }

    void SpawnEnemiesFunc(EnemyType currentEnemyType)
    {
        if (currentWaveNumber < currentEnemyType.waveStart)
            return;


        if (Time.time >= currentEnemyType.nextEnemySpawnTime && currentEnemyType.enemyCount > 0)
        {

            currentEnemyType.enemyCount--;
            enemiesRemainingToSpawn--;
            enemiesRemainingAlive++;

            currentEnemyType.nextEnemySpawnTime = Time.time + currentEnemyType.timeBetweenSpawn;

            GameObject newUnit = poolScript.GetPoolObject(currentEnemyType.enemie);
			ProgressBarScript progressBarScript = newUnit.GetComponent<ProgressBarScript>();
			progressBarScript.maxHealth += (currentWaveNumber / 10);
			progressBarScript.currentHealth = newUnit.GetComponent<ProgressBarScript>().maxHealth;
			progressBarScript.OnDeath += OnEnemyDeath;

            newUnit.transform.position = spawnPosition.position;

        }

    }

    int GetEnemyTypeWeight(float weight)
    {
        int enemyNumber = (int)(enemiesNumberToSpawn * weight);

        if (enemyNumber < 1)
        {
            enemyNumber = 1;
        }

        return enemyNumber;
    }

    void OnEnemyDeath()
    {
        enemiesRemainingAlive--;

    }

    void IsThereWavesRemaining()
    {
        //Is there waves remaining ?
        if (currentWaveNumber >= numberOfWaves )
        {
            allWavesDone = true;
            nextWaveTime = 0;
            return;
        }
    }

    void NextWaveFunc()
    {
        IsThereWavesRemaining();

        if (enemiesRemainingAlive <= 0 && enemiesRemainingToSpawn <= 0 && nextWaveTime <= 0)
        {
            if (allWavesDone)
            {
                currentWaveNumber++;
                return;
            }
            currentWaveNumber++;

            if(currentWaveNumber != numberOfWaves - 1)
                nextWaveTime = 10;

            enemiesNumberToSpawn = currentWaveNumber + 10;

            foreach (EnemyType currentEnemyType in enemyTypes)
            {
                if (currentEnemyType.waveStart > currentWaveNumber)
                    continue;
                currentEnemyType.enemyCount = GetEnemyTypeWeight(currentEnemyType.weight);
                enemiesRemainingToSpawn += currentEnemyType.enemyCount;
            }
        }
    }

    [System.Serializable]
    public class EnemyType
    {
        public GameObject enemie;
        public int waveStart;
        public float weight;
        public float timeBetweenSpawn;

        public int enemyCount;
        public float nextEnemySpawnTime;
    }

    [System.Serializable]
    public class BossType
    {
        public GameObject enemie;
        public int waveStart;
        public bool bossIsDead = false;
    }

}
