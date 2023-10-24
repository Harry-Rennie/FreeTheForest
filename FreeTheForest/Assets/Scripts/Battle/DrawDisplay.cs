using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// This class controls the display for the draw pile.
/// </summary>
public class DrawDisplay : MonoBehaviour
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
    /// The prefab configured to display a card's info in the compendium/draw pile display.
    /// </summary>
    [SerializeField] private GameObject compendiumCardPrefab;

    private Deck deck;

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
        deck = battleManager.deck;
        // populate the draw pile display
        UpdateDrawDisplay();
    }

    /// <summary>
    /// This function updates the panel that contains the draw pile with the current draw pile.
    /// </summary>
    public void UpdateDrawDisplay()
    {
        // clear the cardContainer of all children (so that it doesn't show duplicates from a previous call)
        foreach (Transform child in cardContainer.transform)
        {
            Destroy(child.gameObject);
        }

        setDrawDisplayWidth();

        // for each card in deck.MainDeck (the draw pile), add a CompendiumCard prefab to the cardContainer
        // then display the card info with the CompendiumCard.CompendiumCardDisplay function
        foreach (Card card in deck.MainDeck)
        {
            GameObject compendiumCard = Instantiate(compendiumCardPrefab);

            // set the parent of the compendiumCard to the cardContainer
            compendiumCard.transform.SetParent(cardContainer.transform);

            // because the scale and z position of the card is changed to weird values when the parent is set
            // these lines of code reset the scale and z position to their original values
            compendiumCard.transform.localScale = new Vector3(1, 1, 1);
            compendiumCard.GetComponent<RectTransform>().localPosition = new Vector3(0, 0, 0);

            // display the card info
            compendiumCard.GetComponent<CompendiumCard>().DisplayCard(card);
        }

        // Scroll to the left of the draw pile display
        ScrollRect.horizontalNormalizedPosition = 0;
    }

    /// <summary>
    /// This function resizes the width of the draw pile display to fit the cards in the draw pile.
    /// </summary>
    private void setDrawDisplayWidth()
    {
        // Get the rect transform of the cardContainer
        RectTransform cardContainerRect = cardContainer.GetComponent<RectTransform>();

        // Get the minimum (initial) width of the draw pile display
        int minDrawDisplayWidth = (int)cardContainer.GetComponent<RectTransform>().rect.width;

        // occasionally it loses the reference to the deck here, not sure why, but this fixes it
        if (deck == null)
        {
            deck = battleManager.deck;
        }
        // Calculate the amount of cards in the deck
        int cardCount = deck.MainDeck.Count;

        // Calculate the total width of the draw pile display based on card count and spacing
        int width = cardCount * (int)compendiumCardPrefab.GetComponent<RectTransform>().rect.width;

        // Set the width of the cardContainer to the calculated width or the minimum width of the draw pile display with a ternary
        cardContainerRect.sizeDelta = new Vector2((width > minDrawDisplayWidth) ? width : minDrawDisplayWidth, cardContainerRect.sizeDelta.y);

                
    }

    /// <summary>
    /// This function is used to add sample cards to the draw pile for testing.
    /// </summary>
    /// <param name="drawAmt">The amount of cards to add to the draw pile.</param>
    private void testDraw(int drawAmt = 30)
    {
        for (int i = 0; i < drawAmt; i++)
        {
            Card testCard = new Card();
            testCard.title = "New Card " + i;
            testCard.description = "New Card Description";
            testCard.manaCost = 0;
            deck.AddCard(testCard);
        }
    }
}