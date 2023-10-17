using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;

/// <summary>
/// This class controls the functionality of the trinket slots in the player info panel.
/// </summary>
public class TrinketSlot : MonoBehaviour, IPointerClickHandler
{
    [SerializeField] private GameObject trinketInfoPanel;
    [SerializeField] private Color unequippedColour, equippedColour;
    [SerializeField] private Image trinketImage;
    private Trinket trinket;

    /// <summary>
    /// This method sets the trinket of the trinket slot.
    /// </summary>
    /// <param name="trinket">The trinket to be set.</param>
    public void SetTrinket(Trinket trinket)
    {
        this.trinket = trinket;
        trinketImage.sprite = trinket.Sprite;
        // update right click info panel
        trinketInfoPanel.GetComponentInChildren<TMP_Text>().text = trinket.ToString();
        // update colour of trinket image based on equipped status
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
        //raycast down and try find trinketslot game object
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