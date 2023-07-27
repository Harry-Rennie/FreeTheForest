/* This class contains the deck of deck and the methods to manipulate it. */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Deck : MonoBehaviour
{
    public List<Card> deck;

    public Deck()
    {
        deck = new List<Card>();
    }

    public void AddCard(Card card)
    {
        deck.Add(card);
    }

    public void RemoveCard(Card card)
    {
        deck.Remove(card);
    }

    public Card drawCard()
    {
        Card card = deck[0];
        deck.RemoveAt(0);
        return card;
    }

    public void Shuffle()
    {
        for (int i = 0; i < deck.Count; i++)
        {
            Card temp = deck[i];
            int randomIndex = Random.Range(i, deck.Count);
            deck[i] = deck[randomIndex];
            deck[randomIndex] = temp;
        }
    }
}