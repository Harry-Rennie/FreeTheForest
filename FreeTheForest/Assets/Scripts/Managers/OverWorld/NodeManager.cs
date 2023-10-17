using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Unity.UI;

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
        //create object on screen
        GameObject newNode = Instantiate(prefabToSpawn, container);
        RectTransform newNodeTransform = newNode.GetComponent<RectTransform>();
        newNodeTransform.anchoredPosition = position;
        SetNodeProperties(newNode);
        //keep track of the nodes so far.
        nodesAddedSoFar.Add(newNode);
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

        int randomIndex = UnityEngine.Random.Range(0, availablePrefabs.Count);
        GameObject selectedPrefab = availablePrefabs[randomIndex];

        //update the count of special nodes
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
        Button nodeButton = node.GetComponent<UnityEngine.UI.Button>();
        nodeButton.interactable = false;
    }
    
    //this is dirty. the method checks the nodes after they have been spawned, and replaces the ones causing undesired consecutive spawning. Ideally this is done before.
    public GameObject Respawn(GameObject node, RectTransform container, string tag)
    {
        //get position of node
        RectTransform nodeTransform = node.GetComponent<RectTransform>();
        Vector2 position = nodeTransform.anchoredPosition;
        //grab the lines that are children of the object
        List<GameObject> lines = lineManager.GetLinesFromNode(node);
        //spawn a new node
        GameObject newNode = SpawnSpecificNode(GetRandomPrefab(false, tag), position, container);
        // Get parent node if any
        GameObject parentNode;
        if (lineManager.nodeParentMap.TryGetValue(node, out parentNode))
        {
            // Remove old node from dictionary
            lineManager.nodeParentMap.Remove(node);

            // Add new node with old node's parent
            lineManager.nodeParentMap.Add(newNode, parentNode);
        }

        // If the node itself was a parent, update its children
        List<GameObject> childrenToUpdate = new List<GameObject>();
        foreach (var entry in lineManager.nodeParentMap)
        {
            if (entry.Value == node)
            {
                childrenToUpdate.Add(entry.Key);
            }
        }

        foreach (var child in childrenToUpdate)
        {
            lineManager.nodeParentMap[child] = newNode;
        }
        //destroy the node
        Destroy(node);
        if(tag == "Heal")
        {
            totalHealNodes--;
        }
        if (tag == "Upgrade")
        {
            totalUpgradeNodes--;
        }
        //set the lines as child of the new node
        foreach (var line in lines)
        {
            line.transform.SetParent(newNode.transform);
        }
        return newNode;
    }

    public GameObject ParsePrefabName(string prefabName)
    {
        if(prefabName.Contains("Heal"))
        {
            return healPrefab;
        }
        if(prefabName.Contains("Upgrade"))
        {
            return upgradePrefab;
        }
        if(prefabName.Contains("Basic"))
        {
            return basicPrefab;
        }
        if(prefabName.Contains("Boss"))
        {
            return bossPrefab;
        }
        return null;
    }
    public void CleanNodes()
    {
        foreach (var node in nodesAddedSoFar)
        {
            Destroy(node);
        }
        //also clearing dictionary
        lineManager.nodeParentMap.Clear();
    }
}