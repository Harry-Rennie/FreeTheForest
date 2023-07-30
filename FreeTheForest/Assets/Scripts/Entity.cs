using System.Collections;
using System.Collections.Generic;
using UnityEditor.Build.Content;
using UnityEngine;

public class Entity : MonoBehaviour
{
    public int currentHealth;
    public int maxHealth;
    public int currentBlock;

    public int offense;
    public int defense;

    public bool isPlayer;
    BattleManager battleManager;
    GameManager gameManager;

    private void Awake()
    {
        battleManager = FindObjectOfType<BattleManager>();
        currentHealth = maxHealth;
    }

    public void TakeDamage(int amount)
    {
        if (currentBlock > 0)
        {
            amount = BlockDamage(amount);
        }

        Debug.Log($"Dealt {amount} damage");

        currentHealth -= amount;

        if(currentHealth<=0)
        {
            Destroy(gameObject);
        }
    }

    public void AddBlock(int amount)
    {
        currentBlock += amount;
    }

    private int BlockDamage(int amount)
    {
        if (currentBlock>=amount) 
        {
            currentBlock -= amount;
            amount = 0;
        }
        else
        {
            amount -= currentBlock;
            currentBlock = 0;
        }

        return amount;
    }

}
