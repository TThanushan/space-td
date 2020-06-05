using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GenerateWavesScriptableObject : MonoBehaviour
{
    public Waves waves;
    public GameObject[] prefabs;

    private int currentPrefabsIndex;
    private int numberOfWaves = 100;
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
            Waves.Wave newWave = new Waves.Wave
            {
                enemyTypes = new Waves.EnemyType[currentPrefabsIndex + 1]
            };
            for (int y = 0; y <= currentPrefabsIndex; y++)
            {
                newWave.enemyTypes[y] = new Waves.EnemyType();
                newWave.enemyTypes[y].Enemy = prefabs[y];
                newWave.enemyTypes[y].TimeBetweenSpawn = GetTimeBetweenSpawn(i);
                newWave.enemyTypes[y].EnemyCount = GetEnemyCount(i, y);
            }
            newWave.moneyReward = moneyReward;
            newWaveArray.Add(newWave);
        }

        waves.wavesArray = newWaveArray.ToArray();
        fileWhereToSave = waves;
    }

    private int GetEnemyCount(int waveNumber, int prefabIndex)
    {
        return Mathf.RoundToInt(((float)waveNumber + 1f) * ((5f + (float)prefabIndex) / ((float)prefabIndex + 1f)));
    }

    private void AddNewPrefab(int currentWaveNumber)
    {
        if (currentWaveNumber != 0 && currentWaveNumber % 3 == 0 && currentPrefabsIndex < prefabs.Length - 1)
            currentPrefabsIndex++;
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

    //We add a new wave
    //Then we add a new wave derived from the previous one

}
