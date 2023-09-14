using System;
using UnityEngine;
using UnityEngine.UI;

// this class handles the graphics and click events for a trinket slot

public class TrinketSlot : MonoBehaviour
{
    public Button TrinketButton;
    public Image TrinketImage;
    private Trinket trinket;
    [SerializeField]private Color pressedColour;
    [SerializeField]private Color normalColour;


    // Start is called before the first frame update
    void Start()
    {
        TrinketImage = GetComponent<Image>();
        TrinketButton = GetComponent<Button>();
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
}