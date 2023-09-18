//Basic stats for an entity on the battlefield

using System.Collections;
using System.Collections.Generic;
using UnityEditor.Build.Content;
using UnityEngine;

public class Entity : MonoBehaviour
{
    [Header("Health")]
    public int currentHealth;
    public int maxHealth;
    public int currentBlock;

    [Header("Stats")]
    public int offense;
    public int defense;

    [Header("Buffs")]
    public List<BuffBase> buffs;

    [Header("Player Check")]
    public bool isPlayer;

    [Header("Enemy Actions")]
    public List<Card> enemyCards;

    public BattleManager battleManager;
    PlayerInfoController gameManager;

    private void Awake()
    {
        battleManager = FindObjectOfType<BattleManager>();
        gameManager = FindObjectOfType<PlayerInfoController>();
        // assign stats from PlayerInfoController
        if (isPlayer)
        {
            maxHealth = gameManager.MaxHealth;
            currentHealth = gameManager.CurrentHealth;
            offense = gameManager.Strength;
            defense = gameManager.Defense;
        }
        else
        {
        currentHealth = maxHealth;
        }
        buffs= new List<BuffBase>();
    }


    public void TakeDamage(int amount)
    {
        if (currentBlock > 0) //Calculate blocked damage, if any.
        {
            amount = BlockDamage(amount);
        }

        Debug.Log($"Dealt {amount} damage");

        currentHealth -= amount; //Take damage

        if(currentHealth<=0) //Die
        {
            Destroy(gameObject);
        }
    }

    public void AddBlock(int amount) //Flat add to Entity block
    {
        currentBlock += amount;
    }

    private int BlockDamage(int amount) //Reduce damage by block amount
    {
        if (currentBlock>=amount) //All damage blocked
        {
            currentBlock -= amount;
            amount = 0;
        }
        else                     //Partial Block
        {
            amount -= currentBlock;
            currentBlock = 0;
        }

        return amount;
    }

    public void AddBuff(BuffBase buff)
    {
        Debug.Log(buff);
        if (buffs.Contains(buff))
        {
            buffs[buffs.IndexOf(buff)].Activate();
        }
        else
        {
            buffs.Add(buff);
            buff.Activate();
        }
    }

    public void CleanseBuffs()
    {
        foreach (BuffBase buff in buffs)
        {
            buff.End();
            buffs.Remove(buff);
        }
    }

}
