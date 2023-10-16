using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// This class controls the compendium UI.
/// It displays all the cards the player has collected.
/// </summary>
public class DiscardDisplay : MonoBehaviour
{
    public float Val { get; set; } = 0;
    /// <summary>
    /// The UI element that contains the ScrollRect.
    /// </summary>
    [SerializeField] private ScrollRect ScrollRect;
    /// <summary>
    /// The UI element that contains the scrollable card content.
    /// </summary>
    [SerializeField] private GameObject cardContainer;
    /// <summary>
    /// The prefab configured to display a card's info in the compendium.
    /// </summary>
    [SerializeField] private GameObject compendiumCardPrefab;
    [SerializeField] private GameObject exiledShroud;
    private Deck deck;
    private BattleManager battleManager;

    #region Singleton
    // singleton pattern
    public static DiscardDisplay Instance;
    private void Awake()
    {
        if (Instance != null)
        {
            Debug.LogWarning("More than one instance of DiscardDisplay found.");
            // destroy myself
            Destroy(gameObject);
            return;
        }
        Instance = this;

    }
    #endregion

    private void Start()
    {
        battleManager = FindObjectOfType<BattleManager>();
        deck = battleManager.deck;
        testDiscard(20, 10);
        // resize the height of the compendium if necessary
        setCompendiumHeight();
        // populate the compendium with the player's cards
        displayCards();
    }

    // private void Update()
    // {
    //     ScrollRect.verticalNormalizedPosition = Val;
    // }

    /// <summary>
    /// This function displays all the cards in the player's deck in the compendium.
    /// </summary>
    private void displayCards()
    {
        // for each card in deck.DiscardPile, add a CompendiumCard prefab to the cardContainer
        // then display the card info with the CompendiumCard.CompendiumCardDisplay function
        foreach (Card card in deck.DiscardPile)
        {
            GameObject compendiumCard = Instantiate(compendiumCardPrefab);
            // set the parent of the compendiumCard to the cardContainer
            compendiumCard.transform.SetParent(cardContainer.transform);
            // for reasons unknown to me, the scale and z position of the card is changed to weird values when the parent is set
            // these lines of code resets the scale and z position to their original values
            compendiumCard.transform.localScale = new Vector3(1, 1, 1);
            compendiumCard.GetComponent<RectTransform>().localPosition = new Vector3(0, 0, 0);

            // display the card info
            compendiumCard.GetComponent<CompendiumCard>().DisplayCard(card);
        }
        // for each card in deck.ExiledPile, add a CompendiumCard prefab to the cardContainer
        // then display the card info with the CompendiumCard.CompendiumCardDisplay function
        foreach (Card card in deck.ExiledPile)
        {
            GameObject compendiumCard = Instantiate(compendiumCardPrefab);
            // set the parent of the compendiumCard to the cardContainer
            compendiumCard.transform.SetParent(cardContainer.transform);
            // again, for reasons unknown, the scale and z position of the card is changed to weird values when the parent is set
            // these lines of code resets the scale and z position to their original values
            compendiumCard.transform.localScale = new Vector3(1, 1, 1);
            compendiumCard.GetComponent<RectTransform>().localPosition = new Vector3(0, 0, 0);

            // Find the "CardImg" child object
            Transform cardImg = compendiumCard.transform.Find("CardImg");

            if (cardImg != null)
            {
                // Find the "ExiledShroud" child object within "CardImg" and activate it
                Transform exiledShroud = cardImg.transform.Find("Exiled Shroud");

                if (exiledShroud != null)
                {
                    exiledShroud.gameObject.SetActive(true);
                }
            }

            // Display the card info using the CompendiumCard component
            CompendiumCard compendiumCardComponent = compendiumCard.GetComponent<CompendiumCard>();
            if (compendiumCardComponent != null)
            {
                compendiumCardComponent.DisplayCard(card);
            }
        }

    }

    /// <summary>
    /// This function resizes the height of the card container based on the grid size and amount of cards.
    /// It's not really necessary to do it dynamically, but it avoids plugging in a bunch of magic numbers.
    /// </summary>
    private void setCompendiumHeight()
    {
        // get the rect transform of the cardContainer
        RectTransform cardContainerRect = cardContainer.GetComponent<RectTransform>();

        // get the width of the compendium
        int compendiumWidth = (int)cardContainer.GetComponent<RectTransform>().rect.width;
        // get the minimum (initial) height of the compendium
        int minCompendiumHeight = (int)cardContainer.GetComponent<RectTransform>().rect.height;
        // get the width of the card cell from the grid layout group of the card container
        int cardCellWidth = (int)cardContainer.GetComponent<GridLayoutGroup>().cellSize.x;
        // get the height of the card cell from the grid layout group of the card container
        int cardCellHeight = (int)cardContainer.GetComponent<GridLayoutGroup>().cellSize.y;

        // calculate the amount of cards that can fit in a row
        int amtCardsPerRow = compendiumWidth / cardCellWidth;
        // calculate the amount of rows needed to display all the cards
        int rows = (int)Mathf.Ceil((float)(deck.DiscardPile.Count + deck.ExiledPile.Count) / amtCardsPerRow);

        // set the height of the cardContainer to the height of the cards * the amount of rows, or the minimum height of the compendium
        cardContainerRect.sizeDelta = (rows > minCompendiumHeight / cardCellHeight) ?
            new Vector2(cardContainerRect.sizeDelta.x, cardCellHeight * rows) : new Vector2(cardContainerRect.sizeDelta.x, minCompendiumHeight);

        // scroll to the top of the compendium
        ScrollRect.verticalNormalizedPosition = 1;
    }

    public void SetVNPos(float val)
    {
        ScrollRect.verticalNormalizedPosition = val;
    }

    private void testDiscard(int discardAmt = 5, int exileAmt = 5)
    {

        for (int i = 0; i < discardAmt; i++)
        {
                        Card testCard = new Card();
            testCard.title = "New Card " + i;
            testCard.description = "New Card Description";
            testCard.manaCost = 0;

            // add card to discard pile with playerDeck.Discard(card);
            deck.Discard(testCard);
        }
        for (int i = 0; i < exileAmt; i++)
        {
                        Card testCard = new Card();
            testCard.title = "New Card " + i;
            testCard.description = "New Card Description";
            testCard.manaCost = 0;
            // add card to discard pile with playerDeck.Discard(card);
            deck.ExileCard(testCard);
        }
        // log the number of cards in the discard pile and the exile pile
        Debug.Log("Discard Pile: " + deck.DiscardPile.Count);
        Debug.Log("Exile Pile: " + deck.ExiledPile.Count);
    }
}