//Basic stats for an entity on the battlefield

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;

public class Entity : MonoBehaviour
{
    [Header("Health")]
    public int currentHealth;
    public int maxHealth;
    public int currentBlock;

    [Header("Stats")]
    public int strength;
    public int defence;

    [Header("Buffs")]
    public List<BuffBase> buffs;

    [Header("Player Check")]
    public bool isPlayer;

    [Header("Enemy Actions")]
    public List<Card> enemyCards;

    public BattleManager battleManager;
    PlayerInfoController gameManager;
    public Image e_healthBar;
    public Image e_damageBar;
    public TMP_Text e_healthText;

    public GameObject intent;


    private void Awake()
    {
        battleManager = FindObjectOfType<BattleManager>();
        gameManager = FindObjectOfType<PlayerInfoController>();
        if(!isPlayer)
        {
            intent = transform.GetChild(4).gameObject; 
        }
        // assign stats from PlayerInfoController
        if (isPlayer)
        {
            maxHealth = gameManager.MaxHealth;
            currentHealth = gameManager.CurrentHealth;
            strength = gameManager.Strength;
            defence = gameManager.Defence;
        }
        else
        {
        currentHealth = maxHealth;
        GameObject healthBar = transform.GetChild(3).gameObject;
        e_healthBar = healthBar.transform.GetChild(1).GetComponent<Image>();
        e_damageBar = healthBar.transform.GetChild(0).GetComponent<Image>();
        e_healthText = healthBar.transform.GetChild(2).GetComponent<TMP_Text>();
        e_healthText.text = currentHealth.ToString() + "/" + maxHealth.ToString();
        }
        buffs= new List<BuffBase>();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            DebugBuffs();
        }
    }

    public void DebugBuffs()
    {
        if (isPlayer)
        {
            foreach (BuffBase buff in buffs)
            {
                Debug.Log(buff.buffName + " " + buff.stacks);
            }
        }
    }

    public void TakeDamage(int amount)
    {
        if (currentBlock > 0) //Calculate blocked damage, if any.
        {
            amount = BlockDamage(amount);
        }
        if(isPlayer)
        {
            gameManager.CurrentHealth -= amount;
            PlayerInfoPanel.Instance.UpdateStats();
            gameManager.SetHealthBar();
        }

        currentHealth -= amount; //Take damage
        if(!isPlayer)
        {
            e_healthBar.fillAmount = (float) currentHealth/maxHealth;
            e_healthText.text = currentHealth.ToString() + "/" + maxHealth.ToString();
        }
        if(currentHealth<=0) //Die
        {
            if (!isPlayer)
           {
                intent.GetComponent<Intent>().DisableIntent();
                gameObject.GetComponent<Canvas>().enabled = false;
                battleManager.AddKill();
            }
        }
    }

    public void AddBlock(int amount) //Flat add to Entity block
    {
        currentBlock += amount;
        Debug.Log($"Added {amount} block");
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
        Debug.Log("Blocked " + (amount) + " damage");
        return amount;
    }

    public void AddBuff(BuffBase buff)
    {
        bool buffFound = false;

        for (int i = 0; i < buffs.Count; i++)
        {
            if (buffs[i].buffName == buff.buffName)
            {
                buffFound = true;
                buffs[i].Activate();
            }
        }

        if (!buffFound)
        {
            buffs.Add(buff);
            buff.Activate();
        }
    }

    public BuffBase GetBuff(string buffName) //Return a reference to a buff on entity by buffName
    {
        BuffBase buffReturn = null;

        foreach (BuffBase buff in buffs)
        {
            if (buff.buffName == buffName)
            {
                buffReturn = buff;
            }
        }

        if (buffReturn == null)
        {
            Debug.Log(buffName + " Buff Not Found");
        }

        return buffReturn;
    }

    public void CleanseBuffs() //Wipe all buffs on an entity
    {
        foreach (BuffBase buff in buffs)
        {
            buff.End();
        }

        buffs.Clear();
    }

}
