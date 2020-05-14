using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DisableScript : MonoBehaviour {

    public float destroyTime = 1f;
    float destroyTimeCount;
    PlayerStatsScript playerStats;

    void OnEnable() {
        playerStats = PlayerStatsScript.instance;
        destroyTimeCount = destroyTime;
    }
	
	void Update () {
        if (playerStats && playerStats.IsGamePaused)
            return;
        if (destroyTimeCount <= 0f)
            Destroy();
        destroyTimeCount -= Time.deltaTime;
    }

    void Destroy()
    {
        gameObject.SetActive(false);
    }
}
