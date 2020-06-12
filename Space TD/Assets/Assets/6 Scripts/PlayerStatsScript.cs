using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[System.Serializable]
public class PlayerStatsScript {

    private static PlayerStatsScript _instance;
    public static PlayerStatsScript instance
    {
        get
        {
            if (_instance == null)
                _instance = new PlayerStatsScript();
            return _instance;
        }
    }

    public delegate void PauseAction();
    public static event PauseAction PauseEvent;

    public delegate void UnpauseAction();
    public static event UnpauseAction UnpauseEvent;

    public int life = 50;
    public float money = 300f;
    public bool pause;
    public float timeWhenPaused;

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
    public void PauseGame(bool value)
    {
        pause = value;
        if (pause)
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
