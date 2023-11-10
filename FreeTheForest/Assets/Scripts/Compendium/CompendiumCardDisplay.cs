using UnityEngine;
using TMPro;

/// <summary>
/// This class displays the card data on the compendium card prefab.
/// </summary>
/// <todo>
/// This will eventually need to display the card image also.
/// </todo>
public class CompendiumCard : MonoBehaviour
{
    [Header("Card UI Elements")]
    public TextMeshProUGUI TitleText;
    public TextMeshProUGUI DescriptionText;
    public TextMeshProUGUI ManaText;

    /// <summary>
    /// This function displays the card data on the compendium card prefab.
    /// </summary>
    /// <param name="card">The card to display.</param>
    public void DisplayCard(Card card)
    {
        if(card.description.Contains("{0}") && card.cardType == Card.CardType.Attack)
        {
            DescriptionText.text = card.description.Replace("{0}", "X");   
        }
        if(card.description.Contains("{0}") && card.cardType == Card.CardType.Skill)
        {
            DescriptionText.text = card.description.Replace("{0}", "X");   
        }
        if(!card.description.Contains("{0}"))
        {
            DescriptionText.text = card.description;
        }
        TitleText.text = card.title;
        ManaText.text = card.manaCost.ToString();
    }
}