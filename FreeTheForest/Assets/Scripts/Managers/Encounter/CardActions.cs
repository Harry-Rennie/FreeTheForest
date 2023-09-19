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
                    case Card.CardEffect.Draw:
                        DrawCards(card.values[i]);
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
        switch (mode)
        {
            case 0: //Default case, deal damage based on Offense
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
                break;
            case 1: //Weak attack, deals 30% less damage
                if (battleManager.playersTurn)
                {
                    int damage = Mathf.RoundToInt(player.offense * 0.7f);

                    target.TakeDamage(damage);
                }
                else
                {
                    int damage = Mathf.RoundToInt(target.offense * 0.7f);

                    player.TakeDamage(damage);
                }
                break;
            case 2: //Attack enhanced by Entity current block, usually to be followed by a Block Wipe
                if (battleManager.playersTurn)
                {
                    int damage = player.offense + (player.currentBlock * 2);

                    target.TakeDamage(damage);
                }
                else
                {
                    int damage = target.offense + (target.currentBlock * 2);

                    player.TakeDamage(damage);
                }
                break;
            case 9000: //DEBUG ATTACK MODE: Kill Target
                target.TakeDamage(9999);
                break;
            default:
                Debug.Log("Something gone wrong with Attack Mode");
                break;
        }
    }

    private void ChangeEnergy(int change)
    {
        battleManager.energy += change;
    }

    private void PerformBlock(int mode) //Entity gains block equal to their Defense stat
    {
        switch (mode)
        {
            case 0: //Default Case, Add block equal to defense
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
                break;
            case 1://Double entity block
                if (battleManager.playersTurn)
                {
                    int block = player.currentBlock;

                    player.AddBlock(block);
                }
                else
                {
                    int block = target.currentBlock;

                    target.AddBlock(block);
                }
                break;
            case -1: //Entity loses all block
                if (battleManager.playersTurn)
                {
                    player.AddBlock(-player.currentBlock);
                }
                else
                {
                    target.AddBlock(-target.currentBlock);
                }
                break;
            default:
                Debug.Log("Something gone wrong with Block Mode");
                break;
        }
    }

    private void DrawCards(int amount)
    {
        battleManager.DrawCards(amount);
    }
}
