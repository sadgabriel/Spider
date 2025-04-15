using System.Collections.Generic;
using UnityEngine;

public abstract class MapManager : MonoBehaviour
{
    public static MapManager Instance { get; private set; }

    protected List<Node> Nodes { get; private set; } = new();
    protected List<Node> SpawnerNodes { get; private set; } = new();

    private void Awake()
    {
        Instance = this;
    }

    public void RegenerateMap()
    {
        ResetMap();
        GenerateMap();
    }

    protected virtual void ResetMap()
    {
        foreach (Node node in Nodes)
        {
            if (node != null)
            {
                Destroy(node.gameObject);
            }
        }

        Nodes.Clear();
        SpawnerNodes.Clear();
    }

    public virtual List<Node> GetNodes()
    {
        return Nodes;
    }

    public virtual List<Node> GetSpawnerNodes()
    {
        return SpawnerNodes;
    }

    public virtual List<Node> FindRoute(Node from, Node to)
    {
        if (from == null || to == null) return null;

        Dictionary<Node, Node> cameFrom = new();
        Queue<Node> frontier = new();
        HashSet<Node> visited = new();

        frontier.Enqueue(from);
        visited.Add(from);
        cameFrom[from] = null;

        while (frontier.Count > 0)
        {
            Node current = frontier.Dequeue();

            if (current == to)
                break;

            foreach (Node neighbor in current.Neighbors)
            {
                if (!visited.Contains(neighbor) && (!neighbor.IsOccupied || neighbor == to))
                {
                    visited.Add(neighbor);
                    frontier.Enqueue(neighbor);
                    cameFrom[neighbor] = current;
                }
            }
        }

        if (!cameFrom.ContainsKey(to)) return null;

        List<Node> path = new();
        Node step = to;

        while (step != null)
        {
            path.Add(step);
            step = cameFrom[step];
        }

        path.Reverse();
        return path;
    }

    public abstract void GenerateMap();
    public abstract Node GetStartNode();
    public abstract float GetPlayerYOffset();
}
