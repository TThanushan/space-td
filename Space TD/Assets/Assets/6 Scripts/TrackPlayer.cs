using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrackPlayer : MonoBehaviour
{
    public AudioSource myAudio;
    public AudioClip[] myMusic; // declare this as Object array
    public static TrackPlayer instance;
    int trackHistory;

    private void Awake()
    {
        DontDestroyOnLoad(gameObject);
    }
    void Start()
    {
        if (!instance)
            instance = this;
        else
        {
            Destroy(gameObject);
            return;
        }
        myAudio = GetComponent<AudioSource>();
        if (!myAudio.isPlaying)
            PlayRandomMusic();
    }

    void Update()
    {
        if (!myAudio.isPlaying || Input.GetKeyDown(KeyCode.A))
            PlayRandomMusic();
    }

    public void MuteMusic()
    {
        myAudio.mute = !myAudio.mute;
    }

    public void PlayRandomMusic()
    {
        if (myAudio.enabled)
        {
            int rand = Random.Range(0, myMusic.Length);

            while (rand == trackHistory)
                rand = Random.Range(0, myMusic.Length);

            myAudio.clip = myMusic[rand] as AudioClip;
            myAudio.Play();
            trackHistory = rand;
        }
    }
}
