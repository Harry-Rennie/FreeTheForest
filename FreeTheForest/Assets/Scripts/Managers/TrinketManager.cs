using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using UnityEngine;

///<summary>
/// This singleton class manages the trinkets in the game.
/// It contains a list of all trinkets in the game.
/// It also contains a list of all trinkets that the player has.
/// It provides a method to award a random trinket from the list of all trinkets, based on the level of the dungeon.
///</summary>

public class TrinketManager : MonoBehaviour
{
    public List<Trinket> AllTrinkets;
    public List<Trinket> PlayerTrinkets;
    public static TrinketManager instance;
    private PlayerInfoController playerInfoController;

    // this property returns the total health buff from all equipped trinkets
    public int TotalHealthBuff
    {
        get
        {
            int totalHealthBuff = 0;
            foreach (Trinket trinket in PlayerTrinkets)
            {
                if (trinket.Equipped)
                {
                    totalHealthBuff += trinket.MaxHealthBuff;
                }
            }
            return totalHealthBuff;
        }
    }

    // this property returns the total strength buff from all equipped trinkets
    public int TotalStrengthBuff
    {
        get
        {
            int totalStrengthBuff = 0;
            foreach (Trinket trinket in PlayerTrinkets)
            {
                if (trinket.Equipped)
                {
                    totalStrengthBuff += trinket.AttackBuff;
                }
            }
            return totalStrengthBuff;
        }
    }

    // this property returns the total defense buff from all equipped trinkets
    public int TotalDefenseBuff
    {
        get
        {
            int totalDefenseBuff = 0;
            foreach (Trinket trinket in PlayerTrinkets)
            {
                if (trinket.Equipped)
                {
                    totalDefenseBuff += trinket.DefenseBuff;
                }
            }
            return totalDefenseBuff;
        }
    }

    private void Awake()
    {
        // implement a singleton pattern
        if (instance == null)
            instance = this;
        else
        {
            Destroy(gameObject);
            return;
        }
        DontDestroyOnLoad(gameObject);

    }

    // Start is called before the first frame update
    void Start()
    {
        playerInfoController = PlayerInfoController.instance;
        // Load all Trinket assets from the "Resources/Trinkets" folder.
        AllTrinkets = Resources.LoadAll<Trinket>("Trinkets").ToList();


        // Check if the trinkets were loaded successfully.
        if (AllTrinkets.Count > 0)
        {
            Debug.Log("Loaded " + AllTrinkets.Count + " trinkets.");
        }
        else
        {
            Debug.LogWarning("No trinkets found in Resources/Trinkets.");
        }

    }
    
    public Trinket GetTrinket(int level)
    {
        // get a list of all trinkets that are at the specified level
        List<Trinket> trinketsAtLevel = AllTrinkets.Where(trinket => trinket.Level == level).ToList();

        // get a random trinket from the list of trinkets at the specified level
        Trinket trinket = trinketsAtLevel[UnityEngine.Random.Range(0, trinketsAtLevel.Count)];

        return trinket;
    }
    public void AddTrinket(Trinket trinket)
    {
        AllTrinkets.Remove(trinket);
        PlayerTrinkets.Add(trinket);
        EquipTrinket(trinket);
    }

    public void EquipTrinket(Trinket trinket)
    {
        trinket.Equipped = true;
        playerInfoController.MaxHealth += trinket.MaxHealthBuff;
        playerInfoController.Strength += trinket.AttackBuff;
        playerInfoController.Defense += trinket.DefenseBuff;
    }

    public void UnequipTrinket(Trinket trinket)
    {
        trinket.Equipped = false;
        playerInfoController.MaxHealth -= trinket.MaxHealthBuff;
        playerInfoController.Strength -= trinket.AttackBuff;
        playerInfoController.Defense -= trinket.DefenseBuff;
    }
}