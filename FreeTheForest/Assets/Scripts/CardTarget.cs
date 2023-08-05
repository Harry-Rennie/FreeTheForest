//Component goes on targetable entities, ie Enemies, tells manager that it has been selected as the target

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CardTarget : MonoBehaviour
{
    BattleManager battle;
    Entity enemy; //Self

    private void Awake()
    {
        battle = FindObjectOfType<BattleManager>();
        enemy = GetComponent<Entity>();
    }

    //Linked to event system; If an attack card is currently selected, will tell BattleManager that
    //this linked Entity is the current target for attacking purposes.
    public void PointerEnter()
    {
        if (enemy==null) //Sanity check
        {
            Debug.Log("Fighter Null");
            battle = FindObjectOfType<BattleManager>();
            enemy = GetComponent<Entity>();
        }

        if (battle.selectedCard != null && battle.selectedCard.card.cardType == Card.CardType.Attack) //If Card Selected and The Card Is An Attack...
        {
            battle.cardTarget = enemy;
            Debug.Log("Targeting... " + enemy);
        }
    }

    //Reset BattleManager target on MouseOut
    public void PointerExit()
    {
        battle.cardTarget = null;
    }
}
