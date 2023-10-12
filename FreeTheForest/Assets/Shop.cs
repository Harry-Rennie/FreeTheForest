using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shop : MonoBehaviour
{
    private PlayerInfoController playerInfoController;
    private TrinketManager trinketManager;

    [SerializeField] private GameObject compendiumCardPrefab;

    //parent canvas for card section of shop
    [SerializeField] private GameObject cards;
    [SerializeField] private GameObject cardContainer;
    public List<Card> cardLibrary = new List<Card>();
    public int cost;
    // Start is called before the first frame update
    void Start()
    {
        playerInfoController = PlayerInfoController.instance;
        trinketManager = TrinketManager.Instance;
        DisplayCards();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void BuyCard(Card card)
    {
        if (playerInfoController.Gold >= cost)
        {
            playerInfoController.Gold -= cost;
            playerInfoController.playerDeck.Add(card);
        }
    }
    public void DisplayCards()
    {
        //randomly select 6 cards from the card library
        //display them
        List <Card> cardsForSale = new List<Card>();
        for (int i = 0; i < 6; i++)
        {
            int randomIndex = Random.Range(0, cardLibrary.Count);
            cardsForSale.Add(cardLibrary[randomIndex]);
        }
        for (int i = 0; i < cardsForSale.Count; i++)
        {
            Card card = cardsForSale[i];
            GameObject compendiumCard = Instantiate(compendiumCardPrefab);
            
            if(i > 2)
            {
                //display the card underneath the first row
                compendiumCard.GetComponent<RectTransform>().anchoredPosition = new Vector2(0, -200);
            }
            else
            {
                //display the card in the first row
                compendiumCard.GetComponent<RectTransform>().anchoredPosition = new Vector2(0, 0);
            }
            compendiumCard.transform.SetParent(cardContainer.transform);
            compendiumCard.transform.localScale = new Vector3(1.5f, 1.5f, 1f);
            compendiumCard.GetComponent<CompendiumCard>().DisplayCard(card);
        }
        cardContainer.transform.SetParent(cards.transform);
    }
}
