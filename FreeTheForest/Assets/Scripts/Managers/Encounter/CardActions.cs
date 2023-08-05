//Performs card actions as told by the Battle Manager

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CardActions : MonoBehaviour
{
    Card card;
    public Entity target;
    public Entity player;
    BattleManager battleManager;

    private void Awake()
    {
        battleManager = GetComponent<BattleManager>();
    }

    //This method reads the title of a given card and executes the appropriate action via 
    //switch statement. BattleManager calls this once player has played a card.
    public void PerformAction(Card _card, Entity _entity)
    {
        card = _card;
        target = _entity;

        switch (card.title)
        {
            case "Strike":
                AttackEnemy();
                break;
            case "Block":
                PerformBlock();
                break;
            default:
                Debug.Log("Something gone wrong with Performing Action");
                break;
        }
    }

    private void AttackEnemy() //Deal damage to current target equal to Player Offense stat
    {
        int damage = player.offense;
        
        target.TakeDamage(damage);
    }

    private void PerformBlock() //Player gains block equal to their Defense stat
    {
        int block = player.defense;

        player.AddBlock(block);
    }
}
