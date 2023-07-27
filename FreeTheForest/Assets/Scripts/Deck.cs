/* This class contains the deck of deck and the methods to manipulate it. */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class Deck : MonoBehaviour
{
    public List<Card> deck;
    public List<Card> discardPile;
    public List<Card> exiledPile;

    private Random random = UnityEngine.Random;

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

    /// <summary>
    /// Draws a single card from the deck and returns it.
    /// </summary>
    public Card DrawCard()
    {
        Card card = deck[0];
        deck.RemoveAt(0);
        return card;
    }

    public List<Card> DrawCards(int numCards)
    {
        List<Card> cards = new List<Card>();
        // If there are not enough cards in the deck:
        if (numCards > deck.Count)
        {
            // Add remaining cards in deck to cards to return
            numCards -= deck.Count;
            for (int i = 0; i < deck.Count; i++)
            {
                cards.Add(deck[0]);
                deck.RemoveAt(0);
            }
            // Then shuffle discard pile into deck 
            cards.AddRange(discardPile);
            discardPile.Clear();
            Shuffle();
            // Then draw remaining cards
            for (int i = 0; i < numCards; i++)
            {
                cards.Add(deck[0]);
                deck.RemoveAt(0);
            }
        }

        for (int i = 0; i < numCards; i++)
        {
            cards.Add(deck[0]);
            deck.RemoveAt(0);
        }
        
        return cards;
    }

    public void Shuffle()
    {
        for (int i = 0; i < deck.Count; i++)
        {
            Card temp = deck[i];
            int randomIndex = random.Range(i, deck.Count);
            deck[i] = deck[randomIndex];
            deck[randomIndex] = temp;
        }
    }
}