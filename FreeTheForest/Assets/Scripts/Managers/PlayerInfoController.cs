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
    public GameObject LastNodeVisited { get; set; }
    public Vector2 lastPosition;
    public int EnemyCount;

    [SerializeField] private GraphLayoutManager graphLayoutManager;

    //serialized while testing
    [SerializeField] public int floorNumber = 0;
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
    public void SetLastNodeVisited(GameObject node)
    {
        LastNodeVisited = node;
        lastPosition = node.GetComponent<RectTransform>().anchoredPosition;
    }

    public GameObject GetLastNodeVisited()
    {
        return LastNodeVisited;
    }

    public void SetCurrentEnemies()
    {
        currentEnemies = MobManager.Instance.BattleGrid[floorNumber][0];
        EnemyCount = currentEnemies.Count;
    }
}
