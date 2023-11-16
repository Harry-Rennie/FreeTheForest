using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RewardHandler : MonoBehaviour
{
    public PlayerInfoController gm;
    public List<CardDisplay> cards;
    
    // Start is called before the first frame update
    void Start()
    {
        gm = FindObjectOfType<PlayerInfoController>();
        GenerateRewards();
    }

    private void GenerateRewards()
    {
        List<Card> rewards = new List<Card>(); //Hold a reference list for checking duplicates

        for (int i = 0; i < 3; i++) //For our three cards...
        {
            bool chosen = false;

            while (!chosen)
            {
                Card card = gm.cardLibrary[Random.Range(0, gm.cardLibrary.Count)]; //Roll a random card from GameManager's CardLibrary list
                if (!rewards.Contains(card))
                {
                    chosen = true; //If we don't have it already, flag this loop as successful
                    rewards.Add(card); //Add that card to our reward list
                    cards[i].LoadCard(rewards[i]); //Load the reward card into the CardDisplay
                }
            }
            FormatCards();
        }
    }

    private void FormatCards()
    {
        foreach (CardDisplay card in cards)
        {
            if(card.descriptionText.text.Contains("{0}"))
            {
                card.descriptionText.text = card.descriptionText.text.Replace("{0}", "X");
            }
        }
    }
}
