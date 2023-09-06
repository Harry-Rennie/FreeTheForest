using UnityEngine;
using System;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Collections.Generic;

// in progress, serializable data for saving and loading maps
[Serializable]
public class SerializableVector2
{
    //small custom class that allows us to serialize and deserialize vectors to file
    public float x;
    public float y;

    public SerializableVector2(Vector2 v)
    {
        x = v.x;
        y = v.y;
    }

    public Vector2 ToVector2()
    {
        return new Vector2(x, y);
    }
}

[Serializable]
public class GraphLayoutData
{
    public List<SerializableVector2> nodePositions;
    public List<string> nodePrefabNames;
    //add other node data here as needed... save data
}

public class GraphLayoutManager : MonoBehaviour
{
    private string layoutDataPath;

    private void Awake()
    {
        layoutDataPath = Application.persistentDataPath + "/graph_layout.dat";
    }

    public void SaveGraphLayout(List<SerializableVector2> nodePositions, List<string> nodePrefabNames)
    {
        try
        {
            GraphLayoutData layoutData = new GraphLayoutData { nodePositions = nodePositions, nodePrefabNames = nodePrefabNames };
            using (FileStream file = File.Open(layoutDataPath, FileMode.Create, FileAccess.Write, FileShare.None))
            {
                BinaryFormatter bf = new BinaryFormatter();
                bf.Serialize(file, layoutData);
            }
        }
        catch (Exception e)
        {
            Debug.LogError("Error saving graph layout: " + e.Message);
        }
    }

    public GraphLayoutData LoadGraphLayout()
    {
        if (File.Exists(layoutDataPath))
        {
            try
            {
                using (FileStream file = File.Open(layoutDataPath, FileMode.Open, FileAccess.Read, FileShare.Read))
                {
                    BinaryFormatter bf = new BinaryFormatter();
                    GraphLayoutData layoutData = (GraphLayoutData)bf.Deserialize(file);
                    return layoutData;
                }
            }
            catch (Exception e)
            {
                Debug.LogError("Error loading graph layout: " + e.Message);
            }
        }

        return null;
    }

    public void DeleteGraphLayout()
    {
        try
        {
            //close any existing file streams that might be open
            if (File.Exists(layoutDataPath))
            {
                File.SetAttributes(layoutDataPath, FileAttributes.Normal); //remove any read only attributes
                using (FileStream fs = File.Open(layoutDataPath, FileMode.Open, FileAccess.ReadWrite, FileShare.None))
                {
                    fs.Close();
                }

                File.Delete(layoutDataPath);
            }
        }
        catch (Exception e)
        {
            Debug.LogError("Error deleting graph layout: " + e.Message);
        }
    }
}