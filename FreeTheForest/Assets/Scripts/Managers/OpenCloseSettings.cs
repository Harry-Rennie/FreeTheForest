using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OpenCloseSettings : MonoBehaviour
{
    public void OpenSettings()
    {
        SettingsManager.Instance.OpenSettings();
    }

    public void CloseSettingsSettings()
    {
        SettingsManager.Instance.CloseSettings();
    }
}

