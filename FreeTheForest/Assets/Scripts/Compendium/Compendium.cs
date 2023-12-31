using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// This class controls the compendium UI.
/// It displays all the cards the player has collected.
/// </summary>
public class Compendium : MonoBehaviour
{
    /// <summary>
    /// The UI element that contains the scrollRect.
    /// </summary>
    [SerializeField] private GameObject scrollRect;
    /// <summary>
    /// The UI element that contains the scrollable card content.
    /// </summary>
    [SerializeField] private GameObject cardContainer;
    /// <summary>
    /// The prefab configured to display a card's info in the compendium.
    /// </summary>
    [SerializeField] private GameObject compendiumCardPrefab;
    private List<Card> playerCards;


    #region Singleton
    // singleton pattern
    public static Compendium Instance;
    private void Awake()
    {
        if (Instance != null)
        {
            Debug.LogWarning("More than one instance of Compendium found.");
            // destroy myself
            Destroy(gameObject);
            return;
        }
        Instance = this;

    }
    #endregion

    private void Start()
    {
        // assign the player's deck to playerCards
        playerCards = PlayerInfoController.instance.playerDeck;
        // resize the height of the compendium if necessary
        setCompendiumHeight();
        // populate the compendium with the player's cards
        displayCards();
    }

    /// <summary>
    /// This function displays all the cards in the player's deck in the compendium.
    /// </summary>
    private void displayCards()
    {
        // for each card in playerCards), add a CompendiumCard prefab to the cardContainer
        // then display the card info with the CompendiumCard.CompendiumCardDisplay function
        foreach (Card card in playerCards)
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
        int rows = (int)Mathf.Ceil((float)playerCards.Count / amtCardsPerRow);

        // set the height of the cardContainer to the height of the cards * the amount of rows, or the minimum height of the compendium
        cardContainerRect.sizeDelta = 
            new Vector2(cardContainerRect.sizeDelta.x, (rows > minCompendiumHeight / cardCellHeight) ? cardCellHeight * rows : minCompendiumHeight);

        // scroll to the top of the compendium
        scrollRect.GetComponent<ScrollRect>().verticalNormalizedPosition = 1;
    }
}