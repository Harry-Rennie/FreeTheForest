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
    [SerializeField] private TMP_Text playerStats;
    private PlayerInfoController playerInfoController;
    private TrinketManager trinketManager;

    #region Singleton
    // singleton pattern
    public static PlayerInfoPanel Instance;
    private void Awake()
    {
        if (Instance != null)
        {
            Debug.LogWarning("More than one instance of PlayerInfoPanel found.");
            return;
        }
        Instance = this;
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
        playerStats.text = @$"<b>Health:</b>      {playerInfoController.CurrentHealth}/{playerInfoController.MaxHealth} {getBuffString(trinketManager.TotalHealthBuff)}
<b>Strength:</b>   {playerInfoController.Strength} {getBuffString(trinketManager.TotalStrengthBuff)}
<b>Defense:</b>    {playerInfoController.Defense} {getBuffString(trinketManager.TotalDefenseBuff)}";
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
        SceneManager.LoadScene("TitleScreen");
    }
    #endregion
}