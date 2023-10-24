//Main brains of the Battle System. Controls the Player Deck and Hand, tracks targeted entites, and delegates playing cards

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System.Xml;
using UnityEngine.SceneManagement;

public class BattleManager : MonoBehaviour
{
    [Header("Cards")]
    public Deck deck;
    private List<Card> _cardsInHand = new List<Card>();
    public List<CardDisplay> handCardObjects;
    [SerializeField] public Hand handManager;
    [Header("Stats")]
    public Entity cardTarget;
    public Entity player;
    public int maxEnergy;
    public int energy;
    public int energyGain;
    public int drawAmount = 5;
    public bool playersTurn = true;
    public int battleCounter;
    public bool targeting = false;

    [Header("Enemies")]
    [SerializeField] public List<Entity> enemies;
    public int EnemyCount;
    
    CardActions cardActions;
    PlayerInfoController gameManager;
    public GameObject RewardPanel;

    [SerializeField] public GameObject battleCanvas;
    private TargetSlot targetSlot;
    private LineRenderer targetLine;
    public CardDisplay selectedCard;

    public delegate void ClearTargetingEvent();
    public event ClearTargetingEvent OnClearTargeting;

    public bool battleOver;
    public List<Card> cardsInHand
    {
        get { return _cardsInHand; } // Getter to access the cardsInHand list
        set
        {
            _cardsInHand = value; // Set the new value
        }
    }
    public void Awake()
    {
        gameManager = FindObjectOfType<PlayerInfoController>();
        cardActions = GetComponent<CardActions>();
        StartBattle();
    }

    void Start()
    {
        targetSlot = FindObjectOfType<TargetSlot>();
        targetLine = FindObjectOfType<LineRenderer>();
        targetLine.material = new Material(Shader.Find("Sprites/Default"));
        targetLine.startWidth = 0.05f; //set line width
        targetLine.endWidth = 0.05f;
        targetLine.positionCount = 0;
        targetSlot.OnChildChanged.AddListener(OnTargetSlotChildChanged);
        battleOver = false;
    }

void Update()
{
    if (targeting)
    {
        if (Input.GetMouseButtonDown(1))
        {
            ClearTargeting();
            return;
        }
        int numPoints = 50;
        targetLine.positionCount = numPoints;
        float cardSlotWidth = targetSlot.GetComponent<RectTransform>().rect.width / 2;
        float cardSlotHeight = targetSlot.GetComponent<RectTransform>().rect.height / 2;
        Vector2 cardSlotMidY = new Vector2(targetSlot.transform.position.x - cardSlotWidth, targetSlot.transform.position.y - cardSlotHeight + 55f);

        Vector2 canvasMousePosition;
        RectTransform canvasRect = battleCanvas.GetComponent<RectTransform>();
        RectTransformUtility.ScreenPointToLocalPointInRectangle(canvasRect, Input.mousePosition, Camera.main, out canvasMousePosition);

        Vector2 startPoint = cardSlotMidY;
        Vector2 endPoint = canvasMousePosition;
        
        //control point for the quadratic Bezier curve.
        Vector2 controlPoint = (startPoint + endPoint) / 2 + Vector2.up * 80; //adjust magic number to change angle of curve

        for (int i = 0; i < numPoints; i++)
        {
            float t = i / (float)(numPoints - 1);
            Vector2 pointOnCurve = QuadraticBezierCurve(startPoint, controlPoint, endPoint, t);
            targetLine.SetPosition(i, pointOnCurve);
        }
    }
}
    //couldnt get .lerp to work so used bezier quadratic for curve
   private Vector2 QuadraticBezierCurve(Vector2 p0, Vector2 p1, Vector2 p2, float t) 
   {
        //lerping between 2 points equation: (1 - t) * (1 - t) * p0 + 2 * (1 - t) * t * p1 + t * t * p2;
        Vector2 pointOnCurve = (1 - t) * (1 - t) * p0 + 2 * (1 - t) * t * p1 + t * t * p2;
        return pointOnCurve;
    }
    private void OnTargetSlotChildChanged()
    {
       //if the target slot has only one child, set the card target to that child
        
         if(targetSlot.transform.childCount == 1)
         {
              selectedCard = targetSlot.transform.GetChild(0).GetComponent<CardDisplay>();
            BeginTargeting();
         }
         else
         {
              selectedCard = null;
         }
    }

    private void BeginTargeting()
    {
        if (selectedCard == null) //check if a card is selected for targeting
        {
            targeting = false;
            return;
        }
        else
        {
            targeting = true;
        }
    }

    private void ClearTargeting()
    {
        targeting = false;

        if (targetLine != null)
        {
            targetLine.positionCount = 0; //clear the LineRenderer
        }
        OnClearTargeting?.Invoke();
    }

    //Function initializes the battle state, loading in the deck from GameManager and drawing the opening hand.
    public void StartBattle()
    {
        deck = new Deck();
        cardsInHand = new List<Card>();
        deck.DiscardPile.AddRange(gameManager.playerDeck);
        battleCounter = 0;
        energy = maxEnergy;

        DrawCards(drawAmount);
        LoadEnemies();
    }
    //Draw cards. Loop over Deck card draw X times. Load returned card into hand.
    public void DrawCards(int amount)
    {
        int cardsDrawn = 0;
        List<Card> newCardsInHand = new List<Card>(cardsInHand); // Create a copy of the current hand
        while (cardsDrawn < amount && newCardsInHand.Count <= 10) // While we have cards to draw AND our hand is not full...
        {
            Card cardDrawn = deck.DrawCard(); // Get the card from the deck
            newCardsInHand.Add(cardDrawn); // Put the card in the Hand list (in the copy)
            cardsInHand = newCardsInHand; //update the hand with the copy, which triggers the action
            DisplayCardInHand(cardDrawn); // Display the card
            cardsDrawn++;
        }
    }

    //debug method for testing
    public void DrawCard()
    {
        Card cardDrawn = deck.DrawCard();
        List<Card> newCardsInHand = new List<Card>(cardsInHand);
        newCardsInHand.Add(cardDrawn);
        cardsInHand = newCardsInHand; //this triggers the action
        DisplayCardInHand(cardDrawn);
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
        handManager.heldCards.Add(cardDis);
    }

    //Play a given card
    public void PlayCard(CardDisplay card)
    {
        cardActions.PerformAction(card.card, cardTarget); //Tell CardActions to perform action based on Card name and Target if necessary

        energy -= card.card.manaCost; //Reduce energy by card cost (CardActions checks for enough mana)\
        PlayerInfoController.instance.Energy = energy;
        PlayerInfoPanel.Instance.UpdateStats();
        ClearTargeting();
        selectedCard = null; //Drop the referenced card
        
        DiscardCard(card);
    }

    public void DiscardCard(CardDisplay card)
    {
        card.gameObject.SetActive(false); //Deactivate the gameObject
        List<Card> newCardsInHand = new List<Card>(cardsInHand);
        newCardsInHand.Remove(card.card);//Remove the Hand list item
        cardsInHand = newCardsInHand;//trigger action
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
        battleOver = true;
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
        energy = maxEnergy;
        PlayerInfoController.instance.Energy = energy;
        PlayerInfoPanel.Instance.UpdateStats();
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
