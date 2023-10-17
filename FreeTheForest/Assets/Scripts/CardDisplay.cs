//Script for fetching Card data and filling out a blank CardDisplay object with the information

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

    BattleManager battleManager;

    private void Awake()
    {
        battleManager = FindObjectOfType<BattleManager>();
    }

    //Load card blank TMPro text objects with the data from the Card scriptable object.
    public void LoadCard(Card _card)
    {
        card = _card;
        nameText.text = card.title;
        descriptionText.text = card.description;
        manaCost.text = card.manaCost.ToString();
    }

    //Tell manager that this is the current card
    public void SelectCard()
    {
        battleManager.selectedCard = this;
        Debug.Log("Selected Card: " + card.title);
    }

    //Tell manager this card is no longer selected OR Choose the card for reward if battle is over.
    public void DeselectCard()
    {
        if (battleManager.EnemyCount > 0) 
        { 
            battleManager.selectedCard = null; 
        }
        else
        {
            battleManager.AddCard(this.card);
        }
    }

}
