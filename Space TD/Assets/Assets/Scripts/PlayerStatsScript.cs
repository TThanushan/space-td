using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerStatsScript : MonoBehaviour {

    public static PlayerStatsScript instance;

    public int life = 10;
    public float money = 40;

	void Awake() {
        if (instance == null)
            instance = this;
	}
	
	void Update () {

	}
}
