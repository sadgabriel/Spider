using System.Collections.Generic;
using UnityEngine;

public abstract class MapManager : MonoBehaviour
{
    public static MapManager Instance { get; private set; }

    [SerializeField] protected GameObject LargePillarPrefab;
    [SerializeField] protected GameObject SmallPillarPrefab;
    [SerializeField] protected GameObject bridgePrefab;

    protected List<Node> Nodes { get; private set; } = new();
    private void Awake()
    {
        Instance = this;
    }

    public List<Node> FindRoute(Node from, Node to)
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

    public void ConnectPillars(Pillar pillar1, Pillar pillar2){
        if (pillar1 == null || pillar2 == null) return;

        GameObject bridgeGO = Instantiate(bridgePrefab);
        Bridge bridge = bridgeGO.GetComponent<Bridge>();

        bridge.Initialize(pillar1.Position, pillar2.Position);

        pillar1.ConnectTo(bridge);
        pillar2.ConnectTo(bridge);
        Nodes.Add(bridge);
    }

    public abstract void GenerateMap();
}
