using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// This class controls the compendium UI.
/// It displays all the cards the player has collected.
/// </summary>
public class DrawDisplay : MonoBehaviour
{
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
    public static DrawDisplay Instance;
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
        testDraw(20);
        // resize the height of the compendium if necessary
        setCompendiumWidth();
        // populate the compendium with the player's cards
        displayCards();
    }

    /// <summary>
    /// This function displays all the cards in the player's deck in the compendium.
    /// </summary>
    private void displayCards()
    {
        // for each card in deck.DiscardPile, add a CompendiumCard prefab to the cardContainer
        // then display the card info with the CompendiumCard.CompendiumCardDisplay function
        foreach (Card card in deck.MainDeck)
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


    }

    private void setCompendiumWidth()
    {
        // Get the rect transform of the cardContainer
        RectTransform cardContainerRect = cardContainer.GetComponent<RectTransform>();

        // Get the minimum (initial) width of the compendium
        int minCompendiumWidth = (int)cardContainer.GetComponent<RectTransform>().rect.width;

        // Calculate the amount of cards in the deck
        int cardCount = deck.MainDeck.Count;

        // Calculate the total width of the compendium based on card count and spacing
        int width = cardCount * (int)compendiumCardPrefab.GetComponent<RectTransform>().rect.width;
        // Set the width of the cardContainer to the calculated width or the minimum width of the compendium
        cardContainerRect.sizeDelta = new Vector2(Mathf.Max(width, minCompendiumWidth), cardContainerRect.sizeDelta.y);

        // Scroll to the left of the compendium (if necessary)
        ScrollRect.horizontalNormalizedPosition = 0;
    }

    private void testDraw(int drawAmt = 30)
    {
        Card testCard = new Card();

        testCard.title = "New Card";
        testCard.description = "New Card Description";
        testCard.manaCost = 0;


        for (int i = 0; i < drawAmt; i++)
        {
            // add card to draw pile
            deck.AddCard(testCard);

        }

        // log the number of cards in the discard pile and the exile pile
        Debug.Log("Discard Pile: " + deck.MainDeck.Count);
    }
}