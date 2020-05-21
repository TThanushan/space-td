using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using TMPro;

public class WavesEditor : MonoBehaviour
{
    public static WavesEditor instance;

    public string wavesName;

    private List<Waves.Wave> waves;
    private List<Waves.EnemyType> enemyTypes;
    private int waveIndex = 0;

    private void Awake()
    {
        if (instance == null)
            instance = this;
        else
            print("There is already an levelEditor Script");
        InitVariables();
    }

    private void InitVariables()
    {
        waves = new List<Waves.Wave>();
        waves.Add(new Waves.Wave());
        AddEnemyTypeToCurrentWave();   
    }

    public void SetWaveIndex(int value)
    {
        waveIndex = value;
    }

    public int GetNumberOfWaves()
    {
        return waves.Count;
    }

    public void AddNewWave()
    {
        waves.Add(new Waves.Wave());
        SetWaveIndex(waves.Count - 1);
        AddEnemyTypeToCurrentWave();
    }

    public void RemoveCurrentWave()
    {
        if (waves.Count <= 1)
            return;
        waves.RemoveAt(waveIndex);
        SetWaveIndex(waves.Count - 1);
    }

    public void UpdateEnemyList()
    {
        foreach (Waves.EnemyType enemyType in waves[waveIndex].enemyTypes)
        {
            //GetEnemyTypeBlockInfoGameObject(newBlock, "Health").GetComponent<TMP_InputField>().text = 
        }
    }

    private void AddEnemyTypeToCurrentWave()
    {
        if (waves[waveIndex].enemyTypes == null)
            waves[waveIndex].enemyTypes = new List<Waves.EnemyType>();
        waves[waveIndex].enemyTypes.Add(new Waves.EnemyType());
    }


}
