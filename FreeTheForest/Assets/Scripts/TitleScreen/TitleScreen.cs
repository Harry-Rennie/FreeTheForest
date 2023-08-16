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
    // Start is called before the first frame update
    void Start()
    {
        if(volumeOn.activeSelf == true)
        {
            audioManager.GetComponent<AudioManager>().PlayMusic("ForestTheme");
        }
        else
        {
            audioManager.GetComponent<AudioManager>().PlayMusic("ForestTheme");
            audioManager.GetComponent<AudioManager>().ToggleMusic();
        }

        SceneManager.LoadSceneAsync("MainMenu", LoadSceneMode.Additive);      
    }

}
