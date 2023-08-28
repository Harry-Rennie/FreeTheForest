using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class NodeManager : MonoBehaviour
{
    [SerializeField] private GameObject bossPrefab;
    [SerializeField] private GameObject basicPrefab;
    [SerializeField] private GameObject healPrefab;
    [SerializeField] private GameObject upgradePrefab;
    [SerializeField] private LineManager lineManager;
    List<GameObject> nodesAddedSoFar = new List<GameObject>();
    private int totalNodes = 0;
    private int totalHealNodes = 0;
    private int totalUpgradeNodes = 0;

    public GameObject SpawnBossNode(Vector2 position, RectTransform container)
    {
        return SpawnSpecificNode(bossPrefab, position, container);
    }

    public GameObject SpawnBasicNode(Vector2 position, RectTransform container)
    {
        return SpawnSpecificNode(basicPrefab, position, container);
    }

    public GameObject SpawnRandomNode(Vector2 position, RectTransform container, List<Vector2> existingNodeLocations, int row, int col, List<List<Vector2?>> nodeGrid)
    {
        GameObject prefab = GetRandomPrefab();
        return SpawnSpecificNode(prefab, position, container);
    }

    public GameObject SpawnSpecificNode(GameObject initialPrefab, Vector2 position, RectTransform container)
    {
        GameObject prefabToSpawn = initialPrefab;

        Debug.Log($"Initial prefab: {initialPrefab.tag}");
        Debug.Log(nodesAddedSoFar.Count);
        if (nodesAddedSoFar.Count > 0)
        {
            //remove nulls from the list
            nodesAddedSoFar = nodesAddedSoFar.Where(n => n != null).ToList();

            if (initialPrefab.tag == "Heal" || initialPrefab.tag == "Upgrade")
            {
                GameObject closestUpperNode = lineManager.GetClosestUpperNode(initialPrefab, nodesAddedSoFar);
                if (closestUpperNode != null && closestUpperNode.tag == initialPrefab.tag)
                {
                    prefabToSpawn = GetRandomPrefab(false, initialPrefab.tag);  //change the prefab to spawn
                    Debug.Log($"Changed prefab to: {prefabToSpawn.tag}");
                }
            }
        }
        //create object on screen
        GameObject newNode = Instantiate(prefabToSpawn, container);
        RectTransform newNodeTransform = newNode.GetComponent<RectTransform>();
        newNodeTransform.anchoredPosition = position;
        SetNodeProperties(newNode);
        //keep track of the nodes so far.
        nodesAddedSoFar.Add(newNode);
        Debug.Log($"Spawned: {newNode.tag}");
        return newNode;
    }

    public GameObject GetRandomPrefab(bool allowRandomSelection = true, string excludeTag = null)
    {
        //update total number of nodes
        if (allowRandomSelection) totalNodes++;

        //define maximum number of special nodes based on total nodes
        int maxHealNodes = Mathf.FloorToInt(totalNodes * 0.2f);
        int maxUpgradeNodes = Mathf.FloorToInt(totalNodes * 0.2f);

        //start with the basic prefab in available options
        List<GameObject> availablePrefabs = new List<GameObject> { basicPrefab };

        //add heal prefab if conditions are met
        if (totalHealNodes < maxHealNodes)
        {
            availablePrefabs.Add(healPrefab);
        }

        //add upgrade prefab if conditions are met
        if (totalUpgradeNodes < maxUpgradeNodes)
        {
            availablePrefabs.Add(upgradePrefab);
        }

        //remove excluded prefab if needed
        if (excludeTag != null)
        {
            availablePrefabs.RemoveAll(x => x.tag == excludeTag);
        }

        // Randomly select a prefab from the available options
        int randomIndex = UnityEngine.Random.Range(0, availablePrefabs.Count);
        GameObject selectedPrefab = availablePrefabs[randomIndex];

        // Update the count of special nodes
        if (selectedPrefab == healPrefab)
        {
            totalHealNodes++;
        }
        else if (selectedPrefab == upgradePrefab)
        {
            totalUpgradeNodes++;
        }

        return selectedPrefab;
    }

    private void SetNodeProperties(GameObject node)
    {
        RectTransform nodeTransform = node.GetComponent<RectTransform>();
        nodeTransform.sizeDelta = new Vector2(50, 50);
        nodeTransform.anchorMin = new Vector2(0, 0);
        nodeTransform.anchorMax = new Vector2(0, 0);
    }
    
    public void CleanNodes()
    {
        foreach (var node in nodesAddedSoFar)
        {
            Destroy(node);
        }
    }
}