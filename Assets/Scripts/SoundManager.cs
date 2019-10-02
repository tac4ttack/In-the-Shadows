using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundManager : MonoBehaviour
{
    public static SoundManager sm { get; private set; }

    public List<AudioClip> Musics = new List<AudioClip>();
    public List<AudioClip> Sounds = new List<AudioClip>();
    public AudioSource CommonSrc;

    private void Awake()
    {
        if (!CommonSrc)
            CommonSrc = GetComponent<AudioSource>();
        if (!sm)
            sm = this;
    }
}
