//Main brains of the Battle System. Controls the Player Deck and Hand, tracks targeted entites, and delegates playing cards

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System.Xml;
using UnityEditor.Build.Content;

public class BattleManager : MonoBehaviour
{
    [Header("Cards")]
    public Deck deck = new Deck();
    public List<Card> cardsInHand = new List<Card>();
    public CardDisplay selectedCard;
    public List<CardDisplay> handCardObjects;

    [Header("Stats")]
    public Entity cardTarget;
    public Entity player;
    public int maxEnergy;
    public int energy;
    public int drawAmount = 5;

    [Header("Enemies")]
    public List<Entity> enemies = new List<Entity>();
    CardActions cardActions;
    PlayerInfoController gameManager;

    public void Awake()
    {
        gameManager = FindObjectOfType<PlayerInfoController>();
        cardActions = GetComponent<CardActions>();

        StartBattle();
    }

    //Function initializes the battle state, loading in the deck from GameManager and drawing the opening hand.
    public void StartBattle()
    {
        deck = new Deck();
        cardsInHand = new List<Card>();

        deck.DiscardPile.AddRange(gameManager.playerDeck); //Add cards from GM to discard pile of new empty deck

        deck.Shuffle(); //Shuffle the added cards into the main deck
        DrawCards(drawAmount);
        energy = maxEnergy;
    }

    //Draw cards. Loop over Deck card draw X times. Load returned card into hand.
    public void DrawCards(int amount)
    {
        int cardsDrawn = 0;
        
        while(cardsDrawn<amount && cardsInHand.Count<=10) //While we have cards to draw AND our hand is not full...
        {
            Card cardDrawn = deck.DrawCard(); //Get the card from the deck
            cardsInHand.Add(cardDrawn); //Put the card in the Hand list
            DisplayCardInHand(cardDrawn); //Display the card
            cardsDrawn++;
        }
    }

    //Load CardDisplay game object with given Card data and make visible.
    public void DisplayCardInHand(Card card)
    {
        CardDisplay cardDis = handCardObjects[cardsInHand.Count-1]; //Get the next available card
        cardDis.LoadCard(card); //Tell card to load the Card data
        cardDis.gameObject.SetActive(true); //Activate the gameObject.
    }

    //Play a given card
    public void PlayCard(CardDisplay card)
    {
        cardActions.PerformAction(card.card, cardTarget); //Tell CardActions to perform action based on Card name and Target if necessary

        energy -= card.card.manaCost; //Reduce energy by card cost (CardActions checks for enough mana)

        selectedCard = null; //Drop the referenced card
        card.gameObject.SetActive(false); //Deactivate the gameObject
        cardsInHand.Remove(card.card); //Remove the Hand list item
        deck.Discard(card.card); //Put the card into the Discard pile of the Deck object.
    }
}