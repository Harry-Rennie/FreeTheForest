using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
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
    public int MinRow, MaxRow;
    public List<GameObject> enemies;
}

/// <summary>
/// This class is responsible for selecting a battle template.
/// </summary>
public class MobManager : MonoBehaviour
{

    [SerializeField]
    public List<BattleTemplate> BattleTemplates;
    public List<List<List<GameObject>>> BattleGrid { get; private set; }


    /// <todo>
    /// This should be replaced with a function that gets the maximum x and y values of the overworld tree
    /// </todo>
    private int overworldX = 5;
    private int overworldY = 5;

    void Start()
    {
        BattleGrid = GenBattleGrid(overworldX, overworldY);
        printGrid();
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
            // if the row is within the range of the template
            if (row >= template.MinRow && row <= template.MaxRow)
            {
                // add the template to the list of possible templates
                possibleTemplates.Add(template);
            }
        }

        if (possibleTemplates.Count > 0)
        {
            // select a random template from the list of possible templates
            BattleTemplate selectedTemplate = possibleTemplates[UnityEngine.Random.Range(0, possibleTemplates.Count)];
            // add the enemies from the selected template to the list of enemies for the battle
            enemies.AddRange(selectedTemplate.enemies);
        }

        return enemies;
    }

    /// <summary>
    /// This function generates a grid of mobs corresponding to the maximum size of the overworld tree
    /// </summary>
    /// <param name="x">The maximum x value of the overworld tree</param>
    /// <param name="y">The maximum y value of the overworld tree</param>
    /// <returns> A 3D list of mobs
    /// The first dimension is a list of rows
    /// The second dimension is a list of battles
    /// The third dimension is a list of enemies for each battle
    ///</returns>
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

    /// <summary>
    /// This function prints the battle grid, nicely formatted
    /// </summary>
    private void printGrid()
    {
        string gridString = "";
        // for each row
        for (int i = 0; i < BattleGrid.Count; i++)
        {
            string battleString = "";
            gridString += "Row " + i + ":\n";
            // for each battle
            foreach (List<GameObject> battle in BattleGrid[i])
            {
                string enemies = string.Join(", ", battle.Select(enemy => enemy.name)) + " | ";
                battleString += enemies;
            }
            gridString += battleString + "\n";
        }
        Debug.Log(gridString);
    }
}
