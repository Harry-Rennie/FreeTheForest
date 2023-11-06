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
    [SerializeField] CardAnimator cardSprite;
    public CardDisplayAnimator cardAnimator;
    public Deck deck;
    private List<Card> _cardsInHand = new List<Card>();
    public List<CardDisplay> handCardObjects;
    [SerializeField] public Hand handManager;
    [Header("Stats")]
    public Entity cardTarget;
    public Entity player;

    public int energy;
    public int energyGain;
    public int currentMaxEnergy;
    public int currentEnergy; //energy in context of the battle/turn
    [SerializeField] public GameObject energyCounter; //Max energy will stay the same in top ui, counter visually shows battle specific energy
    [SerializeField] public TMP_Text eCounter; //Txt for now, parent object might contain animations.
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
    public TargetSlot targetSlot;
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
        gameManager.GetCamera();
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
        targetLine.sortingOrder = 400;
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
        Vector2 cardSlotMidY = new Vector2(targetSlot.transform.position.x - cardSlotWidth, targetSlot.transform.position.y - cardSlotHeight + 20f);

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
            cardAnimator = selectedCard.GetComponent<CardDisplayAnimator>();
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
        energy = PlayerInfoController.instance.Energy;
        currentEnergy = energy;
        currentMaxEnergy = energy;
        SetEnergyCounter(currentEnergy, currentMaxEnergy);
        gameManager.activateHealthBar();
        gameManager.SetHealthBar();
        LoadEnemies();
        StartCoroutine(DrawCards(drawAmount));
    }
    //Draw cards. Loop over Deck card draw X times. Load returned card into hand.
    public IEnumerator DrawCards(int amount)
    {
        int cardsDrawn = 0;
        while (cardsDrawn < amount && cardsInHand.Count <= 10)
        {
            Card cardDrawn = deck.DrawCard();
            cardsInHand.Add(cardDrawn);
            yield return StartCoroutine(DisplayCardInHand(cardDrawn));
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
            enemies[i].strength = curEnemies[i].strength;
            enemies[i].defence = curEnemies[i].defence;
            enemies[i].maxHealth = curEnemies[i].health;
            enemies[i].currentHealth = curEnemies[i].health;
            enemies[i].enemyCards = curEnemies[i].Actions;
            enemies[i].gameObject.SetActive(true);
        }
    }

    //Load CardDisplay game object with given Card data and make visible.
    public IEnumerator DisplayCardInHand(Card card)
    {
        cardSprite.Draw();
        yield return new WaitUntil(() => cardSprite.IsAnimationComplete);
        cardSprite.IsAnimationComplete = false;
        CardDisplay cardDis = handCardObjects[cardsInHand.Count-1];
        cardDis.LoadCard(card);
        cardDis.gameObject.SetActive(true);
        if (cardDis.GetComponent<Canvas>().enabled == false) //if the card was previously discarded, some elements are disabled to smooth animation - so re-enable them
        {
            cardDis.GetComponent<Canvas>().enabled = true;
            cardDis.transform.GetChild(0).gameObject.SetActive(true);
            cardDis.transform.GetChild(1).gameObject.SetActive(true);
            cardDis.transform.GetChild(2).gameObject.SetActive(true);
            cardDis.transform.GetChild(3).gameObject.SetActive(true);
        }
        cardDis.OnCardLoad();
        CardDisplayAnimator resetDiscard = cardDis.GetComponent<CardDisplayAnimator>();
        resetDiscard.discarded = false;
        handManager.heldCards.Add(cardDis);
    }

    //Play a given card
    public IEnumerator PlayCard(CardDisplay card)
    {
        int tempCurrentMax = currentMaxEnergy;
        CardDisplayAnimator cardToAnimate = selectedCard.GetComponent<CardDisplayAnimator>();
        if(card.card.manaCost > currentEnergy)
        {
            card.DeselectCard();
            ClearTargeting();
        }
        else
        {
            cardActions.PerformAction(card.card, cardTarget); //Tell CardActions to perform action based on Card name and Target if necessary
            if(card.card.cardType == Card.CardType.Attack )
            {
                gameManager.deactivateHealthBar();
            }
            currentEnergy -= card.card.manaCost; //Reduce energy by card cost (CardActions checks for enough mana)\
            if(tempCurrentMax != currentMaxEnergy)
            {
                SetEnergyCounter(currentEnergy, currentMaxEnergy);
            }
            else
            {
                SetEnergyCounter(currentEnergy, tempCurrentMax);
            }
            ClearTargeting();
            cardToAnimate.Discard();
            yield return new WaitUntil(() => cardAnimator.IsDisplayAnimationComplete);
            DiscardCard(card);
            if(card.card.cardType == Card.CardType.Attack)
            {
                yield return new WaitForSeconds(0.5f);
                gameManager.activateHealthBar();
            }
        }
    }

    public IEnumerator PlayDiscardAnimation()
    {
        foreach(CardDisplay card in handCardObjects)
        {
            if(card.gameObject.activeSelf)
            {
                cardAnimator = card.GetComponent<CardDisplayAnimator>();
                cardAnimator.Discard();
                yield return new WaitUntil(() => cardAnimator.IsDisplayAnimationComplete);
            }
        }
    }
    public void DiscardCard(CardDisplay card)
    {
        // card.OnCardDeload();
        card.gameObject.SetActive(false);
        cardsInHand.Remove(card.card);
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
        // StartCoroutine(PlayDiscardAnimation());
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

        //Restore energy
        SetEnergyCounter(energy, energy);
        currentEnergy = energy;
        PlayerInfoPanel.Instance.UpdateStats();
        //Set turn to yes
        playersTurn = true;
                //Draw the player their next hand
        StartCoroutine(DrawCards(drawAmount));
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

    private void SetEnergyCounter(int e, int maxE)
    {
        string energyString;
        //energy/maxEnergy
        energyString = e.ToString() + "/" + maxE.ToString();
        eCounter.text = energyString;
    }
}
