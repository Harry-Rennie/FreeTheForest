using System.Collections.Generic;
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
    public string Title;
    public string Description;
    public Sprite Sprite;
    // Trinket level (corresponds to the level of the dungeon that the trinket is found in)
    public int Level;
    // Trinket buffs or debuffs these stats
    // A positive value buffs the stat, a negative value debuffs the stat
    public int MaxHealthBuff, AttackBuff, DefenceBuff;
    public bool Equipped;

    // this method returns a string that describes the trinket
    public override string ToString()
    {
        string trinketString = $"{Title}\n\n<i>{Description}</i>\n\n";
        if (MaxHealthBuff != 0)
        {
            trinketString += $"Max Health: {MaxHealthBuff}\n";
        }
        if (AttackBuff != 0)
        {
            trinketString += $"Attack: {AttackBuff}\n";
        }
        if (DefenceBuff != 0)
        {
            trinketString += $"Defense: {DefenceBuff}\n";
        }
        return trinketString;
    }
}
