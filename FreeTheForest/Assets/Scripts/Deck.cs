/// <summary>
/// This class represents a Deck of cards and the methods to manipulate it.
/// </summary>
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Deck : MonoBehaviour
{
    public List<Card> MainDeck { get; } = new List<Card>();
    public List<Card> DiscardPile { get; } = new List<Card>();
    public List<Card> ExiledPile { get; } = new List<Card>();

    /// <summary>
    /// Adds a card to the Deck.
    /// </summary>
    /// <param name="card">The card to add to the Deck.</param>
    public void AddCard(Card card) => MainDeck.Add(card);

    /// <summary>
    /// Removes a card from the Deck.
    /// </summary>
    /// <param name="card">The card to remove from the Deck.</param>
    public void RemoveCard(Card card) => MainDeck.Remove(card);

    /// <summary>
    /// Adds a card to the exiled pile.
    /// </summary>
    /// <param name="card">The card to add to the exiled pile.</param>
    public void ExileCard(Card card) => ExiledPile.Add(card);

    /// <summary>
    /// Add a card to the discard pile.
    /// </summary>
    /// <param name="card">The card to add to the discard pile.</param>
    public void Discard(Card card) => DiscardPile.Add(card);

    /// <summary>
    /// Draws a single card from the Deck and returns it.
    /// </summary>
    /// <returns>The drawn card from the Deck, or null if both Deck and discard pile are empty.</returns>
    public Card DrawCard()
    {
        // check if Deck is empty
        if (MainDeck.Count == 0)
        {
            if (DiscardPile.Count == 0)
            {
                // Both Deck and discard pile are empty, return null
                return null;
            }

            // Reshuffle the discard pile into the Deck
            MainDeck.AddRange(DiscardPile);
            DiscardPile.Clear();
            Shuffle();
        }

        Card card = MainDeck[0];
        MainDeck.RemoveAt(0);
        return card;
    }


    /// <summary>
    /// Draws multiple cards from the Deck and returns them in a list.
    /// </summary>
    /// <param name="numCards">The number of cards to draw from the Deck.</param>
    /// <returns>A list of drawn cards from the Deck.</returns>
    public List<Card> DrawCards(int numCards)
    {
        List<Card> cards = new List<Card>();

        // Check if numCards is greater than Deck.Count
        if (numCards > MainDeck.Count)
        {
            // If so, add all cards from Deck to cards list
            numCards -= MainDeck.Count;
            cards.AddRange(MainDeck);
            MainDeck.Clear();
            // Add all cards from DiscardPile to Deck and shuffle
            MainDeck.AddRange(DiscardPile);
            DiscardPile.Clear();
            Shuffle();
        }
        // Check that Deck is not empty
        if (MainDeck.Count > 0)
        {
            // Add numCards from Deck to cards list and remove them from Deck
            cards.AddRange(MainDeck.GetRange(0, numCards));
            MainDeck.RemoveRange(0, numCards);
        }
        return cards;
    }

    /// <summary>
    /// Shuffles the Deck using Fisher-Yates algorithm.
    /// </summary>
    public void Shuffle()
    {
        for (int i = 0; i < MainDeck.Count; i++)
        {
            Card temp = MainDeck[i];
            int randomIndex = Random.Range(i, MainDeck.Count);
            MainDeck[i] = MainDeck[randomIndex];
            MainDeck[randomIndex] = temp;
        }
    }

    /// <summary>
    /// Reset the Deck by adding all cards from the discard and exiled piles to the Deck and shuffling.
    /// </summary>  
    public void Reset()
    {
        MainDeck.AddRange(DiscardPile);
        MainDeck.AddRange(ExiledPile);
        DiscardPile.Clear();
        ExiledPile.Clear();
        Shuffle();
    }
}
