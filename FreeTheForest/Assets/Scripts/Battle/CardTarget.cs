using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class CardTarget : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
{
    BattleManager battle;
    Entity enemy; // Self
    GameObject valid;
    GameObject invalid;
    private void Awake()
    {
        battle = FindObjectOfType<BattleManager>();
        enemy = GetComponent<Entity>();
        valid = transform.GetChild(1).gameObject;
        invalid = transform.GetChild(2).gameObject;
    }

    // Implement the OnPointerEnter method from IPointerEnterHandler
    public void OnPointerEnter(PointerEventData eventData)
    {
        if(battle.selectedCard != null && battle.selectedCard.card.cardType != Card.CardType.Attack) //then target self
        {
            if(enemy.isPlayer)
            {
                battle.cardTarget = enemy;
                valid.SetActive(true);
            }
        }
        if(battle.selectedCard != null && battle.selectedCard.card.cardType == Card.CardType.Attack) //then target enemy
        {
            if(enemy.isPlayer)
            {
                invalid.SetActive(true);
            }
        }
        // Additional logic when the pointer enters the object area
        if (battle.selectedCard != null && battle.selectedCard.card.cardType == Card.CardType.Attack) // If Card Selected and The Card Is An Attack...
        {
            if(enemy.isPlayer)
            {
                invalid.SetActive(true);
            }
            if(!enemy.isPlayer)
            {
                battle.cardTarget = enemy;
                valid.SetActive(true);
            }
        }
    }

    // Implement the OnPointerClick method from IPointerClickHandler
    public void OnPointerClick(PointerEventData eventData)
    {
        if(battle.selectedCard != null)
        {
        valid.SetActive(false);
        invalid.SetActive(false);
        //check to see if battle is over and is a reward card
        if(battle.battleOver == true && battle.RewardPanel.activeSelf == true)
        {
            battle.AddCard(battle.selectedCard.card);
        }
        if(battle.selectedCard != null && battle.selectedCard.card.cardType != Card.CardType.Attack)
        {
            if(enemy.isPlayer)
            {
                battle.cardTarget = enemy;
                StartCoroutine(battle.PlayCard(battle.selectedCard));
            }
        }
        // Logic to execute when the object is clicked.
        if (battle.selectedCard != null && battle.selectedCard.card.cardType == Card.CardType.Attack)
        {
            if(!enemy.isPlayer)
            {
                battle.cardTarget = enemy;
                StartCoroutine(battle.PlayCard(battle.selectedCard));
            }
        }
        }
    }
    public void OnPointerExit(PointerEventData eventData)
    {
        // Logic to execute when the pointer exits the object area.
        battle.cardTarget = null;
        valid.SetActive(false);
        invalid.SetActive(false);
    }

    public void AddRewardCard(PointerEventData eventData)
    {
        battle.AddCard(battle.selectedCard.card);
    }
}