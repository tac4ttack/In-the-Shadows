using System.Collections.Generic;
using UnityEngine;

public class SoundManager : MonoBehaviour
{
    public List<AudioClip> Musics = new List<AudioClip>();
    public List<AudioClip> Sfx = new List<AudioClip>();
    public AudioSource MusicSrc;
    public AudioSource SfxSrc;

    private void Awake()
    {
        #if UNITY_EDITOR || DEVELOPMENT_BUILD
        // DEBUG
        Debug.Log($"SOUND MANAGER - {this.name} - Awake()");
        #endif
        
        List<AudioSource> AS = new List<AudioSource>();
        GetComponents<AudioSource>(AS);
        
        // DEBUG
        #if UNITY_EDITOR
        if (AS.Count != 2)
        {
            Debug.LogError("SoundManager does not seems to have the correct number of Audio sources");
        }
        #endif

        if (MusicSrc == null)
            MusicSrc = AS[0];
        if (SfxSrc == null)
            SfxSrc = AS[1];
    }
}
