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
        // add all images withe the tag TrinketImage to the list of trinket images
        foreach (GameObject trinketImage in GameObject.FindGameObjectsWithTag("TrinketImage"))
        {
            Debug.Log("Adding trinket image");  
            trinketImages.Add(trinketImage.GetComponent<Image>());
        }
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
        PlayerStats.text = "Health: " + playerInfoController.CurrentHealth + "/" + playerInfoController.MaxHealth + "\n" +
            "Strength: " + playerInfoController.Strength + "\n" +
            "Defense: " + playerInfoController.Defense;
    }

    public void UpdateTrinkets()
    {
        // set each trinket sprite in trinketManager.PlayerTrinkets to the corresponding image in trinketImages
        for (int i = 0; i < trinketManager.PlayerTrinkets.Count; i++)
        {
            trinketImages[i].sprite = trinketManager.PlayerTrinkets[i].Sprite;
        }
    }
}