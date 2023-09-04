//Basic stats for an entity on the battlefield

using System.Collections;
using System.Collections.Generic;
//using System.Diagnostics;
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

    [Header("Player Check")]
    public bool isPlayer;

    [Header("Enemy Actions")]
    public List<Card> enemyCards;

    BattleManager battleManager;


    private void Awake()
    {
        battleManager = FindObjectOfType<BattleManager>();
         //TODO: IF PLAYER get player health from GAMEMANAGER
        if (isPlayer)
        {
            currentHealth = PlayerInfoController.Instance.PlayerHealth;
            Debug.Log("Player Health: " + currentHealth);
        }
        else
        {
            currentHealth = maxHealth;
        }
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

}
