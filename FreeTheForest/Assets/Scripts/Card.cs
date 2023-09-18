/* This class represents a card in the deck. */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Card", menuName = "Card")]
public class Card : ScriptableObject
{
    public string title; //Card Name
    public string description; //Card Text

    public Sprite image; //Card image, currently unused, will need a mask on the card blank for this

    public int manaCost; //How much the card costs to cast
    public CardType cardType; //What kind of card it is
    public enum CardType { Attack, Skill}
    //public CardTargetType cardTargetType; //What does the card target
    //public enum CardTargetType { self, enemy}
    public enum CardEffect { Attack, Block, Energy, Draw, BuffSelf}
    public List<CardEffect> effects; //What does the card do? Read for CardActions
    public List<int> values; //Values for the given related effect
}