//Performs card actions as told by the Battle Manager

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CardActions : MonoBehaviour
{
    Card card;
    [SerializeField] PlayerAnimator PlayerSprite;
    CardDisplayAnimator CardSprite;
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
        //Get the card display animator from the targetslot canvas
        if (card.effects != null) //Check that we have a card that does stuff
        {
            for (int i = 0; i < card.effects.Count; i++) //Loop through the effects
            {
                Debug.Log(card.effects[i]);
                switch (card.effects[i])
                {
                    case Card.CardEffect.Attack:
                        AttackEnemy(card.values[i]); //Call the relevant effect with the current "values" as argument
                        SFXManager.Instance.Play("Attack");
                        break;
                    case Card.CardEffect.Block:
                        PerformBlock(card.values[i]);
                        SFXManager.Instance.Play("Block");
                        break;
                    case Card.CardEffect.Energy:
                        ChangeEnergy(card.values[i]);
                        break;
                    case Card.CardEffect.EnergySpecial:
                        EnergySpecial(card.values[i]);
                        break;
                    case Card.CardEffect.DrawSpecial:
                        DrawSpecial(card.values[i]);
                        break;
                    case Card.CardEffect.Draw:
                        DrawCards(card.values[i]);
                        SFXManager.Instance.Play("DrawCard");
                        break;
                    case Card.CardEffect.PoisonSelf:
                        ApplySelfPoison(card.values[i]);
                        SFXManager.Instance.Play("Poison");
                        break;
                    case Card.CardEffect.GrowthSelf:
                        ApplySelfGrowth(card.values[i]);
                        break;
                    case Card.CardEffect.GrowthSpecial:
                        GrowthSpecial(card.values[i]);
                        break;
                    case Card.CardEffect.Stun:
                        ApplyStun(card.values[i]);
                        break;
                    case Card.CardEffect.AOE:
                        AOEAttack(card.values[i]);
                        break;
                    default:
                        Debug.Log("Something gone wrong with Performing Action");
                        break;
                }
            }
            PlayerInfoPanel.Instance.UpdateStats();
        }
    }

    private void ApplyStun(int mode)
    {
        switch (mode)
        {
            case 0: //Apply 1 stack of stun to enemy.
                StunBuff stun = new StunBuff();
                stun.target = target;
                break;
            default:
                break;
        }
    }

    private void ApplySelfPoison(int stacks)
    {
        PoisonBuff pbuff = new PoisonBuff();
        pbuff.target = player;
        for (int i = 0; i < stacks; i++)
        {
            player.AddBuff(pbuff);
        }
    }

    private void ApplySelfGrowth(int stacks)
    {
        GrowthBuff gbuff = new GrowthBuff();
        gbuff.target = player;
        for (int i = 0; i < stacks; i++)
        {
            player.AddBuff(gbuff);
        }
    }

    private void GrowthSpecial (int mode)
    {
        switch (mode)
        {
            case 0://Double growth
                BuffBase buff = player.GetBuff("Growth");
                ApplySelfGrowth(buff.stacks);
                break;
            case -1://Lose a stack
                foreach (BuffBase abuff in player.buffs)
                {
                    if (abuff.buffName == "Growth" && abuff.stacks > 0)
                    {
                        abuff.stacks--;
                        abuff.target.strength--;
                        abuff.target.defence--;
                        abuff.target.battleManager.energyGain--;
                    }
                }
                break;
            default:
                break;
        }
    }

    private void AttackEnemy(int mode) //Deal damage to current target equal to Player Offense stat
    {
        if(battleManager.playersTurn)
        {
            if(!target.isPlayer)
            {
                PlayerSprite.Attack();
            }
        }
        switch (mode)
        {
            case 0: //Default case, deal damage based on Offense
                if (battleManager.playersTurn)
                {
                    int damage = player.strength;
                    target.TakeDamage(damage);
                }
                else
                {
                    int damage = target.strength;
                    player.TakeDamage(damage);
                }
                break;
            case 1: //Weak attack, deals 30% less damage
                if (battleManager.playersTurn)
                {
                    int damage = Mathf.RoundToInt(player.strength * 0.7f);

                    target.TakeDamage(damage);
                }
                else
                {
                    int damage = Mathf.RoundToInt(target.strength * 0.7f);

                    player.TakeDamage(damage);
                }
                break;
            case 2: //Attack enhanced by Entity current block, usually to be followed by a Block Wipe
                if (battleManager.playersTurn)
                {
                    int damage = player.strength + (player.currentBlock * 2);

                    target.TakeDamage(damage);
                }
                else
                {
                    int damage = target.strength + (target.currentBlock * 2);

                    player.TakeDamage(damage);
                }
                break;
            case 3: //Strong Attack
                if (battleManager.playersTurn)
                {
                    int damage = Mathf.RoundToInt(player.strength * 1.3f);

                    target.TakeDamage(damage);
                }
                else
                {
                    int damage = Mathf.RoundToInt(target.strength * 1.3f);

                    player.TakeDamage(damage);
                }
                break;
            case 4: //Detonate Effect (damage = 4x current energy)
                if (battleManager.playersTurn)
                {
                    int damage = battleManager.energy * 4;

                    target.TakeDamage(damage);
                }
                else
                {
                    int damage = battleManager.energy * 4;

                    player.TakeDamage(damage);
                }
                break;
            case 5: //Sap effect
                target.TakeDamage(1);
                ChangeEnergy(2);
                break;
            case 6: //Attack for twice your growth. If kill, then also growth.
                int temp = battleManager.EnemyCount;
                target.TakeDamage(player.GetBuff("Growth").stacks);
                if (battleManager.EnemyCount != temp) //If enemy death number changed from this, then...
                {
                    ApplySelfGrowth(1);
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

    private void AOEAttack(int mode) //Perform the designated attack on all enemies in scene
    {   
        foreach (Entity enemy in battleManager.enemies)
        {
            if (enemy.gameObject.activeSelf)
            {
                target = enemy;

                switch (mode)
                {
                    case 0:
                        AttackEnemy(0);
                        break;
                    case 1:
                        AttackEnemy(1);
                        break;
                    case 2:
                        AttackEnemy(2);
                        break;
                    case 3:
                        AttackEnemy(3);
                        break;
                    case 4:
                        AttackEnemy(4);
                        break;
                    case 5:
                        AttackEnemy(5);
                        break;
                    case 6:
                        AttackEnemy(6);
                        break;
                    case 100: //TIMBER behaviour
                        AttackEnemy(0);
                        AttackEnemy(0);
                        AttackEnemy(0);
                        ApplyStun(0);
                        break;
                    default:
                        break;

                }
            }
        }
    }

    private void ChangeEnergy(int change)
    {
        battleManager.currentEnergy += change;
    }

    private void PerformBlock(int mode) //Entity gains block equal to their Defense stat
    {
        switch (mode)
        {
            case 0: //Default Case, Add block equal to defense
                if (battleManager.playersTurn)
                {
                    int block = player.defence;

                    player.AddBlock(block);
                }
                else
                {
                    int block = target.defence;

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
            case 2://Strong Block
                if (battleManager.playersTurn)
                {
                    int block = Mathf.RoundToInt(player.defence * 1.33f);

                    player.AddBlock(block);
                }
                else
                {
                    int block = Mathf.RoundToInt(target.defence * 1.33f);

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

    private void DrawSpecial(int mode)
    {
        switch (mode)
        {
            case 0: //Draw equal to growth
                BuffBase buff = player.GetBuff("Growth");
                DrawCards(buff.stacks);
                break;
            default:
                break;
        }
    }

    private void EnergySpecial(int mode)
    {
        switch (mode)
        {
            case 0: //Gain Energy equal to growth
                BuffBase buff = player.GetBuff("Growth");
                ChangeEnergy(buff.stacks);
                break;
            case -1: //Lose all
                battleManager.energy = 0;
                break;
            default:
                break;
        }
    }
}
