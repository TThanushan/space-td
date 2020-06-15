using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveScript : MonoBehaviour {

    public float moveSpeedX = 1;
    public float moveSpeedY = 0;

    public int wayX = 1;
    public int wayY = 1;

    void FixedUpdate () {

        if (PlayerStatsScript.instance.IsGamePaused)
            return;
        transform.Translate(new Vector2(moveSpeedX * wayX * Time.deltaTime, moveSpeedY * wayY * Time.deltaTime));
    }
}
