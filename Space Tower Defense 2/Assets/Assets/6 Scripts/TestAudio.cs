using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestAudio : MonoBehaviour
{
    public Sound sound;
    // Start is called before the first frame update
    void Awake()
    {
        sound.source = gameObject.AddComponent<AudioSource>();
        sound.source.clip = sound.clip;
        sound.source.volume = sound.volume;
        sound.source.pitch = sound.pitch;
        sound.source.loop = sound.loop;
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.anyKeyDown)
            Play();
    }

    public void Play()
    {
        Debug.Log("play");
        if (sound == null)
            return;
        sound.source.pitch = sound.pitch;
        sound.source.volume = sound.volume;

        sound.source.Play();
    }
}
