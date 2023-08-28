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

    //This method reads the effects of a given card and executes the appropriate action(s) via 
    //switch statement. BattleManager calls this once player has played a card.
    public void PerformAction(Card _card, Entity _entity)
    {
        card = _card;
        target = _entity;

        if (card.effects != null) //Check that we have a card that does stuff
        {
            for (int i = 0; i < card.effects.Count; i++) //Loop through the effects
            {
                switch (card.effects[i])
                {
                    case Card.CardEffect.Attack:
                        AttackEnemy(card.values[i]); //Call the relevant effect with the current "values" as argument
                        break;
                    case Card.CardEffect.Block:
                        PerformBlock(card.values[i]);
                        break;
                    case Card.CardEffect.Energy:
                        ChangeEnergy(card.values[i]);
                        break;
                    default:
                        Debug.Log("Something gone wrong with Performing Action");
                        break;
                }
            }
        }
    }

    private void AttackEnemy(int mode) //Deal damage to current target equal to Player Offense stat
    {
        if (battleManager.playersTurn)
        {
            int damage = player.offense;

            target.TakeDamage(damage);
        }
        else
        {
            int damage = target.offense;

            player.TakeDamage(damage);
        }
    }

    private void ChangeEnergy(int change)
    {
        battleManager.energy += change;
    }

    private void PerformBlock(int mode) //Player gains block equal to their Defense stat
    {
        if (battleManager.playersTurn)
        {
            int block = player.defense;

            player.AddBlock(block);
        }
        else
        {
            int block = target.defense;

            target.AddBlock(block);
        }
    }
}
