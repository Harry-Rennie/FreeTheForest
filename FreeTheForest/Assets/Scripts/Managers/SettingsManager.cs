/// <summary>
/// This class is used to manage the settings menus.
/// </summary>

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SettingsManager : MonoBehaviour
{
    #region Singleton
    public static SettingsManager Instance;
    private Canvas canvas;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            canvas = GetComponent<Canvas>();
        }
        else
        {
            Destroy(gameObject);
            return;
        }
        DontDestroyOnLoad(gameObject);
    }
    #endregion
    public UnityEngine.UI.Slider MusicSlider;
    public UnityEngine.UI.Slider SFXSlider;

    public void Start()
    {
        // init levels
        MusicSlider.value = MusicManager.Instance.InitMusicLevel;
        SFXSlider.value = SFXManager.Instance.InitSFXLevel;

        // rig up change methods
        SFXSlider.onValueChanged.AddListener(delegate { SFXManager.Instance.ChangeSFXVolume(SFXSlider.value); });
        MusicSlider.onValueChanged.AddListener(delegate { MusicManager.Instance.ChangeMusicVolume(MusicSlider.value); });
    }

    // find child "SettingsPanel" and set it to active
    public void OpenSettings()
    {
        canvas.transform.Find("SettingsPanel").gameObject.SetActive(true);
    }

    // find child "SettingsPanel" and toggle its active state
    public void CloseSettings()
    {
        canvas.transform.Find("SettingsPanel").gameObject.SetActive(false);
    }
}
