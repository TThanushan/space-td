using UnityEngine;
using System.Collections;

public class KeyPressManager : MonoBehaviour
{
    public static KeyPressManager instance;
    public float downTime, upTime, pressTime = 0;
    public float countDown = 2.0f;
    public bool mouse1KeyReady = false;

    private void Awake()
    {
        if (instance == null)
            instance = this;
        else
            print("There is already an KeyPressManager Script");
    }

    private void Update()
    {
        mouse1KeyReady = IsRightMouseKeyHolded(KeyCode.Mouse1);
    }

    public bool IsRightMouseKeyHolded(KeyCode _keyCode)
    {
        if (Input.GetKeyDown(_keyCode))
        {
            downTime = Time.time;
            pressTime = downTime + countDown;
            return false;
        }
        if (Time.time >= pressTime && Input.GetKey(_keyCode))
            return true;
        return false;
    }

}
