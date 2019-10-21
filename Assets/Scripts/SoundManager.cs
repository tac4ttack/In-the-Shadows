﻿using System.Collections.Generic;
using UnityEngine;

public class SoundManager : MonoBehaviour
{
    public List<AudioClip> Musics = new List<AudioClip>();
    public List<AudioClip> Sfx = new List<AudioClip>();
    public AudioSource MusicSrc;
    public AudioSource SfxSrc;

    private void Start()
    {
        List<AudioSource> AS = new List<AudioSource>();
        GetComponents<AudioSource>(AS);
        if (AS.Count != 2)
            Debug.Log("SoundManager does not seems to have the correct number of Audio sources");
        else
            Debug.Log("Audio sources successfully loaded!");

        if (MusicSrc == null)
            MusicSrc = AS[0];
        if (SfxSrc == null)
            SfxSrc = AS[1];

        // Singleton setup
        // OLD WAY
        // if (!sm)
        //     sm = this;
        // NEW WAY
        // if (sm == null)
        // {
        //     sm = this;
        // }
        // else if (sm != this)
        // {
        //     Destroy(gameObject);   
        // }

        DontDestroyOnLoad(this.gameObject);
    }
}
