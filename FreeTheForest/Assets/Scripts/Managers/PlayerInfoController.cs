using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class PlayerInfoController : MonoBehaviour
{
    public TMP_Text m_TextMeshPro;
    public static PlayerInfoController instance;
    public List<Card> playerDeck = new List<Card>(); //Holds the players current deck as a list of cards.

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
