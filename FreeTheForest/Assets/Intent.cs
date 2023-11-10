using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System;

public class Intent : MonoBehaviour
{

    //I will implement more intents if I get time. Intent Icon - Card effect key word // edge case for multiple enemy intents
    [SerializeField] GameObject attackIcon;
    [SerializeField] GameObject blockIcon;
    [SerializeField] GameObject attackAmount;
    [SerializeField] GameObject blockAmount;
    [SerializeField] TMP_Text attackText;
    [SerializeField] TMP_Text blockText;
    private BattleManager _battleManager;

    private Entity _enemy;

private List<double> attackModes = new List<double> {0, 0.7};
private List<double> blockModes = new List<double> {0, 0.7};

    // Start is called before the first frame update
    void Start()
    {
        _battleManager = FindObjectOfType<BattleManager>();
        _enemy = this.GetComponentInParent<Entity>();
        Debug.Log("Enemy Strength: " + _enemy.strength + " Enemy Defence: " + _enemy.defence);
    }

    //enemies play cards, get their card and enable/disable intent per turn.
    public void ModifyIntent(Card card)
    {
        if(_enemy.currentHealth <= 0 || _enemy == null)
        {
            return;
        }
        //needs to get the intent too
        if (card.cardType == Card.CardType.Attack)
        {
            attackIcon.SetActive(true);
            blockIcon.SetActive(false);
            blockAmount.SetActive(false);
            attackAmount.SetActive(true);
            //index of position of list where effects =='Attack';

            int x = card.effects.IndexOf(Card.CardEffect.Attack);
            attackModes[0] = _enemy.strength;
            double mode = attackModes[x];
            if(x == 0)
            {
                int value =  _enemy.strength;
                attackText.text = value.ToString();
            }
            else
            {
                int value = Convert.ToInt32(mode * _enemy.strength);
                attackText.text = value.ToString();
            }
        }
        else if (card.cardType == Card.CardType.Skill)
        {
            attackIcon.SetActive(false);
            attackAmount.SetActive(false);
            blockIcon.SetActive(true);
            blockAmount.SetActive(true);

            int x = card.effects.IndexOf(Card.CardEffect.Attack);
            double mode = blockModes[x];
            blockModes[0] = _enemy.defence;
            if(x == 0)
            {
                int value =  _enemy.defence;
                blockText.text = value.ToString();
            }
            else
            {
                int value = Convert.ToInt32(mode * _enemy.defence);
                blockText.text = value.ToString();
            }
        }
    }

    public void DisableIntent()
    {
        attackIcon.SetActive(false);
        attackAmount.SetActive(false);
        blockIcon.SetActive(false);
        blockAmount.SetActive(false);
    }
}
