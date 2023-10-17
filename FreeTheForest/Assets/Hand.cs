using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Hand : MonoBehaviour
{
    public List<CardDisplay> _heldCards = new List<CardDisplay>();
    public List<Card> cardsInHand;

    //the parent canvas of each card within the 'Cards' canvas (the hand)
    [SerializeField] List<GameObject> cardSlots = new List<GameObject>();
    private int prevCount;
    BattleManager battleManager;
    public void Start()
    {
        battleManager = FindObjectOfType<BattleManager>();
        prevCount = _heldCards.Count;
    }

    public void SortHand()
    {
    }
}
