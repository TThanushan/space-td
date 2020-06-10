using UnityEngine;
using System.Collections;

public class PlayerPower : MonoBehaviour
{
    public float reloadTime;
    public Transform reloadBar;
    public GameObject laserBeamPrefab;
    public Animator animator;

    private float currentReloadTime;

    private void Update()
    {
        if (PlayerStatsScript.instance.IsGamePaused || !SpawnerScript.instance.WaveIsInProgress())
            return;
        if (currentReloadTime > 0)
            currentReloadTime -= Time.deltaTime;
        else if (currentReloadTime < 0)
        {
            animator.Play("Show");
            currentReloadTime = 0;
        }
        UpdateReloadBarLength();
    }


    private void UpdateReloadBarLength()
    {
        reloadBar.localScale = new Vector2(currentReloadTime/reloadTime, reloadBar.localScale.y);
    }

    public void UsePower()
    {
        if (currentReloadTime > 0 || !SpawnerScript.instance.WaveIsInProgress())
            return;
        AudioManager.instance.Play("PowerActivation");
        currentReloadTime = reloadTime;
        InstantiateLaserBeam();
    }

    private void InstantiateLaserBeam()
    {
        Transform[] pathArray = PoolObject.instance.pathArray;
        GameObject laserBeam = PoolObject.instance.GetPoolObject(laserBeamPrefab);

        laserBeam.transform.position = pathArray[pathArray.Length - 1].position;

    }

}
