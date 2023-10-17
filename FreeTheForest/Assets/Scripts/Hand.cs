using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Hand : MonoBehaviour
{
    public List<Card> _heldCards;

    //the parent canvas of each card within the 'Cards' canvas (the hand)
    [SerializeField] public List<GameObject> cardSlots = new List<GameObject>();
    private int prevCount;
    BattleManager battleManager;

    public List<Card> heldCards
    {
        get { return _heldCards; } // Getter to access the cardsInHand list
        set
        {
            if (_heldCards.Count != value.Count)
            {
                // Perform the action you want when cardsInHand changes size here
                Debug.Log("heldCards size changed");
                // You can also check if the new size is larger or smaller if needed.
            }

            _heldCards = value; // Set the new value
        }
    }
    public void Start()
    {
        battleManager = FindObjectOfType<BattleManager>();
        prevCount = _heldCards.Count;
        //all of the active objects in each card slot
    }

    public void SortHand()
    {
    }

    public void BeginTargeting()
    {

    }
}
