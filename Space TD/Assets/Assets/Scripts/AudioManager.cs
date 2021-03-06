﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
public class AudioManager : MonoBehaviour {

    public static AudioManager instance;

    public List<Sound> sounds;

    public bool musicMuted = false;

    public bool SFXMuted = false;


    void Start()
    {
		Play("Theme", false);
        Play("Brown Noise Ambient", false);

        if (instance == null)
            instance = this;
        else
        {
            Destroy(gameObject);
            return;
        }
        AudioListener.volume = 0.10f;
    }

    void Update()
    {

    }

    void Awake()
    {
        DontDestroyOnLoad(gameObject);


        for (int i = 0; i < 1; i++)
        {
            foreach (Sound s in sounds)
            {
                s.source = gameObject.AddComponent<AudioSource>();
                s.source.clip = s.clip;
                s.source.volume = s.volume;
                s.source.pitch = s.pitch;
                s.source.loop = s.loop;

                
            }
        }



    }

    public void Play(string _name, bool randomPitch)
    {
        if (SFXMuted == true)
            return;


        Sound newS = sounds.Find(Sound => Sound.name == _name && Sound.name == _name && Sound.source.isPlaying == false);

        //Was the sound already playing ?
        if (newS == null)
        {
            //Get a model of the sound.
            Sound s = sounds.Find(Sound => Sound.name == _name);

            if (s != null)
            {
                //Create and add the new Sound to the list.
                newS = AddSound(s.name, s.clip, s.volume, s.pitch, s.loop);
            }
            //It means that the name is wrong !
            else
            {
                Debug.LogWarning("Sound name : " + _name + " not found !");
                return;
            }

        }

        if (newS != null)
        {
            if (randomPitch == true)
                newS.source.pitch = UnityEngine.Random.Range(0.8f, 1.8f);
            
            newS.source.Play();
        }
    }

    public void MuteSfxVolume(GameObject _image)
    {
        SFXMuted = !SFXMuted;

        if (_image != null)
        {
            _image.SetActive(SFXMuted);
        }

    }
        
    public void MuteMusicVolume(GameObject _image)
    {
        sounds.Find(Sound => Sound.name == "Theme").source.mute = !sounds.Find(Sound => Sound.name == "Theme").source.mute;

        sounds.Find(Sound => Sound.name == "Brown Noise Ambient").source.mute = !sounds.Find(Sound => Sound.name == "Brown Noise Ambient").source.mute;

        musicMuted = !musicMuted;

        if (_image != null)
        {
            _image.SetActive(musicMuted);
        }
    }

    public void ChangeMainVolume(float _volume)
    {
        AudioListener.volume = _volume;

    }

    Sound AddSound(string name, AudioClip clip, float volume, float pitch, bool loop)
    {
        Sound s = new Sound();

        s.source = gameObject.AddComponent<AudioSource>();
        s.name = name;
        s.clip = clip;
        s.volume = volume;
        s.pitch = pitch;
        s.loop = loop;

        s.source.clip = s.clip;
        s.source.volume = s.volume;
        s.source.pitch = s.pitch;
        s.source.loop = s.loop;
    
        sounds.Add(s);
        return s;
    }
}
