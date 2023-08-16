using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

public class AudioManager : MonoBehaviour
{

    public static AudioManager instance; //still needs to be converted to dependency injection
    public Audio[] musicSnd, sfxSnd;
    public AudioSource musicSrc, sfxSrc;

    private void Awake()
    {
        if (instance == null)
            instance = this;
        else
        {
            Destroy(gameObject);
            return;
        }
        DontDestroyOnLoad(gameObject);
    }

    public void PlayMusic(string name)
    {
        Audio a = Array.Find(musicSnd, audio => audio.name == name);
        if(a != null)
        {
            musicSrc.clip = a.clip;
            musicSrc.Play();
        }
    }

    public void ToggleMusic()
    {
        musicSrc.mute = !musicSrc.mute;
    }

    public void PlaySfx(string name)
    {
        Audio s = Array.Find(sfxSnd, audio => audio.name == name);
        if(s != null)
        {
            sfxSrc.clip = s.clip;
            sfxSrc.Play();
        }
    }

    public void ToggleSfx()
    {
        sfxSrc.mute = !sfxSrc.mute;
    }

    public void MusicVolume(float volume)
    { musicSrc.volume = volume; }

    public void SfxVolume(float volume)
    { musicSrc.volume = volume; }
}
