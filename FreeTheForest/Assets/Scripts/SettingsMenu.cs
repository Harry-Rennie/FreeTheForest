using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SettingsMenu : MonoBehaviour
{
    // public reference to music and sfx sliders
    public UnityEngine.UI.Slider musicSlider;
    public UnityEngine.UI.Slider sfxSlider;

    public void Start(){
        // log out the component this is attached to
        Debug.Log(this);
        // log out the sliders
        Debug.Log("Music Slider: " + musicSlider);
        Debug.Log("SFX Slider: " + sfxSlider);
        // log out sfx manager
        Debug.Log(SFXManager.Instance);
        // log out statement checking managers are initialised
        musicSlider.value = MusicManager.Instance.InitMusicLevel;
        sfxSlider.value = SFXManager.Instance.InitSFXLevel;
        // set slider values to current music and sfx volumes
        // log out these values:
        // Debug.Log(SFXManager.Instance);



        // onvaluechanged listeners for sliders
        // pass slider value to method in SFXManager
        sfxSlider.onValueChanged.AddListener(delegate {SFXManager.Instance.ChangeSFXVolume(sfxSlider.value);});
        musicSlider.onValueChanged.AddListener(delegate {MusicManager.Instance.ChangeMusicVolume(musicSlider.value);});
    }
}
