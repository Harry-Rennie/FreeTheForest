using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

///<summary>
/// This singleton class manages the trinkets in the game.
/// It contains a list of all trinkets in the game.
/// It also contains a list of all trinkets that the player has.
/// It provides a method to award a random trinket from the list of all trinkets, based on the level of the dungeon.
/// It also handles the equipping and unequipping of trinkets.
///</summary>

public class TrinketManager : MonoBehaviour
{
    [NonSerialized] public List<Trinket> AllTrinkets;
    public List<Trinket> PlayerTrinkets;

    private PlayerInfoController playerInfoController;

    #region Buff Properties
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
    #endregion

    #region Singleton
    // singleton pattern
    public static TrinketManager Instance;
    private void Awake()
    {
        if (Instance != null)
        {
            Debug.LogWarning("More than one instance of TrinketManager found.");
            // destroy myself
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);
    } 
    #endregion

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

        // for all trinkets in PlayerTrinkets, equip the trinket
        foreach (Trinket trinket in PlayerTrinkets)
        {
            if (trinket.Equipped)
            {
                EquipTrinket(trinket);
            }
        }

    }
    
    /// <summary>
    /// This method returns a random trinket from the list of all trinkets that are at the specified level.
    /// It is intended to be used when the player is in a shop or is awarded a trinket.
    /// </summary>
    /// <param name="level">The level of the dungeon that the trinket is found in.</param>
    public Trinket GetTrinket(int level)
    {
        // get a list of all trinkets that are at the specified level
        List<Trinket> trinketsAtLevel = AllTrinkets.Where(trinket => trinket.Level == level).ToList();

        // get a random trinket from the list of trinkets at the specified level
        Trinket trinket = trinketsAtLevel[UnityEngine.Random.Range(0, trinketsAtLevel.Count)];

        return trinket;
    }

    /// <summary>
    /// This method removes the trinket from the list of all trinkets and adds it to the list of player trinkets.
    /// It also equips the trinket.
    /// </summary>
    /// <param name="trinket">The trinket to be added.</param>
    public void AddTrinket(Trinket trinket)
    {
        AllTrinkets.Remove(trinket);
        PlayerTrinkets.Add(trinket);
        EquipTrinket(trinket);
    }

    /// <summary>
    /// This method applies the buffs from the trinket to the player stats.
    /// </summary>
    /// <param name="trinket">The trinket to be equipped.</param>
    public void EquipTrinket(Trinket trinket)
    {
        trinket.Equipped = true;
        playerInfoController.MaxHealth += trinket.MaxHealthBuff;
        playerInfoController.Strength += trinket.AttackBuff;
        playerInfoController.Defense += trinket.DefenseBuff;
    }

    /// <summary>
    /// This method removes the buffs from the trinket from the player stats.
    /// </summary>
    /// <param name="trinket">The trinket to be unequipped.</param>
    public void UnequipTrinket(Trinket trinket)
    {
        trinket.Equipped = false;
        playerInfoController.MaxHealth -= trinket.MaxHealthBuff;
        playerInfoController.Strength -= trinket.AttackBuff;
        playerInfoController.Defense -= trinket.DefenseBuff;
    }
}