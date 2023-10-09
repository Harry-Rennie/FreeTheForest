using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using UnityEngine;

/// <summary>
/// This class contains fields for a battle template.
/// A battle template is a list of rows and a list of enemies.
/// MinRow and MaxRow are the minimum and maximum rows that the enemies can appear in.
/// The class is intended to be stored in a list of battle templates in the MobManager.
/// The templates are populated in the editor.
/// </summary>
[Serializable]
public class BattleTemplate
{
    public int MinRow, MaxRow;
    public List<Enemy> enemies;
}

/// <summary>
/// This class is responsible for selecting a battle template.
/// </summary>
public class MobManager : MonoBehaviour
{

    [SerializeField]
    public List<BattleTemplate> BattleTemplates;
    public List<List<List<Enemy>>> BattleGrid { get; private set; }



    /// <todo>
    /// This should be replaced with a function that gets the maximum x and y values of the overworld tree
    /// </todo>
    private int overworldX = 1;
    private int overworldY = 5;

        #region Singleton
    // singleton pattern
    public static MobManager Instance;
    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
        {
            Destroy(gameObject);
            return;
        }
        DontDestroyOnLoad(gameObject);
    }
    #endregion

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
    public List<Enemy> GenBattle(int row)
    {
        List<Enemy> enemies = new List<Enemy>();
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
    public List<List<List<Enemy>>> GenBattleGrid(int x, int y)
    {
        List<List<List<Enemy>>> grid = new List<List<List<Enemy>>>();
        // loop through rows
        for (int i = 0; i < y; i++)
        {
            List<List<Enemy>> row = new List<List<Enemy>>();
            // loop through possible nodes
            for (int j = 0; j < x; j++)
            {
                // generate battle suitable for the row
                List<Enemy> enemies = GenBattle(i);
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
            foreach (List<Enemy> battle in BattleGrid[i])
            {
                string enemies = string.Join(", ", battle.Select(enemy => enemy.name)) + " | ";
                battleString += enemies;
            }
            gridString += battleString + "\n";
        }
        Debug.Log(gridString);
    }
}
