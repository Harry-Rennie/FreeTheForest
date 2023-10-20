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

    public bool isSelected;
    public bool deselect = false;
    private Vector3 originalPosition;
    private Transform parentSlot;
    private Transform battleCanvas;
    private Transform targetSlot; //reference to the target slot
    private Transform originalCardSlot; //reference to the original card slot
    private BattleManager battleManager;
    private int sortingOrder;

    private void Awake()
    {
        originalPosition = transform.position;
        parentSlot = transform.parent;
        originalCardSlot = parentSlot; //store the original card slot
        battleCanvas = GameObject.Find("BattleCanvas").transform;
        targetSlot = GameObject.Find("TargetSlot").transform; //assign the target slot in the Inspector
    }

    private void Start()
    {
        battleManager = FindObjectOfType<BattleManager>();
        battleManager.OnClearTargeting += HandleClearTargeting;
    }

    private void HandleClearTargeting()
    {
        deselect = true;
        DeselectCard();
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
        if(Input.GetMouseButtonDown(1))
        {
            return;
        }
        if(battleManager.battleOver && battleManager.RewardPanel.activeSelf)
        {
                battleManager.AddCard(card);
        }
        if(targetSlot.childCount > 0 || isSelected)
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
            deselect = false;
            isSelected = true;
            originalPosition = transform.position;
            sortingOrder= transform.parent.gameObject.GetComponent<Canvas>().sortingOrder;
            parentSlot = transform.parent;
            //move the card to the target slot
            transform.SetParent(targetSlot);
            transform.position = targetSlot.position;
            //scale the card up when it is in the targeting slot
            transform.localScale = new Vector3(1f, 1f, transform.localScale.z);
        }
    }

    public CardDisplay GetSelectedCard()
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
            if (deselect && isSelected)
            {
                isSelected = false;
                transform.position = originalPosition;
                transform.SetParent(originalCardSlot);
                transform.localScale = new Vector3(0.8f, 0.8f, transform.localScale.z);
                originalCardSlot.GetComponent<Canvas>().sortingOrder = sortingOrder - 100;
            }
            if(targetSlot.childCount > 1)
            {
                CardDisplay cardToReturn = targetSlot.GetChild(0).GetComponent<CardDisplay>();
                cardToReturn.transform.position = cardToReturn.originalPosition;
                cardToReturn.transform.SetParent(cardToReturn.originalCardSlot);
                originalCardSlot.GetComponent<Canvas>().sortingOrder = sortingOrder - 100;
            }
        }
    }
}