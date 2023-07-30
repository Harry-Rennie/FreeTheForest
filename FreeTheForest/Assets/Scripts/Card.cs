/* This class represents a card in the deck. */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Card", menuName = "Card")]
public class Card : ScriptableObject
{
    public string title;
    public string description;

    public Sprite image;

    public int manaCost;
    public CardType cardType;
    public enum CardType { Attack, Skill}
    public CardTargetType cardTargetType;
    public enum CardTargetType { self, enemy}
}