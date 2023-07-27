/// <summary>
/// This class contains the deck of cards and the methods to manipulate it.
/// </summary>
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Deck : MonoBehaviour
{
    private List<Card> deck;
    private List<Card> discardPile;
    private List<Card> exiledPile;
    private Random random = UnityEngine.Random;

    /// <summary>
    /// Constructor to initialize the deck list when a new Deck instance is created.
    /// </summary>
    public Deck()
    {
        deck = new List<Card>();
    }

    /// <summary>
    /// Adds a card to the deck.
    /// </summary>
    /// <param name="card">The card to add to the deck.</param>
    public void AddCard(Card card) => deck.Add(card);

    /// <summary>
    /// Removes a card from the deck.
    /// </summary>
    /// <param name="card">The card to remove from the deck.</param>
    public void RemoveCard(Card card) => deck.Remove(card);

    /// <summary>
    /// Adds a card to the exiled pile.
    /// </summary>
    /// <param name="card">The card to add to the exiled pile.</param>
    public void ExileCard(Card card) => exiledPile.Add(card);

    /// <summary>
    /// Add a card to the discard pile.
    /// </summary>
    /// <param name="card">The card to add to the discard pile.</param>
    public void Discard(Card card) => discardPile.Add(card);

    /// <summary>
    /// Draws a single card from the deck and returns it.
    /// </summary>
    /// <returns>The drawn card from the deck.</returns>
    public Card DrawCard()
    {
        Card card = deck[0];
        deck.RemoveAt(0);
        return card;
    }

    /// <summary>
    /// Draws multiple cards from the deck and returns them in a list.
    /// </summary>
    /// <param name="numCards">The number of cards to draw from the deck.</param>
    /// <returns>A list of drawn cards from the deck.</returns>
    public List<Card> DrawCards(int numCards)
    {
        List<Card> cards = new List<Card>();

        // check if numCards is greater than deck.Count
        if (numCards > deck.Count)
        {
            // if so, add all cards from deck to cards list
            numCards -= deck.Count;
            cards.AddRange(deck);
            deck.Clear();
            // add all cards from discardPile to deck
            deck.AddRange(discardPile);
            discardPile.Clear();
            Shuffle();
        }
        // add numCards from deck to cards list and remove them from deck
        cards.AddRange(deck.GetRange(0, numCards));
        deck.RemoveRange(0, numCards);

        return cards;
    }

    /// <summary>
    /// Shuffles the deck using Fisher-Yates algorithm.
    /// </summary>
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

    /// <summary>
    /// Reset the deck by adding all cards from the discard and exiled piles to the deck and shuffling.
    /// </summary>  
    public void Reset()
    {
        deck.AddRange(discardPile);
        deck.AddRange(exiledPile);
        discardPile.Clear();
        exiledPile.Clear();
        Shuffle();
    }
}
