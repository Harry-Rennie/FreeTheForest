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
        SceneLoader.SceneNames = new List<string> { "Battle" };
        SceneManager.LoadScene("Battle");
    }
}