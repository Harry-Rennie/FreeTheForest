using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
/// <summary>
/// This class is used to store the player's information. It is used to pass information between scenes.
/// </summary>
public class PlayerInfoController : MonoBehaviour
{
    private Canvas canvas;
    public TMP_Text m_TextMeshPro;
    public static PlayerInfoController instance;
    public int MaxHealth;
    public int CurrentHealth;
    public int Strength;
    public int Defense;
    public int Gold;
    public int Energy;
    public GameObject LastNodeVisited { get; set; }
    public Vector2 lastPosition;
    public int EnemyCount;
    public float lastScrollPosition {get; set;}
    public Image p_healthBar;
    public Image p_damageBar;
    public TMP_Text p_healthText;

    [SerializeField] private GraphLayoutManager graphLayoutManager;

    //serialized while testing
    [SerializeField] public int floorNumber = 0;
    public List<Card> playerDeck = new List<Card>(); //Holds the players current deck as a list of cards.
    public List<Card> cardLibrary = new List<Card>(); //Holds all possible reward cards.
    
    public List<Enemy> currentEnemies = new List<Enemy>(); //Battle reads from here to load in enemies to battle scene.
    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            canvas = GetComponent<Canvas>();
        }
        else
        {
            Destroy(gameObject);
            return;
        }
        DontDestroyOnLoad(gameObject);
    }

    void Start()
    {
        ResetPlayerInfo();
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

    public void SetHealthBar()
    {
        p_healthBar.fillAmount = (float) CurrentHealth/MaxHealth;
        p_healthText.text = CurrentHealth.ToString() + "/" + MaxHealth.ToString();
    }

    public void deactivateHealthBar()
    {
        p_healthBar.gameObject.SetActive(false);
        p_damageBar.gameObject.SetActive(false);
        p_healthText.gameObject.SetActive(false);
    }

    public void activateHealthBar()
    {
        GameObject healthBar = GameObject.Find("PlayerHealthBar");
        p_healthBar = healthBar.transform.GetChild(1).GetComponent<Image>();
        p_damageBar = healthBar.transform.GetChild(0).GetComponent<Image>();
        p_healthText = healthBar.transform.GetChild(2).GetComponent<TMP_Text>();
        p_damageBar.gameObject.SetActive(true);
        p_healthBar.gameObject.SetActive(true);
        p_healthText.gameObject.SetActive(true);
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

    public void ResetPlayerInfo()
    {
        MaxHealth = 100;
        CurrentHealth = 100;
        Strength = 5;
        Defense = 1;
        Gold = 25;
        Energy = 3;
        floorNumber = 0;
    }

    public void GetCamera() //set the camera to whatever camera is in scene. (loses reference)
    {
        Camera mainCamera = Camera.main;
        if (mainCamera != null && canvas != null)
        {
            canvas.worldCamera = mainCamera;
        }
    }
}
