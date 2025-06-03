using System.Collections.Generic;
using UnityEngine;

public abstract class Map : MonoBehaviour
{
    public static Map Instance { get; private set; }

    public List<Node> Nodes { get; private set; } = new();
    public List<Pillar> Pillars { get; private set; } = new();
    public List<Bridge> Bridges { get; private set; } = new();
    public virtual List<Node> SpawnPoints { get; private set; } = new();

    [SerializeField] protected GameObject LargePillarPrefab;
    [SerializeField] protected GameObject SmallPillarPrefab;
    [SerializeField] protected GameObject bridgePrefab;
    [SerializeField] protected float bridgeOffset = -0.5f;

    public Vector3 Origin
    {
        get
        {
            return transform.position;
        }
    }

    public abstract Pillar StartPillar { get; }

    private void Awake()
    {
        Instance = this;
    }

    public List<Node> FindPath(Node from, Node to, bool ignoreOccupied = false)
    {
        if (from == null || to == null) return null;

        Dictionary<Node, Node> cameFrom = new();
        Queue<Node> frontier = new();
        HashSet<Node> visited = new();

        frontier.Enqueue(from);
        visited.Add(from);
        cameFrom[from] = null;

        bool found = false;
        while (frontier.Count > 0 && !found)
        {
            Node current = frontier.Dequeue();

            if (current == to)
                break;

            foreach (Node neighbor in current.Neighbors)
            {
                if (!visited.Contains(neighbor) && (ignoreOccupied || !neighbor.IsOccupied || neighbor == to))
                {
                    visited.Add(neighbor);
                    frontier.Enqueue(neighbor);
                    cameFrom[neighbor] = current;

                    if (neighbor == to)
                    {
                        found = true;
                        break;
                    }
                }
            }
        }

        if (!cameFrom.ContainsKey(to))
        {
            return null;
        }

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

    public int CalcTrueDistance(Node from, Node to)
    {
        return CalcDistance(from, to, true);
    }

    public int CalcPathDistance(Node from, Node to)
    {
        return CalcDistance(from, to, false);
    }

    private int CalcDistance(Node from, Node to, bool ignoreOccupied)
    {
        if (from == null || to == null) return -1;

        List<Node> path = FindPath(from, to, ignoreOccupied);
        if (path == null) return -1;

        return path.Count - 1;
    }

    public virtual Bridge ConnectPillars(Pillar pillar1, Pillar pillar2)
    {
        if (pillar1 == null || pillar2 == null) return null;

        Bridge bridge = Instantiate(bridgePrefab, transform).GetComponent<Bridge>();

        bridge.Initialize(CalcBridgeJointPosition(pillar1), CalcBridgeJointPosition(pillar2));

        pillar1.ConnectTo(bridge);
        pillar2.ConnectTo(bridge);
        Nodes.Add(bridge);
        Bridges.Add(bridge);

        return bridge;
    }

    protected Pillar InstantiatePillar(GameObject prefab, Vector3 position, Quaternion rotation, string name)
    {
        GameObject pillarGO = Instantiate(prefab, position, rotation, transform);
        pillarGO.name = name;
        Pillar pillar = pillarGO.GetComponent<Pillar>();
        Nodes.Add(pillar);
        Pillars.Add(pillar);
        
        return pillar;
    }

    protected void RemoveNode(Node node)
    {
        if (node == null) return;

        Nodes.Remove(node);
        if (node is Pillar pillar)
        {
            Pillars.Remove(pillar);
        }
        else if (node is Bridge bridge)
        {
            Bridges.Remove(bridge);
        }
        
        if (SpawnPoints.Contains(node))
        {
            SpawnPoints.Remove(node);
        }
    }

    protected Vector3 CalcBridgeJointPosition(Pillar pillar)
    {
        return pillar.TopPosition + pillar.transform.up * bridgeOffset;
    }

    public abstract void GenerateMap();
}
