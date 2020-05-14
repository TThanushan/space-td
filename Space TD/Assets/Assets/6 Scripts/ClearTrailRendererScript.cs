using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClearTrailRendererScript : MonoBehaviour {

	void Start () {
		
	}

    void OnDisable()
    {
        if (GetComponent<TrailRenderer>() == true)
        {
            GetComponent<TrailRenderer>().Clear();
        }
    }
	
	void Update () {
		
	}
}
