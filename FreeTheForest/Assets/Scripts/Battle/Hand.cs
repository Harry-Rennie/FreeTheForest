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
         AdjustInactiveCards();
         ResetCardLayout();
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

    if(prevCount != _heldCards.Count)
    {
        hasChanged = true;
        prevCount = _heldCards.Count;
    }

    if (hasChanged)
    {
        //udate the list of held cards and adjust card positions
        UpdateHeldCards();
        AdjustCardPositions();
        AdjustInactiveCards();
        ResetCardLayout();
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
    // List to track the occupied status of each slot.
    bool[] slotOccupied = new bool[cardSlots.Count];
    for (int i = 0; i < slotOccupied.Length; i++)
    {
        slotOccupied[i] = false; // Initially, no slots are occupied.
    }

    // Determine the number of active cards.
    int activeCardsCount = _heldCards.Count(card => card.gameObject.activeSelf);

    // Calculate the starting point for card positioning.
    int totalSlots = cardSlots.Count;
    int middleIndex = totalSlots / 2; // Assuming middle slot index for even totalSlots.
    
    // Cards expand from the middle to the sides.
    int startSlotIndex = middleIndex - (activeCardsCount / 2);
    if (activeCardsCount % 2 == 0)
    {
        startSlotIndex++; // Adjust if an even number of active cards to keep them centered.
    }

    // Position the cards.
    int placedCards = 0;
    foreach (var card in _heldCards)
    {
        if (card.gameObject.activeSelf) // Only position active cards.
        {
            int currentSlotIndex = startSlotIndex + placedCards;

            // Find the next unoccupied slot if necessary.
            while (currentSlotIndex < totalSlots && slotOccupied[currentSlotIndex])
            {
                currentSlotIndex++; // This slot is occupied, check the next one.
            }

            if (currentSlotIndex < totalSlots)
            {
                // Move the card to the correct slot.
                card.transform.SetParent(cardSlots[currentSlotIndex].transform, false);
                card.transform.localPosition = Vector3.zero; // Center in the slot.
                placedCards++;

                slotOccupied[currentSlotIndex] = true; // Mark this slot as occupied.
            }
            else
            {
                Debug.LogWarning("No empty slots available. Cannot place card: " + card.card.title);
                // You might want to handle this case specifically.
            }
        }
    }
}

//clean up function, some edge cases card positions can get out of whack.
public void ResetCardLayout()
{
    int layerOrder = 1;
    cardSlots.OrderBy(slot => slot.transform.position.x);
    foreach (var slot in cardSlots)
    {
        slot.GetComponent<Canvas>().sortingOrder = layerOrder;
        layerOrder++;
        CardDisplay card = slot.GetComponentInChildren<CardDisplay>();
        if (card != null)
        {
            card.transform.localScale = new Vector3(0.8f, 0.8f, card.transform.localScale.z);
            card.transform.localPosition = Vector3.zero;
            card.transform.position = slot.transform.position;
            card.GetComponent<Canvas>().sortingOrder = layerOrder;
        }
    }
    layerOrder = 1;
}
}
