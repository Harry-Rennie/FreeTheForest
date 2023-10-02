using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;

public class Navigation : MonoBehaviour
{
    [SerializeField] List <Button> encounterButtons;

    public void Awake()
    {
    } 

    public void LoadEncounter()
    {
        SceneLoader.SceneNames.Add("Battle");
        SceneManager.LoadSceneAsync("Battle");
    }

    public void LoadHeal()
    {
        SceneManager.LoadScene("Heal");
    }

    public void Continue()
    {
        //need additional logic to not duplicate node objects in scene
        SceneManager.LoadScene("Overworld");
    }
}