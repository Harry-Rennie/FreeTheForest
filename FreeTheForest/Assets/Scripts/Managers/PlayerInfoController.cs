using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;
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

    private int _Strength;
    public int Strength
    {
        get => _Strength;
        set
        {
            _Strength = value;
            OnStrengthChanged?.Invoke();
        }
    }

    private int _Defence;
    public int Defence
    {
        get => _Defence;
        set
        {
            _Defence = value;
            OnDefenceChanged?.Invoke();
        }
    }

    private int _Energy;
    public int Energy
    {
        get => _Energy;
        set
        {
            _Energy = value;
            OnEnergyChanged?.Invoke();
        }
    }
    public int Gold;
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

    public event Action OnStrengthChanged;
    public event Action OnDefenceChanged;
    public event Action OnEnergyChanged;
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

    public void ModifyDefence(int amount)
    {
        Defence += amount;
        OnDefenceChanged?.Invoke();
    }

    public void ModifyStrength(int amount)
    {
        Strength += amount;
        OnStrengthChanged?.Invoke();
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
        Defence = 1;
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
