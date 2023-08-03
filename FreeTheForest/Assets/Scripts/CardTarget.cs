using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CardTarget : MonoBehaviour
{
    BattleManager battle;
    Entity enemy;

    private void Awake()
    {
        battle = FindObjectOfType<BattleManager>();
        enemy = GetComponent<Entity>();
    }

    public void PointerEnter()
    {
        if (enemy==null)
        {
            Debug.Log("Fighter Null");
            battle = FindObjectOfType<BattleManager>();
            enemy = GetComponent<Entity>();
        }

        if (battle.selectedCard != null && battle.selectedCard.card.cardType == Card.CardType.Attack)
        {
            battle.cardTarget = enemy;
            Debug.Log("Targeting...");
        }
    }

    public void PointerExit()
    {
        battle.cardTarget = null;
    }
}
