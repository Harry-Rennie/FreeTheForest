using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

///<summary>
/// This class is a scriptable object class for a trinket
/// The trinket contains values for the name, description, and sprite of the trinket
/// as well as values for the stats that the trinket modifies.
/// The trinket also contains a level value that corresponds to the level of the dungeon
///</summary>
[CreateAssetMenu(fileName = "New Trinket", menuName = "Trinket")]
public class Trinket : ScriptableObject
{
    // Trinket name
    public string Title;
    // Trinket description
    public string Description;
    // Trinket sprite
    public Sprite Sprite;
    // Trinket level (corresponds to the level of the dungeon that the trinket is found in)
    public int Level;
    // Trinket buffs or debuffs these stats
    // A positive value buffs the stat, a negative value debuffs the stat
    public int MaxHealthBuff, AttackBuff, DefenseBuff;
    // The trinket is equipped by default
    public bool Equipped = true;
}
