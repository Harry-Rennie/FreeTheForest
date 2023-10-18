using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class CardTarget : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
{
    BattleManager battle;
    Entity enemy; // Self

    private void Awake()
    {
        battle = FindObjectOfType<BattleManager>();
        enemy = GetComponent<Entity>();
    }

    // Implement the OnPointerEnter method from IPointerEnterHandler
    public void OnPointerEnter(PointerEventData eventData)
    {
        if (enemy == null) // Sanity check
        {
            Debug.Log("Fighter Null");
            battle = FindObjectOfType<BattleManager>();
            enemy = GetComponent<Entity>();
        }

        // Additional logic when the pointer enters the object area
        if (battle.selectedCard != null && battle.selectedCard.card.cardType == Card.CardType.Attack) // If Card Selected and The Card Is An Attack...
        {
            battle.cardTarget = enemy;
            Debug.Log("Targeting... " + enemy);
        }
    }

    // Implement the OnPointerClick method from IPointerClickHandler
    public void OnPointerClick(PointerEventData eventData)
    {
        //check to see if battle is over and is a reward card
        if(battle.battleOver == true && battle.RewardPanel.activeSelf == true)
        {
            battle.AddCard(battle.selectedCard.card);
        }
        // Logic to execute when the object is clicked.
        if (battle.selectedCard != null && battle.selectedCard.card.cardType == Card.CardType.Attack)
        {
            battle.cardTarget = enemy;
            battle.PlayCard(battle.selectedCard);
        }
    }
    public void OnPointerExit(PointerEventData eventData)
    {
        // Logic to execute when the pointer exits the object area.
        battle.cardTarget = null;
    }

    public void AddRewardCard(PointerEventData eventData)
    {
        battle.AddCard(battle.selectedCard.card);
    }
}