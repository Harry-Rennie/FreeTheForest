using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class CardDisplay : MonoBehaviour
{
    public Card card;

    public TextMeshProUGUI nameText;
    public TextMeshProUGUI descriptionText;
    public TextMeshProUGUI manaCost;
    BattleManager battleManager;

    private void Awake()
    {
        battleManager = FindObjectOfType<BattleManager>();
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    public void LoadCard(Card _card)
    {
        card = _card;
        nameText.text = card.title;
        descriptionText.text = card.description;
        manaCost.text = card.manaCost.ToString();
    }

    public void SelectCard()
    {
        battleManager.selectedCard = this;
    }

    public void DeselectCard()
    {
        battleManager.selectedCard = null;
    }

    public void HandleEndDrag()
    {
        if (battleManager.energy < card.manaCost)
            return;

        battleManager.PlayCard(this);
    }

}
