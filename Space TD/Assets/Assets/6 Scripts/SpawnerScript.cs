using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif
public class SpawnerScript : MonoBehaviour {

    public static SpawnerScript instance;
    public Transform spawnPosition;
    public Waves waves;
    public int currentWaveNumber = 0;
    public int enemiesRemainingAlive;
    //Waiting, InProgress
    public string waveState;
    public int enemiesRemainingToSpawn;
    private float nextWaveTime;
    public bool isAllWavesDone = false;
    public int numberOfWaves;

    public event System.Action OnWaveOver;
    public event System.Action OnWaveStart;

    PoolObject poolScript;
    PlayerStatsScript playerStats;
    DifficultyData difficultyData;
    int playerHealthLoss;

    void Awake()
    {
        difficultyData = new DifficultyData();
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
        if (playerStats.IsGamePaused)
            return;

        CheckIfIsAllWavesDone();
        if (UIScript.instance.playerLoose || isAllWavesDone)
            return;
        if (WaveStarting())
        {
            WaveStart();
            SpawnEnemies();
            WaveOver();

        }
    }

    public float GetWaveTotalHealth()
    {
        float total = 0f;
        foreach (Waves.EnemyType enemyType in GetCurrentWave().enemyTypes)
        {
            total += GetUpdatedEnemyHealth(enemyType) * enemyType.EnemyCount;
        }
        return total;
    }

    public Waves.EnemyType[] GetEnemyTypes()
    {
        return GetCurrentWave().enemyTypes;
    }


    private void UpdateEnemyGoldReward(GameObject newEnemy, Waves.EnemyType currentEnemyType)
    {
         ProgressBarScript progressBar = newEnemy.GetComponent<ProgressBarScript>();
         progressBar.moneyGiven = currentEnemyType.Enemy.GetComponent<ProgressBarScript>().moneyGiven + difficultyData.GoldPoint;
    }


    private void UpdateEnemyCount()
    {
        foreach (Waves.EnemyType _currentEnemyType in GetCurrentWave().enemyTypes)
        {
            _currentEnemyType.currentEnemyCount += (int)difficultyData.SpawnPoint;
        }
    }

    public Waves.EnemyType[] GetNextWaveEnemyType()
    {
        Waves.EnemyType[] nextWave = GetCurrentWave().enemyTypes;
        foreach (Waves.EnemyType _currentEnemyType in nextWave)
        {
            _currentEnemyType.currentEnemyCount = _currentEnemyType.EnemyCount;
            //_currentEnemyType.currentEnemyCount += difficultyData.SpawnPoint;
        }
        return nextWave;
    }

    public bool WaveIsInProgress()
    {
        return waveState == "InProgress";
    }

    private void DebugInput()
    {
        if (Input.GetKeyDown(KeyCode.Keypad1) && currentWaveNumber > 0)
            currentWaveNumber--;
        if (Input.GetKeyDown(KeyCode.Keypad3) && currentWaveNumber < numberOfWaves - 1)
            currentWaveNumber++;
        if (Input.GetKey(KeyCode.Keypad4))
            playerStats.money += 15;
    }

    public void IncreasePlayerHealthLoss()
    {
        playerHealthLoss++;
    }

    public void StartWave()
    {
        nextWaveTime = 0f;
    }

    void WaveStart()
    {
        if (waveState == "Waiting" && enemiesRemainingToSpawn <= 0 && enemiesRemainingAlive <= 0)
        {
            PlayerStatsScript.UnpauseEvent += UpdateCurrentWaveSpawnTime;
            waves.ResetCurrentWaveTemporaryData(currentWaveNumber);
            CountEnemiesToSpawn();
            NextWavePreview.instance.HidePreview();
            waveState = "InProgress";
            OnWaveStart?.Invoke();
        }
    }

    public void UpdateCurrentWaveSpawnTime()
    {
        foreach (Waves.EnemyType enemyType in waves.wavesArray[currentWaveNumber].enemyTypes)
        {
            enemyType.nextEnemySpawnTime += playerStats.GetTimeElapsedSincePause();
        }
    }

    private void UpdateDifficulty()
    {
        if (playerHealthLoss == 0)
            difficultyData.IncreaseDifficulty();
        else
            difficultyData.DecreaseDifficulty(playerHealthLoss);
        playerHealthLoss = 0;
    }

    private void ApplyDifficulty()
    {
        UpdateEnemyCount();
        difficultyData.ApplyMultiplier();
    }

    void WaveOver()
    {
        if (waveState == "InProgress" && CurrentWaveOver() && WaveStarting())
        {
            PlayerStatsScript.UnpauseEvent -= UpdateCurrentWaveSpawnTime;
            waveState = "Waiting";
            OnWaveOver?.Invoke();
            currentWaveNumber++;
            if (isAllWavesDone)
                return;
            if (currentWaveNumber != numberOfWaves)
            { 
                nextWaveTime = 10;
                playerStats.money += waves.wavesArray[currentWaveNumber].moneyReward;
                UIScript.instance.PlayGainMoneyAnimation();
                UpdateDifficulty();
                ApplyDifficulty();
               NextWavePreview.instance.ShowPreview();
            }
        }
    }

    public ref Waves.Wave GetCurrentWave()
    {
        return ref waves.wavesArray[currentWaveNumber];
    }

