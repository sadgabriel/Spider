using System.Collections.Generic;
using UnityEngine;

public abstract class MapManager : MonoBehaviour
{
    protected List<Node> nodes = new();

    public abstract void GenerateMap();
    protected virtual void Start()
    {
        
    }

    public void RegenerateMap()
    {
        ResetMap();
        GenerateMap();
    }

    protected virtual void ResetMap()
    {
        foreach (Node node in nodes)
        {
            if (node != null)
            {
                Destroy(node.gameObject);
            }
        }

        nodes.Clear();
    }

    public virtual List<Node> GetNodes()
    {
        return nodes;
    }

    public abstract Node GetStartNode();

    public abstract float GetPlayerYOffset();
}
