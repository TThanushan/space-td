using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DisableScript : MonoBehaviour {

    public float destroyTime = 1f;
	void OnEnable() {
        
        Invoke("Destroy", destroyTime);
    }
	
	void Update () {
		
	}

    void Destroy()
    {
        gameObject.SetActive(false);
    }
}