    public Waves.Wave GetNextWave()
    {
        if (isAllWavesDone)
            return null;
        return waves.wavesArray[currentWaveNumber+1];
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
        if (currentEnemyType.ReadyToSpawn())
        {
            currentEnemyType.ResetNextSpawnTime();
            InstantiateEnemy(currentEnemyType);
            DecreaseSpawnerCounter(ref currentEnemyType);
        }
    }

    void DecreaseSpawnerCounter(ref Waves.EnemyType currentEnemyType)
    {
        currentEnemyType.currentEnemyCount--;
        enemiesRemainingToSpawn--;
        enemiesRemainingAlive++;
    }

    GameObject InstantiateEnemy(Waves.EnemyType currentEnemyType)
    {
        GameObject newUnit = poolScript.GetPoolObject(currentEnemyType.Enemy);
        newUnit.GetComponent<ProgressBarScript>().OnDeath += OnEnemyDeath;
        UpdateEnemyHealth(newUnit, currentEnemyType);
        UpdateEnemyGoldReward(newUnit, currentEnemyType);
        newUnit.transform.position = spawnPosition.position;
        return newUnit;
    }

    private void UpdateEnemyHealth(GameObject newEnemy, Waves.EnemyType currentEnemyType)
    {
        ProgressBarScript progressBar = newEnemy.GetComponent<ProgressBarScript>();
        progressBar.maxHealth = currentEnemyType.Enemy.GetComponent<ProgressBarScript>().maxHealth + difficultyData.StatusPoint;
        if (progressBar.maxHealth <= 0)
            progressBar.maxHealth = 1;
        progressBar.currentHealth = progressBar.maxHealth;
    }

    public float GetUpdatedEnemyHealth(Waves.EnemyType currentEnemyType)
    {
        return currentEnemyType.Enemy.GetComponent<ProgressBarScript>().maxHealth + difficultyData.StatusPoint;
    }

    void OnEnemyDeath(GameObject obj)
    {
        enemiesRemainingAlive--;
        obj.GetComponent<ProgressBarScript>().OnDeath -= OnEnemyDeath;
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
            enemiesRemainingToSpawn += currentEnemyType.currentEnemyCount;
    }


    [System.Serializable]
    class DifficultyData
    {
        private float statusPoint = 1;
        private float spawnPoint = 1;
        private float goldPoint = 1;

        private float statusMultiplier = 1;
        private float spawnMultiplier = 1;
        private float goldMultiplier = 1;

        private float statusPointMax = Mathf.Infinity;
        private float statusMultiplierMax = 1.125f;
        private float spawnPointMax = 0;

        public float StatusPoint { get => Mathf.Floor(statusPoint);}
        public float SpawnPoint { get => Mathf.Floor(spawnPoint);}
        public float GoldPoint { get => Mathf.Round(goldPoint);}

        public float StatusMultiplier { get => statusMultiplier;}
        public float SpawnMultiplier { get => spawnMultiplier;}
        public float GoldMultiplier { get => goldMultiplier;}

        public void DecreaseDifficulty(float healthLose)
        {
            if (healthLose <= 3)
            {
                statusMultiplier -= 0.1f;
                spawnMultiplier -= 0.08f;
                goldMultiplier += 0.07f;
            }
            else if (healthLose <= 7)
            {
                statusMultiplier -= 0.12f;
                spawnMultiplier -= 0.1f;
                goldMultiplier += 0.09f;
                statusPoint /= 2;
                spawnPoint /= 2;
            }
            else if (healthLose < 10)
            {
                statusMultiplier -= 0.17f;
                spawnMultiplier -= 0.14f;
                goldMultiplier += 0.12f;
                statusPoint /= 3;
                spawnPoint /= 3;
            }
            else
            {
                statusMultiplier -= 0.2f;
                spawnMultiplier -= 0.16f;
                goldMultiplier += 0.15f;
                statusPoint /= 4;
                spawnPoint /= 4;
            }

            ApplyMaxRange();
        }

        private void ResetMultiplier()
        {
            statusMultiplier = 1;
            spawnMultiplier = 1;
            goldMultiplier = 1;
        }

        public void IncreaseDifficulty()
        {
            spawnMultiplier += 0.05f;
            goldMultiplier -= 0.05f;
            statusMultiplier += 0.025f;

            statusPoint += 0.125f;
            spawnPoint += 0.25f;
            ApplyMaxRange();
        }

        private void ApplyMaxRange()
        {
            if (statusPoint > statusPointMax)
                statusPoint = statusPointMax;
            if (spawnPoint > spawnPointMax)
                spawnPoint = spawnPointMax;
            if (statusMultiplier > statusMultiplierMax)
                statusMultiplier = statusMultiplierMax;
        }

        private void ApplyMinRange()
        {
            if (statusPoint < 0)
                statusPoint = 0;
            if (SpawnPoint < 0)
                statusPoint = 0;
            if (goldPoint < 0)
                goldPoint = 0;
        }
        public void ApplyMultiplier()
        {
            statusPoint *= statusMultiplier;
            spawnPoint *= spawnMultiplier;
            goldPoint *= goldMultiplier;
            ApplyMaxRange();
        }
    }
}
