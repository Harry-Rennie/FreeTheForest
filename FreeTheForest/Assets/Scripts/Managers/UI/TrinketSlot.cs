using System;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;

/// <summary>
/// This class controls the functionality of the trinket slots in the player info panel.
/// </summary>
public class TrinketSlot : MonoBehaviour, IPointerClickHandler
{
    private Image trinketImage;
    private Trinket trinket;

    [SerializeField] private GameObject trinketInfoPanel;

    [SerializeField] private Color unequippedColour;
    [SerializeField] private Color equippedColour;


    // Start is called before the first frame update
    void Awake()
    {
        // assign the image component to the trinketImage variable
        trinketImage = GetComponent<Image>();
    }

    /// <summary>
    /// This method sets the trinket of the trinket slot.
    /// </summary>
    /// <param name="trinket">The trinket to be set.</param>
    public void SetTrinket(Trinket trinket)
    {
        //trinketImage = GetComponent<Image>();
        this.trinket = trinket;
        trinketImage.sprite = trinket.Sprite;
        trinketInfoPanel.GetComponentInChildren<TMP_Text>().text = trinket.ToString();
        trinketImage.color = (trinket.Equipped) ? equippedColour : unequippedColour;
    }

    /// <summary>
    /// This method toggles the equipped status of the trinket.
    /// It also updates the colour of the trinket image based on the equipped status.
    /// </summary>
    private void toggleEquipped()
    {
        if (trinket.Equipped)
        {
            TrinketManager.Instance.UnequipTrinket(trinket);
            trinketImage.color = unequippedColour;
        }
        else
        {
            TrinketManager.Instance.EquipTrinket(trinket);
            trinketImage.color = equippedColour;
        }
        PlayerInfoPanel.Instance.UpdateStats();
    }


    /// <summary>
    /// This method is called when the trinket slot is clicked.
    /// It utilises the IPointerClickHandler interface.
    /// </summary>
    /// <param name="eventData">The pointer event data.</param>
    public virtual void OnPointerClick(PointerEventData eventData)
    {
        // if the trinket is null, return
        if (trinket == null)
        {
            return;
        }
        // if the left mouse button is clicked
        if (eventData.button == PointerEventData.InputButton.Left)
        {
            // toggle the equipped status of the trinket
            toggleEquipped();
        }
        // if the right mouse button is clicked
        else if (eventData.button == PointerEventData.InputButton.Right)
        {         
            // check if any other panels are open in PlayerInfoPanel.instance.TrinketSlots
            // if so, close them
            foreach (TrinketSlot slot in PlayerInfoPanel.Instance.TrinketSlots)
            {
                if (slot.trinketInfoPanel.activeSelf && slot != this)
                {
                    slot.trinketInfoPanel.SetActive(false);
                }
            }
            // toggle the trinket info panel
            trinketInfoPanel.SetActive(!trinketInfoPanel.activeSelf);           
        }
    }
}