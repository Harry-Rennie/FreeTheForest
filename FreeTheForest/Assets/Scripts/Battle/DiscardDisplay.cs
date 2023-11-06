using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Security;
using System;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// This class controls the display for the discard pile.
/// </summary>
public class DiscardDisplay : MonoBehaviour
{
    [SerializeField] private BattleManager battleManager;

    /// <summary>
    /// The UI element that contains the ScrollRect.
    /// </summary>
    [SerializeField] private ScrollRect ScrollRect;
    /// <summary>
    /// The UI element that contains the scrollable card content.
    /// </summary>
    [SerializeField] private GameObject cardContainer;
    /// <summary>
    /// The prefab configured to display a card's info in the compendium/discard pile display.
    /// </summary>
    [SerializeField] private GameObject compendiumCardPrefab;

    private Deck deck;

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
        deck = battleManager.deck;
        // populate the discard pile display
        UpdateDiscardDisplay();
    }

    private void OnEnable()
    {
        UpdateDiscardDisplay();
    }

    /// <summary>
    /// This function updates the container with all the cards in the discard and exiled piles
    /// </summary>
    public void UpdateDiscardDisplay()
    {
        deck = battleManager.deck;
        setDiscardDisplayHeight();

        // clear the cardContainer of all children (so that it doesn't show duplicates from a previous call)
        foreach (Transform child in cardContainer.transform)
        {
            Destroy(child.gameObject);
        }

        // for each card in deck.DiscardPile, add a CompendiumCard prefab to the cardContainer
        // then display the card info with the CompendiumCard.CompendiumCardDisplay function
        // do this in reverse order so that the cards are displayed in the order they were discarded
        for (int i = deck.DiscardPile.Count - 1; i >= 0; i--)
        {
            Card card = deck.DiscardPile[i];
            GameObject compendiumCard = Instantiate(compendiumCardPrefab);

            // set the parent of the compendiumCard to the cardContainer
            compendiumCard.transform.SetParent(cardContainer.transform);

            // for reasons unknown to me, the scale and z position of the card is changed to weird values when the parent is set
            // these lines of code reset the scale and z position to their original values
            compendiumCard.transform.localScale = new Vector3(1, 1, 1);
            compendiumCard.GetComponent<RectTransform>().localPosition = new Vector3(0, 0, 0);

            // display the card info
            compendiumCard.GetComponent<CompendiumCard>().DisplayCard(card);
        }

        // for each card in deck.ExiledPile, add a CompendiumCard prefab to the cardContainer
        // then display the card info with the CompendiumCard.CompendiumCardDisplay function
        // do this in reverse order so that the cards are displayed in the order they were exiled
        for (int i = deck.ExiledPile.Count - 1; i >= 0; i--)
        {
            Card card = deck.ExiledPile[i];
            GameObject compendiumCard = Instantiate(compendiumCardPrefab);

            // set the parent of the compendiumCard to the cardContainer
            compendiumCard.transform.SetParent(cardContainer.transform);

            // because the scale and z position of the card is changed to weird values when the parent is set
            // these lines of code reset the scale and z position to their original values
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

        // Scroll to the top of the discard pile display
        ScrollRect.verticalNormalizedPosition = 1;
    }

    /// <summary>
    /// This function resizes the height of the card container based on the grid size and amount of cards.
    /// </summary>
    private void setDiscardDisplayHeight()
    {
        // get the rect transform of the cardContainer
        RectTransform cardContainerRect = cardContainer.GetComponent<RectTransform>();

        // get the width of the discard pile display
        int compendiumWidth = (int)cardContainer.GetComponent<RectTransform>().rect.width;
        // get the minimum (initial) height of the discard pile display
        int minCompendiumHeight = (int)cardContainer.GetComponent<RectTransform>().rect.height;
        // get the width of the card cell from the grid layout group of the card container
        int cardCellWidth = (int)cardContainer.GetComponent<GridLayoutGroup>().cellSize.x;
        // get the height of the card cell from the grid layout group of the card container
        int cardCellHeight = (int)cardContainer.GetComponent<GridLayoutGroup>().cellSize.y;

        // calculate the amount of cards that can fit in a row
        int amtCardsPerRow = compendiumWidth / cardCellWidth;
        // calculate the amount of rows needed to display all the cards
        int rows = (int)Mathf.Ceil((float)(deck.DiscardPile.Count + deck.ExiledPile.Count) / amtCardsPerRow);

        // set the height of the cardContainer to the height of the cards * the amount of rows, or the minimum height of the discard pile display
        cardContainerRect.sizeDelta = 
            new Vector2(cardContainerRect.sizeDelta.x, (rows > minCompendiumHeight / cardCellHeight) ? cardCellHeight * rows : minCompendiumHeight);
    }

    /// <summary>
    /// This function is used to add sample cards to the discard and exiled piles for testing.
    /// </summary>
    /// <param name="discardAmt">The amount of cards to add to the discard pile.</param>
    /// <param name="exileAmt">The amount of cards to add to the exiled pile.</param>
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
            Card testCard = ScriptableObject.CreateInstance<Card>();
            testCard.title = "New Card " + i;
            testCard.description = "New Card Description";
            testCard.manaCost = 0;

            // add card to discard pile with playerDeck.Discard(card);
            deck.ExileCard(testCard);
        }
    }
}