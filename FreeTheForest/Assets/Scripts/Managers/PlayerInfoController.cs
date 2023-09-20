using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
/// <summary>
/// This class is used to store the player's information. It is used to pass information between scenes.
/// </summary>
public class PlayerInfoController : MonoBehaviour
{
    public TMP_Text m_TextMeshPro;
    public static PlayerInfoController instance;
    public int MaxHealth;
    public int CurrentHealth;
    public int Strength;
    public int Defense;
    public List<Card> playerDeck = new List<Card>(); //Holds the players current deck as a list of cards.
    public List<Card> cardLibrary = new List<Card>(); //Holds all possible reward cards.
    
    public List<Enemy> currentEnemies = new List<Enemy>(); //Battle reads from here to load in enemies to battle scene.
    private void Awake()
    {
        if (instance == null)
            instance = this;
        else
        {
            Destroy(gameObject);
            return;
        }
        DontDestroyOnLoad(gameObject);
    }

    void Start()
    {
    }

    public void GainHealth(int amount)
    {

    }

    public void LoseHealth(int amount)
    {

    }

    public int HealNode(float percentageHeal)
    {
        int amountToHeal = Mathf.RoundToInt(percentageHeal * MaxHealth);
        CurrentHealth += amountToHeal;
        if (CurrentHealth > MaxHealth)
            CurrentHealth = MaxHealth;
        
        PlayerInfoPanel.Instance.UpdateStats();
        return amountToHeal;
    }
}
