//Using statements
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;
using System.Linq;

// This script controls the functionality of the player info panel
// There are fields for player image, their stats, and their inventory


public class PlayerInfoPanel : MonoBehaviour
{
    private PlayerInfoController playerInfoController;
    private TrinketManager trinketManager;
    //private List<Image> trinketImages = new List<Image>();
    public List<TrinketSlot> TrinketSlots = new List<TrinketSlot>();
    public TMP_Text PlayerStats;

    // singleton pattern
    public static PlayerInfoPanel instance;
    private void Awake()
    {
        if (instance != null)
        {
            Debug.LogWarning("More than one instance of PlayerInfoPanel found!");
            return;
        }
        instance = this;
    }

    // Start is called before the first frame update    
    void Start()
    {
        playerInfoController = PlayerInfoController.instance;
        trinketManager = TrinketManager.instance;
        // populate the list of TrinketSlots by finding all TrinketSlot components in the scene
        TrinketSlots = FindObjectsOfType<TrinketSlot>().Reverse().ToList();

        UpdateTrinkets();
        UpdateStats();
    }

    public void Update()
    {
        //UpdateStats();
        //UpdateTrinkets();
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
        for (int i = 0; i < TrinketSlots.Count; i++)
        {
            if (i < trinketManager.PlayerTrinkets.Count)
            {
            TrinketSlots[i].SetTrinket(trinketManager.PlayerTrinkets[i]);
            }
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

    public void TestClick()
    {
        Debug.Log("Clicked");
    }

}