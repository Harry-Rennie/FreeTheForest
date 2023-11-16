using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class Shop : MonoBehaviour
{
    private PlayerInfoController playerInfoController;
    private TrinketManager trinketManager;

    [SerializeField] private GameObject compendiumCardPrefab;

    //parent canvas for card section of shop
    [SerializeField] private GameObject cards;
    [SerializeField] private GameObject cardContainer;
    public List<Card> cardLibrary = new List<Card>();
    public List <Card> cardsForSale = new List<Card>();
    [SerializeField] public List<TextMeshProUGUI> cardCosts = new List<TextMeshProUGUI>();
    // Start is called before the first frame update
    [SerializeField] public Sprite shopKeeperIdle;
    [SerializeField] public Sprite shopKeeperActive;
    [SerializeField] public Image shopKeeper;
    public void Start()
    {
        playerInfoController = PlayerInfoController.instance;
        trinketManager = TrinketManager.Instance;
        DisplayCards();
        CostCards();
    }

    public void Update()
    {
        if (Input.GetMouseButtonDown(0)) // Left mouse button click
        {
            CardClicked(cardsForSale);
        }
    }

    public void BuyCard(Card card, int cost, GameObject cardObject, TextMeshProUGUI costText)
    {
        if (playerInfoController.Gold >= cost)
        {
            playerInfoController.Gold -= cost;
            playerInfoController.playerDeck.Add(card);
            costText.text = "SOLD";
            PlayerInfoPanel.Instance.UpdateGold();
            StartCoroutine(ShopKeeperAnimation());
        }
        else
        {
            Debug.Log("not enough gold");
        }
    }
    public void DisplayCards()
    {
        //randomly select 6 cards from the card library
        //display them

        for (int i = 0; i < 6; i++)
        {
            int randomIndex = Random.Range(0, cardLibrary.Count);
            cardsForSale.Add(cardLibrary[randomIndex]);
            cardLibrary.RemoveAt(randomIndex); //stopping duplicate cards in shop
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

            //add a BoxCollider2D with the same size as the card's RectTransform
            BoxCollider2D boxCollider = compendiumCard.AddComponent<BoxCollider2D>();
            RectTransform cardRectTransform = compendiumCard.GetComponent<RectTransform>();
            boxCollider.size = new Vector2(cardRectTransform.rect.width, cardRectTransform.rect.height);

            compendiumCard.transform.SetParent(cardContainer.transform);
            compendiumCard.transform.localScale = new Vector3(1.75f, 1.75f, 1f);
            compendiumCard.GetComponent<CompendiumCard>().DisplayCard(card);
        }
        cardContainer.transform.SetParent(cards.transform);
    }

    public void CostCards()
    {
        float goldLMod = playerInfoController.Gold * 0.15f + Random.Range(0, 10);
        float goldHMod = playerInfoController.Gold * 1.15f - Random.Range(0, 10);
        //round to nearest int
        int goldLowMod = Mathf.RoundToInt(goldLMod);
        int goldHighMod = Mathf.RoundToInt(goldHMod);
        for(int i = 0; i < cardCosts.Count; i++)
        {
            int randomCost = Random.Range(goldLowMod, goldHighMod);
            cardCosts[i].text = randomCost.ToString();
        }
    }

    // detect card clicked
    public void CardClicked(List<Card> cardsForSale)
    {
        List<Card> cardsToCheck = cardsForSale;
        if (EventSystem.current.IsPointerOverGameObject())
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit2D hit = Physics2D.Raycast(ray.origin, ray.direction);
            CompendiumCard clickedCard = hit.collider.GetComponent<CompendiumCard>();

            if (hit.collider != null)
            {

                if (clickedCard != null)
                {
                    //get the compendium card gameobject
                    GameObject card = hit.collider.gameObject;
                    TextMeshProUGUI titleText = card.GetComponentInChildren<TextMeshProUGUI>();
                    Debug.Log("card title: " + titleText.text);
                    //get the cost of the card from cardCosts list
                    for(int i = 0; i < cardsToCheck.Count; i++)
                    {
                        string cardTitle = cardsToCheck[i].title;
                        if(titleText.text == cardTitle)
                        {
                            string costText = cardCosts[i].text;
                            if(costText == "SOLD")
                            {
                                Debug.Log("card already sold");
                                return;
                            }
                            else
                            {
                                int cost = int.Parse(cardCosts[i].text);
                                BuyCard(cardsToCheck[i], cost, card, cardCosts[i]);
                            }
                        }
                    }
                }
            }
            return;
        }
    }

    public IEnumerator ShopKeeperAnimation()
    {
        shopKeeper.sprite = shopKeeperActive;
        yield return new WaitForSeconds(1f);
        shopKeeper.sprite = shopKeeperIdle;
    }
}  
