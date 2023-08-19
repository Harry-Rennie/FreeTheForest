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

    //spawns nodes at given positions
    public GameObject SpawnBossNode(Vector2 position, RectTransform container)
    {
        return SpawnSpecificNode(bossPrefab, position, container);
    }

    public GameObject SpawnBasicNode(Vector2 position, RectTransform container)
    {
        return SpawnSpecificNode(basicPrefab, position, container);
    }

    //spawns random(ish) node
    public GameObject SpawnRandomNode(Vector2 position, RectTransform container, List<GameObject> existingNodes)
    {
        GameObject prefab = GetRandomPrefab(existingNodes);
        return SpawnSpecificNode(prefab, position, container);
    }

    //instantiates node with given properties
    private GameObject SpawnSpecificNode(GameObject prefab, Vector2 position, RectTransform container)
    {
        GameObject node = Instantiate(prefab, container);
        RectTransform nodeTransform = node.GetComponent<RectTransform>();
        nodeTransform.anchoredPosition = position;
        SetNodeProperties(node);
        return node;
    }

    //returns prefab for node creation with spawn rules - needs to be rewritten.
    private GameObject GetRandomPrefab(List<GameObject> existingNodes)
    {
        //total nodes not force spawned
        int totalRegularNodes = existingNodes.Count - 4;

        //percentage of nodes to try spawn
        int desiredHeals = (int)(0.25 * existingNodes.Count);
        int desiredUpgrades = (int)(0.25 * existingNodes.Count);
        int healCount = existingNodes.Count(node => node.name.Contains("Heal"));
        int upgradeCount = existingNodes.Count(node => node.name.Contains("Upgrade"));

        List<GameObject> availablePrefabs = new List<GameObject>();

        //first node is always boss
        if (existingNodes.Count == 0)
        {
            return bossPrefab;
        }

        //at least one heal node and one upgrade node are spawned:
        if (healCount == 0)
        {
            availablePrefabs.Add(healPrefab);
        }
        if (upgradeCount == 0)
        {
            availablePrefabs.Add(upgradePrefab);
        }

        //ensure both node types are spawned if not already:
        //this is a todo.
        //at this point I realised we definitely need an overarching data structure to keep track of 'paths' nodes consist of, 
        //aswell as their row and column position in the grid, so that we can distribute nodes in desired way.
        //we can then run a comparison against a data set of encounter types to modify stats/difficulty/rewards as we see fit (dependent on the 'floor' or row of the dungeon).
        int nodesLeft = existingNodes.Count - 4;
        if (nodesLeft <= (desiredHeals + desiredUpgrades - healCount - upgradeCount))
        {
            if (healCount < desiredHeals && (existingNodes.Last().name != "Heal"))
            {
                availablePrefabs.Add(healPrefab);
            }
            if (upgradeCount < desiredUpgrades && (existingNodes.Last().name != "Upgrade"))
            {
                availablePrefabs.Add(upgradePrefab);
            }
        }

        //last 3 nodes are always basic nodes.
        if (nodesLeft <= 3)
        {
            return basicPrefab;
        }

        //if no specials left, then spawn a basic.
        if (availablePrefabs.Count == 0)
        {
            return basicPrefab;
        }

        int randomIndex = UnityEngine.Random.Range(0, availablePrefabs.Count);
        return availablePrefabs[randomIndex];
    }

    //sets properties to nodes after spawned
    private void SetNodeProperties(GameObject node)
    {
        RectTransform nodeTransform = node.GetComponent<RectTransform>();
        nodeTransform.sizeDelta = new Vector2(50, 50);
        nodeTransform.anchorMin = new Vector2(0, 0);
        nodeTransform.anchorMax = new Vector2(0, 0);
    }
}