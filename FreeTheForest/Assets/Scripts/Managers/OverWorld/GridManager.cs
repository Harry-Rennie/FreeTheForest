using UnityEngine;

public class GridManager : MonoBehaviour
{
    [SerializeField] private int _width, _height;
    [SerializeField] private GameObject _tile;
    [SerializeField] private int _yOffset = 0;
    [SerializeField] private int _xOffset = 0;
    [SerializeField] GameObject decisionTreePanel; // Reference to the DecisionTreePanel GameObject

    //I am aiming to randomly generate encounters (at this point buttons) inside of the grid.
    private void Start()
    {
        GenerateGrid();
    }
    void GenerateGrid()
    {
        for (int x = 0; x < _width; x++)
        {
            for (int y = 0; y < _height; y++)
            {
                var spawnedTile = Instantiate(_tile, new Vector3(x + _xOffset,y + _yOffset), Quaternion.identity);
                // Set the DecisionTreePanel as the parent of the spawned tile
                spawnedTile.transform.SetParent(decisionTreePanel.transform);
                spawnedTile.name = $"Cell {x}, {y}";
            }
        }
    }
}