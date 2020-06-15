using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GenerateWavesScriptableObject : MonoBehaviour
{
    public Waves waves;
    public GameObject[] prefabs;
    public GameObject[] bossPrefabs;

    private int currentPrefabsIndex;
    private int currentBossPrefabsIndex;
    private int numberOfWaves = 50;
    private int moneyReward = 200;

    public Waves fileWhereToSave;

    private void Awake()
    {
        GenerateWaves();
    }

    private void GenerateWaves()
    {
        List<Waves.Wave> newWaveArray = new List<Waves.Wave>(numberOfWaves);

        for (int i = 0; i < numberOfWaves; i++)
        {
            AddNewPrefab(i);
            Waves.Wave newWave = new Waves.Wave();
            newWave.enemyTypes = new Waves.EnemyType[GetNewEnemyTypeSize(i)];
            for (int y = 0; y <= currentPrefabsIndex; y++)
            {
                newWave.enemyTypes[y] = new Waves.EnemyType
                {
                    Enemy = prefabs[y],
                    TimeBetweenSpawn = GetTimeBetweenSpawn(i),
                    EnemyCount = GetEnemyCount(i, y)
                };
            }
            if (NeedToAddBoss(i))
            {
                int lastIndex = newWave.enemyTypes.Length - 1;
                newWave.enemyTypes[lastIndex] = new Waves.EnemyType
                {
                    Enemy = bossPrefabs[currentBossPrefabsIndex],
                    TimeBetweenSpawn = 5,
                    EnemyCount = 1,
                };
                AddBossPrefab(i);
            }
            newWave.moneyReward = moneyReward;
            newWaveArray.Add(newWave);
        }

        waves.wavesArray = newWaveArray.ToArray();
        fileWhereToSave = waves;
    }

    private int GetNewEnemyTypeSize(int currentWaveNumber)
    {
        int size = currentPrefabsIndex + 1;
        if (NeedToAddBoss(currentWaveNumber))
            size++;
        return size;
    }

    private int GetEnemyCount(int waveNumber, int prefabIndex)
    {
        return Mathf.RoundToInt(((float)waveNumber + 1f) * 
            ((2f + (float)prefabIndex) / ((float)prefabIndex + 1f)));
    }

    private void AddNewPrefab(int currentWaveNumber)
    {
        if (currentWaveNumber != 0  && currentWaveNumber % 3 == 0 &&
            currentPrefabsIndex < prefabs.Length - 1)
            currentPrefabsIndex++;
    }

    private void AddBossPrefab(int currentWaveNumber)
    {
        if (NeedToAddBoss(currentWaveNumber))
            currentBossPrefabsIndex++;
    }

    private bool NeedToAddBoss(int currentWaveNumber)
    {
        int everyXWave = 5;
        return currentWaveNumber != 0 && (currentWaveNumber + 1) % everyXWave == 0 &&
            currentBossPrefabsIndex < bossPrefabs.Length;
    }

    private float GetTimeBetweenSpawn(int waveNumber)
    {
        float defaultTime = 1f;
        float minTime = 0.3f;
        float newTime = defaultTime - (waveNumber / 50f);
        if (newTime < minTime)
            newTime = minTime;
        return newTime;
    }

}
