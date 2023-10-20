using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;

public class Hand : MonoBehaviour
{
    public List<CardDisplay> _heldCards;
    private List<CardDisplay> previousSlotCards;
    private List<bool> previousSlotStates;
    //the parent canvas of each card within the 'Cards' canvas (the hand)
    [SerializeField] public List<GameObject> cardSlots = new List<GameObject>();
    private int prevCount;
    BattleManager battleManager;

    public List<CardDisplay> heldCards
    {
        get { return _heldCards; } //getter to access the cardsInHand list
        set
        {
            if (_heldCards.Count != value.Count)
            {
                AdjustCardPositions();
            }

            _heldCards = value; // Set the new value
        }
    }
    public void Start()
    {
        battleManager = FindObjectOfType<BattleManager>();
        prevCount = _heldCards.Count;
        previousSlotStates = new List<bool>();
        previousSlotCards = new List<CardDisplay>(new CardDisplay[cardSlots.Count]);
        for (int i = 0; i < cardSlots.Count; i++)
        {
            previousSlotStates.Add(cardSlots[i].GetComponentInChildren<CardDisplay>() != null);
             previousSlotCards[i] = cardSlots[i].GetComponentInChildren<CardDisplay>();
        }
         AdjustCardPositions();
    }

    void Update()
    {
        CheckCardSlots();
    }

void CheckCardSlots()
{
    bool hasChanged = false;

    for (int i = 0; i < cardSlots.Count; i++)
    {
        CardDisplay currentCard = cardSlots[i].GetComponentInChildren<CardDisplay>();

        //check if the card in this slot has changed
        if (currentCard != previousSlotCards[i])
        {
            //the card has changed which means a card has left or entered the slot
            hasChanged = true;

            //update state
            previousSlotCards[i] = currentCard;
        }
    }

    if (hasChanged)
    {
        //udate the list of held cards and adjust card positions
        UpdateHeldCards();
        AdjustCardPositions();
        AdjustInactiveCards();
    }
}
    void AdjustInactiveCards()
    {
        List <GameObject> emptySlots = cardSlots.Where(slot => slot.transform.childCount == 0).ToList();
        List <GameObject> dupeSlots = cardSlots.Where(slot => slot.transform.childCount > 1).ToList();
        List<CardDisplay> children = new List<CardDisplay>();
        foreach (GameObject slot in dupeSlots)
        {
            //get all of the children from the slot
            foreach (Transform child in slot.transform)
            {
                CardDisplay card = child.GetComponent<CardDisplay>();
                if (!card.gameObject.activeSelf)
                {
                    children.Add(card);
                }
            }
        }
        foreach (GameObject slot in emptySlots)
        {
            if (children.Count > 0) //check if there are still cards to move    
            {
                children[0].transform.SetParent(slot.transform, false);
                children.RemoveAt(0);
            }
            else
            {
                break;
            }
        }
        emptySlots.Clear();
        dupeSlots.Clear();
        children.Clear();
    }
    void UpdateHeldCards()
    {
        //refresh the list of held cards based on the current children of the card slots.
        _heldCards.Clear();
        foreach (var slot in cardSlots)
        {
            CardDisplay card = slot.GetComponentInChildren<CardDisplay>();
            if (card != null && card.gameObject.activeSelf)
            {
                _heldCards.Add(card);
            }
        }
    }

public void AdjustCardPositions()
{
    List<CardDisplay> inactiveCards = new List<CardDisplay>();
    List<GameObject> inactiveSlots = new List<GameObject>();
    //how many active cards are in the hand
    int activeCardsCount = _heldCards.Count(card => card.gameObject.activeSelf);

    //calculate starting point for the cards to be positioned.
    int totalSlots = cardSlots.Count;
    int middleIndex = totalSlots / 2; //slot 5 - index 4

    //cards expand from the middle to the sides.
    int startSlotIndex = middleIndex - (activeCardsCount / 2);
    if (activeCardsCount % 2 == 0)
    {
        startSlotIndex++; //adjust if even number of active cards to keep them centered
    }

    //start positioning the cards from left to right based on their current active state
    int placedCards = 0; //counter for number of cards positioned already
    foreach (var card in _heldCards)
    {
        if (card.gameObject.activeSelf) //only position active cards.
        {
            int currentSlotIndex = startSlotIndex + placedCards; //calculate current slot for this card.
            
            if (currentSlotIndex >= 0 && currentSlotIndex < totalSlots)
            {
                //move the card to the correct slot
                card.transform.SetParent(cardSlots[currentSlotIndex].transform, false);
                card.transform.localPosition = Vector3.zero; //center in the slot
                placedCards++;
            }
        }
    }

    //for each cardslot, if there is an inactive card AND active card in the slot, put the inactive card in the nearest available empty cardslot.
}
}
