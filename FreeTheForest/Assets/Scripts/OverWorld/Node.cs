using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Node : MonoBehaviour
{
    [SerializeField] private GameObject node;
    PlayerInfoController gameManager;
    Camera mainCamera;
    Canvas nodeCanvas;

    private void Awake()
    {
        gameManager = PlayerInfoController.instance;
        mainCamera = Camera.main;
        nodeCanvas = GetComponentInChildren<Canvas>();
        if (nodeCanvas.renderMode == RenderMode.WorldSpace)
        {
            nodeCanvas.worldCamera = mainCamera;
        }
    }
    public void NodeClicked()
    {
        Debug.Log("Node Completed");
        gameManager.floorNumber++;
        gameManager.SetCurrentEnemies();
        gameManager.SetLastNodeVisited(node);
    }
}
