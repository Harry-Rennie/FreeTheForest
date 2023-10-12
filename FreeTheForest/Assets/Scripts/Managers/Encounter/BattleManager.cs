//Main brains of the Battle System. Controls the Player Deck and Hand, tracks targeted entites, and delegates playing cards

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System.Xml;
using UnityEditor.Build.Content;
using UnityEngine.SceneManagement;

public class BattleManager : MonoBehaviour
{
    [Header("Cards")]
    public Deck deck;
    public List<Card> cardsInHand;
    public List<Card> discardPile;
    public List<Card> exilePile;

    public CardDisplay selectedCard;
    public List<CardDisplay> handCardObjects;

    [Header("Stats")]
    public Entity cardTarget;
    public Entity player;
    public int maxEnergy;
    public int energy;
    public int energyGain;
    public int drawAmount = 5;
    public bool playersTurn = true;
    public int battleCounter;

    [Header("Enemies")]
    [SerializeField] public List<Entity> enemies;
    public int EnemyCount;
    
    CardActions cardActions;
    PlayerInfoController gameManager;
    public GameObject RewardPanel;

    public void Awake()
    {
        gameManager = FindObjectOfType<PlayerInfoController>();
        cardActions = GetComponent<CardActions>();
        StartBattle();
    }

    //Function initializes the battle state, loading in the deck from GameManager and drawing the opening hand.
    public void StartBattle()
    {
        deck = new Deck();
        cardsInHand = new List<Card>();
        deck.AddCards(gameManager.playerDeck);
        battleCounter = 0;
        energy = maxEnergy;

        DrawCards(drawAmount);
        LoadEnemies();
    }

    //Draw cards. Loop over Deck card draw X times. Load returned card into hand.
    public void DrawCards(int amount)
    {
        int cardsDrawn = 0;
        
        while(cardsDrawn<amount && cardsInHand.Count<=10) //While we have cards to draw AND our hand is not full...
        {
            Card cardDrawn = deck.DrawCard(); //Get the card from the deck
            cardsInHand.Add(cardDrawn); //Put the card in the Hand list
            DisplayCardInHand(cardDrawn); //Display the card
            cardsDrawn++;
        }
    }

    public void LoadEnemies()
    {
        EnemyCount = gameManager.currentEnemies.Count;
        List<Enemy> curEnemies = gameManager.currentEnemies;
        //loop thorugh cur enemies
        for(int i = 0; i < EnemyCount; i++)
        {   
            enemies[i].name = curEnemies[i].title;
            enemies[i].offense = curEnemies[i].offense;
            enemies[i].defense = curEnemies[i].defense;
            enemies[i].maxHealth = curEnemies[i].health;
            enemies[i].currentHealth = curEnemies[i].health;
            enemies[i].enemyCards = curEnemies[i].Actions;
            enemies[i].gameObject.SetActive(true);
        }
    }

    //Load CardDisplay game object with given Card data and make visible.
    public void DisplayCardInHand(Card card)
    {
        CardDisplay cardDis = handCardObjects[cardsInHand.Count-1]; //Get the next available card
        cardDis.LoadCard(card); //Tell card to load the Card data
        cardDis.gameObject.SetActive(true); //Activate the gameObject.
    }

    //Play a given card
    public void PlayCard(CardDisplay card)
    {
        cardActions.PerformAction(card.card, cardTarget); //Tell CardActions to perform action based on Card name and Target if necessary

        energy -= card.card.manaCost; //Reduce energy by card cost (CardActions checks for enough mana)

        selectedCard = null; //Drop the referenced card
        
        DiscardCard(card);
    }

    public void DiscardCard(CardDisplay card)
    {
        card.gameObject.SetActive(false); //Deactivate the gameObject
        cardsInHand.Remove(card.card); //Remove the Hand list item
        deck.Discard(card.card); //Put the card into the Discard pile of the Deck object.
    }

    public void AddKill() //Subtract our enemy counter for checking Battle End
    {
        EnemyCount--;

        if (EnemyCount <= 0)
        {
            OpenReward();
        }
    }

    public void OpenReward() //Complete the battle and return to main map screen
    {
        RewardPanel.SetActive(true);
        int goldToGain = Random.Range(8, 28);
        for(int i = 0; i < goldToGain; i++)
        {
            gameManager.Gold +=1;
            PlayerInfoPanel.Instance.UpdateStats();
        }
    }

    public void AddCard(Card card) //Add reward card to main Player Deck
    {
        gameManager.playerDeck.Add(card);

        SceneLoader.SceneNames = new List<string> { "OverWorld" }; //Return to overworld
        SceneManager.LoadScene("OverWorld");
    }

    public void EndTurn()
    {
        //Set turn to no
        playersTurn = false;
        
        //Discard all player cards
        foreach (CardDisplay card in handCardObjects)
        {
            if(card.gameObject.activeSelf)
            {
                DiscardCard(card);
            }
        }

        //Go through each enemy and execute their actions
        foreach(Entity enemy in enemies)
        {
            if (enemy != null && enemy.gameObject.activeSelf)
            {
                int roll = battleCounter % enemy.enemyCards.Count;

                Card card = enemy.enemyCards[roll];
                cardActions.PerformAction(card, enemy);
            }
        }

        //Increment battleCounter
        battleCounter++;

        //TODO: FUTURE CONTENT: Process buffs
        // ProcessBuffs();

        //Draw the player their next hand
        DrawCards(drawAmount);
        //Restore energy
        energy += energyGain;

        //Set turn to yes
        playersTurn = true;
    }

    private void ProcessBuffs()
    {
        //Process player buffs
        foreach (BuffBase buff in player.buffs)
        {
            if (buff.eachTurn)
            {
                buff.Tick();
            }
            
            if (!buff.isPermanent)
            {
                buff.stacks--;
                if (buff.stacks <= 0)
                {
                    buff.End();
                }
            }
        }

        //Remove all playerBuffs that have a stack of 0
        player.buffs.RemoveAll(buff => buff.stacks <= 0);

        //Now do it for each enemy
        foreach (Entity enemy in enemies)
        {
            if (enemy.gameObject.activeSelf)
            {
                foreach (BuffBase buff in enemy.buffs)
                {
                    if (buff.eachTurn)
                    {
                        buff.Tick();
                    }

                    if (!buff.isPermanent)
                    {
                        buff.stacks--;
                        if (buff.stacks <= 0)
                        {
                            buff.End();
                        }
                    }
                }

                enemy.buffs.RemoveAll(buff => buff.stacks <= 0);
            }
        }
    }
}
