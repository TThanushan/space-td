using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnerScript : MonoBehaviour {

    public static SpawnerScript instance;

    public Transform spawnPosition;

    //public EnemyType[] enemyTypes;

    public Waves waves;


    public int currentWaveNumber = 0;

    public int enemiesRemainingAlive;

    //Waiting, InProgress
    public string waveState;

    public int enemiesRemainingToSpawn;

    public float nextWaveTime;

    PoolObject poolScript;

    public bool isAllWavesDone = false;
    PlayerStatsScript playerStats;

    public int numberOfWaves;

    void Awake()
    {
        waveState = "Waiting";
        if (instance == null)
            instance = this;
        playerStats = PlayerStatsScript.instance;
        poolScript = GameObject.FindGameObjectWithTag("Data").GetComponent<PoolObject>();
        nextWaveTime = 10;
        numberOfWaves = waves.wavesArray.Length;
    }


    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Keypad1) && currentWaveNumber > 0)
            currentWaveNumber--;
        if (Input.GetKeyDown(KeyCode.Keypad3) && currentWaveNumber < numberOfWaves - 1)
            currentWaveNumber++;

        if (Input.GetKey(KeyCode.Keypad4))
            playerStats.money++;

        if (playerStats.IsGamePaused)
            return;

        CheckIfIsAllWavesDone();
        if (UIScript.instance.playerLoose || isAllWavesDone)
            return;
        if (WaveStarting())
        {
            WaveStart();
            SpawnEnemies();
            StartNextWave();
            WaveOver();
        }
        else
            nextWaveTime -= Time.deltaTime;
    }



    void WaveStart()
    {
        if (waveState == "Waiting" && enemiesRemainingToSpawn <= 0 && enemiesRemainingAlive <= 0)
        {
            PlayerStatsScript.UnpauseEvent += UpdateCurrentWaveSpawnTime;
            AudioManager.instance.Play("Wave Start", false);
            CountEnemiesToSpawn();
            waveState = "InProgress";
        }
    }

    public void UpdateCurrentWaveSpawnTime()
    {
        foreach (Waves.EnemyType enemyType in waves.wavesArray[currentWaveNumber].enemyTypes)
        {
            enemyType.nextEnemySpawnTime += playerStats.GetTimeElapsedSincePause();
        }
    }

    void WaveOver()
    {
        if (waveState == "InProgress" && CurrentWaveOver() && WaveStarting())
        {
            PlayerStatsScript.UnpauseEvent -= UpdateCurrentWaveSpawnTime;
            waveState = "Waiting";
            currentWaveNumber++;
            if (isAllWavesDone)
                return;
            if (currentWaveNumber != numberOfWaves)
                nextWaveTime = 10;
        }
    }

    ref Waves.Wave GetCurrentWave()
    {
        return ref waves.wavesArray[currentWaveNumber];
    }

    void SpawnEnemies()
    {
        foreach (Waves.EnemyType _currentEnemyType in GetCurrentWave().enemyTypes)
        {
            SpawnEnemy(_currentEnemyType);
        }
    }

    void SpawnEnemy(Waves.EnemyType currentEnemyType)
    {
        if (Time.time >= currentEnemyType.nextEnemySpawnTime && currentEnemyType.currentEnemyCount > 0)
        {
            currentEnemyType.nextEnemySpawnTime = Time.time + currentEnemyType.TimeBetweenSpawn;
            GameObject newUnit = poolScript.GetPoolObject(currentEnemyType.Enemy);
			ProgressBarScript progressBarScript = newUnit.GetComponent<ProgressBarScript>();
			IAScript enemyScript = newUnit.GetComponent<IAScript>();

            progressBarScript.maxHealth = currentEnemyType.Health;
            progressBarScript.currentHealth = currentEnemyType.Health;
            enemyScript.startMoveSpeed = currentEnemyType.MoveSpeed;
            enemyScript.moveSpeed = currentEnemyType.MoveSpeed;
            progressBarScript.moneyGiven = currentEnemyType.MoneyRewardPerUnit;

            progressBarScript.OnDeath += OnEnemyDeath;

            newUnit.transform.position = spawnPosition.position;

            currentEnemyType.currentEnemyCount--;
            enemiesRemainingToSpawn--;
            enemiesRemainingAlive++;
        }
    }

    void OnEnemyDeath()
    {
        enemiesRemainingAlive--;
    }

    bool CheckIfIsAllWavesDone()
    {
        if (!isAllWavesDone && currentWaveNumber >= numberOfWaves && enemiesRemainingAlive <= 0 && enemiesRemainingToSpawn <= 0)
        {
            isAllWavesDone = true;
            nextWaveTime = 0;
            AudioManager.instance.Play("Level Complete", false);
            return true;
        }
        return false;
    }

    bool WaveStarting()
    {
        return nextWaveTime <= 0;
    }

    bool CurrentWaveOver()
    {
        return enemiesRemainingToSpawn <= 0 && enemiesRemainingAlive <= 0;
    }

    void CountEnemiesToSpawn()
    {
        foreach (Waves.EnemyType currentEnemyType in GetCurrentWave().enemyTypes)
            enemiesRemainingToSpawn += currentEnemyType.EnemyCount;
    }

    void StartNextWave()
    {
        if (waveState == "Waiting" && CurrentWaveOver() && WaveStarting())
            waveState = "InProgress";
    }


}
