using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelData : MonoBehaviour
{
    public int TrackNumber = -1;

    void Awake()
    {
        if (TrackNumber == -1)
            TrackNumber = 0;
    }

    void Start()
    {
        // SoundManager.sm.MusicSrc.PlayOneShot(SoundManager.sm.Musics[TrackNumber]);
    }

    void OnDestroy()
    {
        SoundManager.sm.MusicSrc.Stop();
    }
}
