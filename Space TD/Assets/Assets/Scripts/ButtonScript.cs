using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ButtonScript : MonoBehaviour {

    public string clipName = "Balloon Popping";

    void OnMouseEnter()
    {
        if (clipName == "")
            return;
        
        print("pop");
        AudioManager.instance.Play(clipName, true);
    }


}
