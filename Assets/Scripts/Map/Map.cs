using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;

public abstract class Map : MonoBehaviour
{
    public static Map Instance { get; private set; }

    [SerializeField] protected GameObject LargePillarPrefab;
    [SerializeField] protected GameObject SmallPillarPrefab;
    [SerializeField] protected GameObject bridgePrefab;
    [SerializeField] protected GameObject ExpOrbPrefab;
    [SerializeField] protected float bridgeOffset = -0.5f;
    [SerializeField] protected int regenerationDelay = 10;

    public List<Node> Nodes
    {
        get => nodes.Where(n => n.isActiveAndEnabled).ToList();
    }

    public List<Pillar> Pillars
    {
        get => pillars.Where(p => p.isActiveAndEnabled).ToList();
    }

    public List<Bridge> Bridges
    {
        get => bridges.Where(b => b.isActiveAndEnabled).ToList();
    }

    public virtual List<Node> SpawnPoints
    {
        get => spawnPoints.Where(sp => sp.isActiveAndEnabled).ToList();
    }

    public Vector3 Origin
    {
        get
        {
            return transform.position;
        }
    }

    public virtual Pillar StartPillar
    {
        get
        {
            return Pillars.FirstOrDefault();
        }
    }

    protected List<Node> nodes = new();
    protected List<Pillar> pillars = new();
    protected List<Bridge> bridges = new();
    protected List<Node> spawnPoints = new();

    private Queue<(Node, int)> regenerationQueue = new();

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void Initialize()
    {
        GameStateManager.Instance.OnTurnChange += OnTurnChange;
        GenerateMap();
    }

    public void AddExpOn(Node node, int amount)
    {
        if (node.HasExpOrb)
        {
            node.ExpOrb.ExpAmount += amount;
        }
        else
        {
            ExpOrb expOrb = Instantiate(ExpOrbPrefab, transform).GetComponent<ExpOrb>();
            expOrb.Initialize(node, amount);
            node.ExpOrb = expOrb;
        }
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

    public int CalcTruePillarDistance(Node from, Node to)
    {
        return CalcTrueNodeDistance(from, to) / 2;
    }

    public int CalcPathPillarDistance(Node from, Node to)
    {
        return CalcPathNodeDistance(from, to) / 2;
    }

    public int CalcTrueNodeDistance(Node from, Node to)
    {
        return CalcNodeDistance(from, to, true);
    }

    public int CalcPathNodeDistance(Node from, Node to)
    {
        return CalcNodeDistance(from, to, false);
    }

    private int CalcNodeDistance(Node from, Node to, bool ignoreOccupied)
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
        nodes.Add(bridge);
        bridges.Add(bridge);

        return bridge;
    }

    public void DemolishBridge(Bridge bridge)
    {
        if (bridge == null) return;

        if (bridge.IsOccupied && bridge.OccupyingUnit is Enemy enemy)
        {
            Player.Instance.GainExperience(enemy.ExpOnDeath);
        }

        TemporarilyRemoveNode(bridge);
    }

    public void CorrodeBridge(Bridge bridge)
    {
        if (bridge == null) return;

        TemporarilyRemoveNode(bridge);
    }

    public void SmashPillar(Pillar pillar)
    {
        if (pillar == null) return;

        TemporarilyRemoveNode(pillar);
    }

    protected Pillar InstantiatePillar(GameObject prefab, Vector3 position, Quaternion rotation, string name)
    {
        GameObject pillarGO = Instantiate(prefab, position, rotation, transform);
        pillarGO.name = name;
        Pillar pillar = pillarGO.GetComponent<Pillar>();
        nodes.Add(pillar);
        pillars.Add(pillar);

        return pillar;
    }

    protected void TemporarilyRemoveNode(Node node)
    {
        if (node == null) return;

        if (node is Pillar pillar)
        {
            foreach (Node neighbor in pillar.AllNeighbors.ToList())
            {
                if (neighbor is Bridge)
                {
                    TemporarilyRemoveNode(neighbor);
                }
            }
        }

        node.Clear();
        node.gameObject.SetActive(false);

        regenerationQueue = new Queue<(Node, int)>(regenerationQueue.Where(e => e.Item1 != node));

        int currentTurnCount = GameStateManager.Instance.TurnCount;
        regenerationQueue.Enqueue((node, currentTurnCount + regenerationDelay));
    }

    protected Vector3 CalcBridgeJointPosition(Pillar pillar)
    {
        return pillar.TopPosition + pillar.transform.up * bridgeOffset;
    }

    public abstract void GenerateMap();

    private void OnTurnChange(TurnState state, int turnCount)
    {
        if (state == TurnState.PlayerTurn)
        {
            while (regenerationQueue.Count > 0 && regenerationQueue.Peek().Item2 <= turnCount)
            {
                var (node, _) = regenerationQueue.Dequeue();
                node.gameObject.SetActive(true);
            }
        }
    }
}
