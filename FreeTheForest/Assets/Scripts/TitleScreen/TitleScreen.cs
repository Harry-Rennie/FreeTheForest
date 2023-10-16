using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class TitleScreen : MonoBehaviour
{
    [SerializeField] public GameObject audioManager;
    [SerializeField] public GameObject volumeOn;
    [SerializeField] public GameObject volumeOff;
    [SerializeField] public GameObject settingsPanel;
    // Start is called before the first frame update
    void Start()
    {
        Clean();
        if(volumeOn.activeSelf == true)
        {
            audioManager.GetComponent<AudioManager>().PlayMusic("ForestTheme");
        }
        else
        {
            audioManager.GetComponent<AudioManager>().PlayMusic("ForestTheme");
            audioManager.GetComponent<AudioManager>().ToggleMusic();
        }
    }

    void Clean()
    {
        //if playerinfocontroller exists, destroy it
        if (PlayerInfoController.instance != null)
        {
            Destroy(PlayerInfoController.instance.gameObject);
        }
        if (TrinketManager.Instance != null)
        {
            Destroy(TrinketManager.Instance.gameObject);
        }
        if (MobManager.Instance != null)
        {
            Destroy(MobManager.Instance.gameObject);
        }

    }

}
