using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Node : MonoBehaviour
{
    [SerializeField] private GameObject node;
    PlayerInfoController gameManager;

    private void Awake()
    {
        gameManager = PlayerInfoController.instance;
    }
    public void NodeClicked()
    {
        Debug.Log("Node Completed");
        gameManager.floorNumber++;
        gameManager.SetCurrentEnemies();
        gameManager.SetLastNodeVisited(node);
    }
}
