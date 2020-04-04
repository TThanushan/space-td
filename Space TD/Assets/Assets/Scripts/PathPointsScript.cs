using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PathPointsScript : MonoBehaviour {

    public Transform[] points;




	void Awake () 
    {
	    
        //Create a new array with X length.
        points = new Transform[transform.childCount];

        //Add each of them in the array.
        for (int i = 0; i < points.Length; i++)
        {
            points[i] = gameObject.transform.GetChild(i);
        }

	}
	
	void Update () {
		
	}
}
