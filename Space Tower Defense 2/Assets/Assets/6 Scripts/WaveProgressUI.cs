using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaveProgressUI : MonoBehaviour
{
    public Transform progressBar;
    public Transform hitBar;

    private float maxValue;
    private float currentValue;
    private SpawnerScript spawner;


    private void Start()
    {
        spawner = SpawnerScript.instance;
        //SpawnerScript.instance.OnWaveStart +=         
    }

    private void Update()
    {
        SetMaxValue();
        SetCurrentValue();
        SetProgressBarLength();
        DecreaseHitBarSize();
    }

    private void DecreaseHitBarSize()
    {
        float decreaseSpeed = 0.25f;
        float lengthGoal = currentValue / maxValue;
        float newLength = hitBar.localScale.x - Time.deltaTime * decreaseSpeed;
        if (newLength < lengthGoal)
        {
            SetHitBarLocalScaleX(lengthGoal);
            return;
        }
        else if (newLength == lengthGoal)
            return;
        SetHitBarLocalScaleX(newLength);
    }

    private void SetHitBarLocalScaleX(float x)
    {
        hitBar.transform.localScale = new Vector3(x, hitBar.transform.localScale.y, hitBar.transform.localScale.z);
    }

    private void SetProgressBarLength()
    {
        float newLength = currentValue / maxValue;
        progressBar.localScale = new Vector2(newLength, progressBar.localScale.y);
    }

    private void SetCurrentValue()
    {
        float newCurrentValue = 0f;
        
        foreach (Waves.EnemyType enemy in spawner.GetCurrentWave().enemyTypes)
        {
            newCurrentValue += spawner.GetUpdatedEnemyHealth(enemy) * enemy.currentEnemyCount;
        }
        foreach (GameObject enemy in PoolObject.instance.enemies)
        {
            newCurrentValue += enemy.GetComponent<ProgressBarScript>().currentHealth;
        }
        currentValue = newCurrentValue;
    }

    private void SetMaxValue()
    {
        maxValue = spawner.GetWaveTotalHealth();
    }
}
