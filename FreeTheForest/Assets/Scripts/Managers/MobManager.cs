using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// This class contains fields for a battle template.
/// A battle template is a list of rows and a list of enemies.
/// The rows determine the rows of the overworld tree this template can appear on.
/// The class is intended to be stored in a list of battle templates in the MobManager.
/// The templates are populated in the editor.
/// </summary>
[Serializable]
public class BattleTemplate
{
    public List<int> rows;
    public List<GameObject> enemies;
}

/// <summary>
/// This class is responsible for selecting a battle template.
/// </summary>
public class MobManager : MonoBehaviour
{

    [SerializeField]
    public List<BattleTemplate> BattleTemplates;
    public List<List<List<GameObject>>> BattleGrid;

    void Start()
    {
        BattleGrid = GenBattleGrid(5, 5);
    }

    /// <summary>
    /// This function generates a battle for a given row
    /// </summary>
    /// <param name="row">The row to generate a battle for</param>
    /// <returns>A list of enemies</returns>
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

    public List<List<List<GameObject>>> GenBattleGrid(int x, int y)
    {
        List<List<List<GameObject>>> grid = new List<List<List<GameObject>>>();
        // loop through rows
        for (int i = 0; i < y; i++)
        {
            List<List<GameObject>> row = new List<List<GameObject>>();
            // loop through possible nodes
            for (int j = 0; j < x; j++)
            {
                // generate battle suitable for the row
                List<GameObject> enemies = GenBattle(i);
                // add battle to row
                row.Add(enemies);
            }
            // add row to grid
            grid.Add(row);
        }
        return grid;
    }

}
