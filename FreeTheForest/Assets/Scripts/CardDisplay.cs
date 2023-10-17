using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class CardDisplay : MonoBehaviour
{
    [Header("Card")]
    public Card card;

    [Header("Card Data")]
    public TextMeshProUGUI nameText;
    public TextMeshProUGUI descriptionText;
    public TextMeshProUGUI manaCost;

    private bool isSelected = false;
    private Vector3 originalPosition;
    private Transform parentSlot;
    private Transform battleCanvas;
    private Transform targetSlot; //reference to the target slot
    private Transform originalCardSlot; //reference to the original card slot

    private void Awake()
    {
        originalPosition = transform.position;
        parentSlot = transform.parent;
        originalCardSlot = parentSlot; //store the original card slot
        battleCanvas = GameObject.Find("BattleCanvas").transform;
        targetSlot = GameObject.Find("TargetSlot").transform; //assign the target slot in the Inspector
    }

    // Load card blank TMPro text objects with the data from the Card scriptable object.
    public void LoadCard(Card _card)
    {
        card = _card;
        nameText.text = card.title;
        descriptionText.text = card.description;
        manaCost.text = card.manaCost.ToString();
    }

    public void SelectCard()
    {
        if(targetSlot.childCount > 0)
        {
            return;
        }
        if (!isSelected)
        {
            CardDisplay previouslySelectedCard = GetSelectedCard();

            if (previouslySelectedCard != null)
            {
                // Deselect the previously selected card and return it to its original card slot
                previouslySelectedCard.DeselectCard();
            }

            isSelected = true;
            originalPosition = transform.position;
            parentSlot = transform.parent;

            //move the card to the target slot
            transform.SetParent(targetSlot);
            transform.position = targetSlot.position;
            //scale the card up when it is in the targeting slot
            transform.localScale = new Vector3(1.2f, 1.2f, transform.localScale.z);
        }
    }

    private CardDisplay GetSelectedCard()
    {
        CardDisplay[] allCards = FindObjectsOfType<CardDisplay>();
        foreach (CardDisplay cardDisplay in allCards)
        {
            if (cardDisplay.isSelected)
            {
                return cardDisplay;
            }
        }
        return null;
    }

    //checks if card is already in target slot, also returns card to hand
    public void DeselectCard()
    {
        if (isSelected)
        {
            if (Input.GetMouseButtonUp(1) && isSelected)
            {
                isSelected = false;
                transform.position = originalPosition;
                transform.SetParent(originalCardSlot);
                transform.localScale = new Vector3(0.8f, 0.8f, transform.localScale.z);
            }
            if(targetSlot.childCount > 1)
            {
                CardDisplay cardToReturn = targetSlot.GetChild(0).GetComponent<CardDisplay>();
                cardToReturn.transform.position = cardToReturn.originalPosition;
                cardToReturn.transform.SetParent(cardToReturn.originalCardSlot);
            }
        }
    }
}