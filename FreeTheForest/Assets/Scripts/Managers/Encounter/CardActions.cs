using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CardActions : MonoBehaviour
{
    Card card;
    public Entity target;
    public Entity player;
    BattleManager battleManager;

    private void Awake()
    {
        battleManager = GetComponent<BattleManager>();
    }

    public void PerformAction(Card _card, Entity _entity)
    {
        card = _card;
        target = _entity;

        switch (card.title)
        {
            case "Strike":
                AttackEnemy();
                break;
            case "Defend":
                PerformBlock();
                break;
            default:
                Debug.Log("Something gone wrong with Performing Action");
                break;
        }
    }

    private void AttackEnemy()
    {
        int damage = player.offense;
        
        target.TakeDamage(damage);
    }

    private void PerformBlock()
    {
        int block = player.defense;

        player.AddBlock(block);
    }
}
