using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

/// <summary>
/// This class is responsible for holding the player's deck and health.
/// </summary>
public class PlayerInfoController : MonoBehaviour
{
    public TMP_Text m_TextMeshPro;
    public static PlayerInfoController instance;
    public List<Card> playerDeck = new List<Card>(); //Holds the players current deck as a list of cards.

    public List<Enemy> currentEnemies = new List<Enemy>(); //Battle reads from here to load in enemies to battle scene.

    public int PlayerHealth = 100;

    # region SINGLETON
    // This is the singleton pattern
    // This pattern makes it so that there can only be one instance of this class.
    // Here is an example of how to reference PlayerHealth from another script:
    // PlayerInfoController.Instance.PlayerHealth
    public static PlayerInfoController Instance
    {
        get
        {
            if (instance == null)
            {
                instance = FindObjectOfType<PlayerInfoController>();
                if (instance == null)
                {
                    GameObject go = new GameObject();
                    go.name = typeof(PlayerInfoController).Name;
                    instance = go.AddComponent<PlayerInfoController>();
                    DontDestroyOnLoad(go);
                }
            }
            return instance;
        }
    }
    #endregion
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
        // You can perform other initialization logic here if needed.
    }
}


