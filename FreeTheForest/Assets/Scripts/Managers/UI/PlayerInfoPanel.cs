using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;

/// <summary>
/// This class controls the functionality of the player info panel.
/// </summary>
public class PlayerInfoPanel : MonoBehaviour
{
    public List<TrinketSlot> TrinketSlots;
    [SerializeField] public TMP_Text strength;
    [SerializeField] public TMP_Text defence;
    [SerializeField] public TMP_Text gold;
    [SerializeField] public TMP_Text energy;
    [SerializeField] public TMP_Text health;
    [SerializeField] public TMP_Text floorNumber;

    [SerializeField] public Button saveButton;
    [SerializeField] public Button noSave;
    [SerializeField] public GameObject savePanel;
    [SerializeField] public GameObject settingsPanel;

    [SerializeField] public Button settingsButton;
    [SerializeField] public Sprite settingsPressed;
    [SerializeField] public Sprite settingsUnpressed;

    private PlayerInfoController playerInfoController;
    private TrinketManager trinketManager;

    #region Singleton
    // singleton pattern
    public static PlayerInfoPanel Instance;
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else if(Instance != null)
        {
          //  Debug.LogWarning("More than one instance of PlayerInfoPanel found.");
            return;
        }
    }
    #endregion

    // Start is called before the first frame update
    void Start()
    {
        // assign static managers to local variables
        playerInfoController = PlayerInfoController.instance;
        trinketManager = TrinketManager.Instance;
        // update the player info panel
        UpdateTrinkets();
        UpdateStats();
    }

    /// <summary>
    /// This method updates the player stats in the player info panel.
    /// It formats stats from playerInfoController and trinketManager into a string.
    /// </summary>
    public void UpdateStats()
    {
        health.text = @$"{playerInfoController.CurrentHealth}/{playerInfoController.MaxHealth}";
        strength.text = @$"{playerInfoController.Strength}";
        defence.text = @$"{playerInfoController.Defence}";
        gold.text = @$"{playerInfoController.Gold}";
        energy.text = @$"{playerInfoController.Energy}";
        //we dont want to visually display 0 based floors
        floorNumber.text = @$"{FormatFloorNumber()}";
    }

    public string FormatFloorNumber()
    {
        if (playerInfoController.floorNumber < 10)
        {
            return "0" + @$"{playerInfoController.floorNumber + 1}";
        }
        else
        {
            return playerInfoController.floorNumber.ToString();
        }
    }

    public void UpdateGold()
    {
        gold.text = @$"{playerInfoController.Gold}";
    }

    /// <summary>
    /// This method updates the trinkets in the player info panel based on the player trinkets in trinketManager.
    /// </summary>
    public void UpdateTrinkets()
    {
        for (int i = 0; i < TrinketSlots.Count; i++)
        {
            // if the index is less than the number of player trinkets, set the trinket of the trinket slot
            if (i < trinketManager.PlayerTrinkets.Count)
            {
                TrinketSlots[i].SetTrinket(trinketManager.PlayerTrinkets[i]);
            }
        }
    }

    /// <summary>
    /// This method returns a string that describes the buff or debuff to the stat.
    /// A positive buff is green, a negative buff is red.
    /// </summary>
    private string getBuffString(int buff)
    {
        string buffString = "";
        if (buff != 0)
        {
            buffString = (buff > 0) ? $"<color=#18ba30>({buff})</color>" : $"<color=#FF0000>({buff})</color>";
            return buffString;
        }
        return buffString;
    }

    #region Button Panel Methods
    /// <summary>
    /// This method is called when the "Main Menu" button is clicked.
    /// </summary>
    public void MainMenu()
    {
        DeactivateUI();
        //reset game
        playerInfoController.ResetPlayerInfo();
        SceneManager.LoadScene("TitleScreen");
    }
    public void Quit()
    {
        Application.Quit();
    }
    public void ShowSavePanel()
    {
        savePanel.SetActive(true);
    }
    public void SaveOption()
    {
        savePanel.SetActive(false);
        MainMenu();
    }

    public void NoSaveOption()
    {
        savePanel.SetActive(false);
        MainMenu();
    }

    public void ShowSettingsPanel()
    {
        settingsPanel.SetActive(true);
    }
    #endregion
    /// <summary>
    /// Deactivates PlayerUI when in a scene that doesn't need it. Todo: reset stats properly for new game
    /// </summary>
    public void DeactivateUI()
    {
        gameObject.SetActive(false);
    }

    public void ActivateUI()
    {
        gameObject.SetActive(true);
    }
}