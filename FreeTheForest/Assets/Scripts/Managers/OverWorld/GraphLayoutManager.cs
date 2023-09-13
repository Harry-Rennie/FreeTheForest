using UnityEngine;
using System;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Collections.Generic;

//todo: remove debug logs when finished
[Serializable]
public class SerializableLine
{
    public SerializableVector2 startPoint;
    public SerializableVector2 endPoint;

    public SerializableLine(Vector2 startPoint, Vector2 endPoint)
    {
        this.startPoint = new SerializableVector2(startPoint);
        this.endPoint = new SerializableVector2(endPoint);
    }
}
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
public class SerializableNode
{
    public SerializableVector2 position;
    public string prefabName;
    public List<SerializableLine> associatedLines;

    public SerializableNode(Vector2 position, string prefabName)
    {
        this.position = new SerializableVector2(position);
        this.prefabName = prefabName;
        this.associatedLines = new List<SerializableLine>();
    }
}
[Serializable]
public class GraphLayoutData
{
    public List<SerializableNode> nodes;
    //add other node data here as needed... save data
}

public class GraphLayoutManager : MonoBehaviour
{
    private string layoutDataPath;

    private void Awake()
    {
        layoutDataPath = Application.persistentDataPath + "/graph_layout.dat";
    }

    public void SaveGraphLayout(List<SerializableNode> nodes)
    {
        try
        {
            GraphLayoutData layoutData = new GraphLayoutData { nodes = nodes };
            using (FileStream file = File.Open(layoutDataPath, FileMode.Create, FileAccess.Write, FileShare.None))
            {
                BinaryFormatter bf = new BinaryFormatter();
                bf.Serialize(file, layoutData);
                Debug.Log("File saved to:" + layoutDataPath);
            }
        }
        catch (Exception e)
        {
            Debug.LogError("Error saving graph layout: " + e.Message);
        }
    }

    public List<SerializableNode> LoadGraphLayout()
    {
        if (File.Exists(layoutDataPath))
        {
            try
            {
                using (FileStream file = File.Open(layoutDataPath, FileMode.Open, FileAccess.Read, FileShare.Read))
                {
                    BinaryFormatter bf = new BinaryFormatter();
                    GraphLayoutData layoutData = (GraphLayoutData)bf.Deserialize(file);
                    Debug.Log("File loaded from:" + layoutDataPath);
                    return layoutData.nodes;
                }
            }
            catch (Exception e)
            {
                Debug.LogError("Error loading graph layout: " + e.Message);
            }
        }

        return new List<SerializableNode>();
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
                Debug.Log("File deleted from:" + layoutDataPath);
                File.Delete(layoutDataPath);
            }
        }
        catch (Exception e)
        {
            Debug.LogError("Error deleting graph layout: " + e.Message);
        }
    }
}