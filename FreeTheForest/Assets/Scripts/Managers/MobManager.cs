using System;
using System.Collections;
using System.Collections.Generic;
//using System.Diagnostics;
using UnityEngine;

[Serializable]
public class BattleTemplate
{
    public List<int> rows;
    public List<GameObject> enemies;
}

public class MobManager : MonoBehaviour
{

    [SerializeField]
    public List<BattleTemplate> BattleTemplates;

    [SerializeField]
    public int test;

    // Start is called before the first frame update
    void Start()
    {
        GenBattleGrid(5, 5);
    }

    public List<GameObject> GenBattle(int row)
    {
        List<GameObject> enemies = new List<GameObject>();
        List<BattleTemplate> possibleTemplates = new List<BattleTemplate>();
        foreach (BattleTemplate template in BattleTemplates)
        {
            if (template.rows.Contains(row))
            {
                possibleTemplates.Add(template);
            }
        }
        BattleTemplate selectedTemplate = possibleTemplates[UnityEngine.Random.Range(0, possibleTemplates.Count)];
        foreach (GameObject enemy in selectedTemplate.enemies)
        {
            enemies.Add(enemy);
        }
        return enemies;
    }

    public void GenBattleGrid(int x, int y)
    {
        // loop through rows
        for (int i = 0; i < y; i++)
        {
            // loop through possible nodes
            for (int j = 0; j < x; j++)
            {
                // generate battle suitable for the row
                List<GameObject> enemies = GenBattle(i);
                string enemyList = "Row " + i + " Node " + j + ": ";
                // print enemies on a single line
                foreach (GameObject enemy in enemies)
                {
                    enemyList += enemy.name + " ";
                }
                // log enemy list
                Debug.Log(enemyList);
            }
        }
    }
}
