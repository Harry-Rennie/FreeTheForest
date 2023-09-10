//Using statements
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;

// This script controls the functionality of the player info panel
// There are fields for player image, their stats, and their inventory


public class PlayerInfoPanel : MonoBehaviour
{
    private PlayerInfoController playerInfoController;
    private TrinketManager trinketManager;
    private List<Image> trinketImages = new List<Image>();
    public TMP_Text PlayerStats;
    
    // Start is called before the first frame update    
    void Start()
    {
        Color targetColor = Color.red;
        playerInfoController = PlayerInfoController.instance;
        trinketManager = TrinketManager.instance;
        // add all images with the tag TrinketImage to the list of trinket images
        foreach (GameObject trinketImage in GameObject.FindGameObjectsWithTag("TrinketImage"))
        {
            Debug.Log("Adding trinket image");  
            trinketImages.Add(trinketImage.GetComponent<Image>());
        }
        // reverse the list of trinket images so that the trinkets are displayed in the correct order
        trinketImages.Reverse();
        // debug log the contents of playerInfoController.PlayerTrinkets
        foreach (Trinket trinket in trinketManager.PlayerTrinkets)
        {
            Debug.Log(trinket.Title);
        }
        // set each trinket sprite in trinketManager.PlayerTrinkets to the corresponding image in trinketImages
        for (int i = 0; i < trinketManager.PlayerTrinkets.Count; i++)
        {
            trinketImages[i].sprite = trinketManager.PlayerTrinkets[i].Sprite;
        }
    }

    public void Update()
    {
        UpdateStats();
        UpdateTrinkets();       
    }

    public void UpdateStats()
    {
        PlayerStats.text = 
        @$"Health: {playerInfoController.CurrentHealth}/{playerInfoController.MaxHealth} {getBuffString(trinketManager.TotalHealthBuff)}
Strength: {playerInfoController.Strength} {getBuffString(trinketManager.TotalStrengthBuff)}
Defense: {playerInfoController.Defense} {getBuffString(trinketManager.TotalDefenseBuff)}";
    }

    public void UpdateTrinkets()
    {
        // set each trinket sprite in trinketManager.PlayerTrinkets to the corresponding image in trinketImages
        for (int i = 0; i < trinketManager.PlayerTrinkets.Count; i++)
        {
            trinketImages[i].sprite = trinketManager.PlayerTrinkets[i].Sprite;
        }
    }

    // this method takes an int and returns it as a string enclosed in parentheses
    // if the int is negative, the string will be red
    // if the int is positive, the string will be green
    private string getBuffString(int buff)
    {
        string buffString = "";
        if (buff != 0)
        {
            buffString = (buff > 0) ? $"<color=#18ba30>({buff})</color>" : $"<color=#FF0000>({buff})</color>";
            return buffString;
        }
        return buffString;
    }

}