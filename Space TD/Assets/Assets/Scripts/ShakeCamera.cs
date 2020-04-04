using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShakeCamera : MonoBehaviour {
    public static ShakeCamera instance;
    public float intensity;
    public float duration;
    public float fadeSpeed;

    float startTime;
    float t;
    public float durationRemaining;
    bool isShaking;
    bool fading = false;


    Vector3 initialPosition;

	// Use this for initialization
	void Awake () 
    {
        if (instance == null)
            instance = this;
        
        initialPosition = transform.position;
	}
	
	// Update is called once per frame
	void Update () 
    {

        if (isShaking)
        {
            durationRemaining -= Time.deltaTime;
            transform.position = initialPosition + new Vector3(Random.Range(-intensity, intensity), Random.Range(-intensity, intensity), initialPosition.z);
            SlowDownShaking();
        }
        else
            StopShaking();
    }

    public void Shake(float _intensity = 1, float _duration = 1)
    {
//        startTime = Time.time;
        durationRemaining = _duration;
        intensity = _intensity;
        duration = _duration;
        isShaking = true;
        fading = false;
        CancelInvoke();
        Invoke("StopShaking", duration);
    }

    void SlowDownShaking()
    {
        if (durationRemaining > 1)
            return;
        if (fading == false)
        {
            fading = true;
            startTime = Time.time;
        }
        

        t = (Time.time - startTime) * fadeSpeed; 
        intensity = Mathf.Lerp(intensity, 0, t);
    }

    public void StopShaking()
    {
        isShaking = false;
        transform.position = initialPosition;
    }
}
