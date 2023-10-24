//Holds enemy behaviour and stats for loading into Entity objects
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Enemy")]
public class Enemy : ScriptableObject
{
    public string title; //Name of enemy

    //Stats
    public int offense;
    public int defense;
    public int health;

    public List<Card> Actions; //List of cards the enemy can execute

}
