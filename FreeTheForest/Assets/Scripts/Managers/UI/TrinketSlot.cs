using System;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;

// this class handles the graphics and click events for a trinket slot

public class TrinketSlot : MonoBehaviour, IPointerClickHandler
{
    public Button TrinketButton;
    public Image TrinketImage;
    private Trinket trinket;

    [SerializeField]private GameObject trinketInfoPanel;

    [SerializeField]private Color pressedColour;
    [SerializeField]private Color normalColour;


    // Start is called before the first frame update
    void Start()
    {
        TrinketImage = GetComponent<Image>();
        TrinketButton = GetComponent<Button>();
        //trinketInfoPanel = GetComponent<Panel>();
        TrinketButton.onClick.AddListener(OnClick);
        TrinketButton.onClick.AddListener(changeColour);
    }

    public void SetTrinket(Trinket trinket)
    {
        this.trinket = trinket;
        TrinketImage.sprite = trinket.Sprite;
    }

    public void OnClick()
    {
        if (trinket.Equipped)
        {
            Debug.Log("Unequipping trinket " + trinket.Title + "...");
            TrinketManager.instance.UnequipTrinket(trinket);
        }
        else
        {
            Debug.Log("Equipping trinket " + trinket.Title + "...");
            TrinketManager.instance.EquipTrinket(trinket);
        }
        PlayerInfoPanel.instance.UpdateStats();
        //PlayerInfoController.instance.UpdateTrinkets();
    }

      private void changeColour()
    {
        // Check if the button is already in the pressed color state
        if (TrinketButton.image.color == pressedColour)
        {
            TrinketButton.image.color = normalColour;
        }
        else
        {
            TrinketButton.image.color = pressedColour;
        }
    }
    
    public virtual void OnPointerClick(PointerEventData eventData)
{
    if (eventData.button == PointerEventData.InputButton.Right)
        {
            Debug.Log("Right click");
            // toggle the trinket info panel
            // check if any other panels are open in PlayerInfoPanel.instance.TrinketSlots
            // if so, close them
            foreach (TrinketSlot slot in PlayerInfoPanel.instance.TrinketSlots)
            {
                if (slot.trinketInfoPanel.activeSelf && slot != this)
                {
                    slot.trinketInfoPanel.SetActive(false);
                }
            }
            trinketInfoPanel.SetActive(!trinketInfoPanel.activeSelf);
            trinketInfoPanel.GetComponentInChildren<TMP_Text>().text = trinket.ToString();
        }
}
}