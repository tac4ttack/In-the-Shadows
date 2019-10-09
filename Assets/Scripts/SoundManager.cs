using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundManager : MonoBehaviour
{
    [HideInInspector] public static SoundManager sm { get; private set; }

    public List<AudioClip> Musics = new List<AudioClip>();
    public List<AudioClip> Sounds = new List<AudioClip>();
    public AudioSource MusicSrc;
    public AudioSource EffectSrc;
    public float GeneralVolume = 50;
    public float MusicVolume = 50;
    public float EffectVolume = 50;

    private void Awake()
    {
        List<AudioSource> AS = new List<AudioSource>();
        GetComponents<AudioSource>(AS);
        if (AS.Count != 2)
            Debug.Log("Error?");
        else
            Debug.Log("Audio source ok!");

        if (MusicSrc == null)
            MusicSrc = AS[0];
            MusicSrc = GetComponent<AudioSource>();
        if (EffectSrc == null)
            EffectSrc = AS[1];
            EffectSrc = GetComponent<AudioSource>();
        // OLD WAY
        // if (!sm)
        //     sm = this;

        // NEW WAY
        // Singleton setup
        if (sm == null)
        {
            sm = this;
        }
        else if (sm != this)
        {
            Destroy(gameObject);   
        }
        DontDestroyOnLoad(this.gameObject);
    }
}
