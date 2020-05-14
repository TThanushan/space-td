using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class PlayerStatsScript : MonoBehaviour {

    public static PlayerStatsScript instance;
    
    public delegate void PauseAction();
    public static event PauseAction PauseEvent;

    public delegate void UnpauseAction();
    public static event UnpauseAction UnpauseEvent;

    public int life = 10;
    public float money = 40;
    public bool pause;
    public float timeWhenPaused;
    void Awake() {
        if (instance == null)
            instance = this;
	}
    public bool IsGamePaused { get { return pause; } }
    public void PauseGame()
    {
        pause = !pause;
        if (pause )
        {
            timeWhenPaused = Time.time;
            if (PauseEvent != null)
                PauseEvent();
        }
        else if (!pause && UnpauseEvent != null)
            UnpauseEvent();
    }
    public float GetTimeElapsedSincePause()
    {
        return Time.time - timeWhenPaused;
    }
}
