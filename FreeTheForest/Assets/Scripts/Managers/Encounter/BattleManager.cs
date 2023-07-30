using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System.Diagnostics;
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
    }

    public void StartBattle()
    {
        deck = new Deck();
        cardsInHand = new List<Card>();
        deck.DiscardPile.AddRange(gameManager.playerDeck);
        deck.Shuffle();
        DrawCards(drawAmount);
        energy = maxEnergy;
    }

    public void DrawCards(int amount)
    {
        int cardsDrawn = 0;
        
        while(cardsDrawn<amount && cardsInHand.Count<=10) 
        {
            if (deck.MainDeck.Count < 1)
            {
                deck.Shuffle();
            }

            Card cardDrawn = deck.DrawCard();
            cardsInHand.Add(cardDrawn);
            DisplayCardInHand(cardDrawn);
            cardsDrawn++;
        }
    }

    public void DisplayCardInHand(Card card)
    {
        CardDisplay cardDis = handCardObjects[cardsInHand.Count-1];
        cardDis.LoadCard(card);
        cardDis.gameObject.SetActive(true);
    }

    public void PlayCard(CardDisplay card)
    {
        cardActions.PerformAction(card.card, cardTarget);

        energy -= card.card.manaCost;

        selectedCard = null;
        card.gameObject.SetActive(false);
        cardsInHand.Remove(card.card);
        deck.Discard(card.card);
    }
}