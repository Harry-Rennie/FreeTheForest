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
        TitleText.text = card.title;
        DescriptionText.text = card.description;
        ManaText.text = card.manaCost.ToString();
    }
}