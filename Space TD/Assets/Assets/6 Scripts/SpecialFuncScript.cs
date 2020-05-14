using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpecialFuncScript : MonoBehaviour {

    public static SpecialFuncScript instance;
	// Use this for initialization
	void Awake() {
        if (instance == null)
            instance = this;
        

	}
	
	// Update is called once per frame
	void Update () {
		
	}

    public static void DisplayPanel(Animator anim, GameObject gameObject)
    {
        if (gameObject.activeSelf == false)
            gameObject.SetActive(true);

        if (!anim.GetBool("Display"))
        {
            anim.SetBool("Display", true);
        }
        else
        {
            anim.SetBool("Display", false);
        }
    }
}
