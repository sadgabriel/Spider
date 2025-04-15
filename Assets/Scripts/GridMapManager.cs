using UnityEngine;

public class GridMapManager : MapManager
{
    [SerializeField] private int width;
    [SerializeField] private int height;
    [SerializeField] private float spacing;
    [SerializeField] private GameObject nodePrefab;
    [SerializeField] private float playerYOffset;

    private Node[,] grid;

    public override void GenerateMap()
    {
        grid = new Node[width, height];

        for (int x = 0; x < width; x++)
        {
            for (int z = 0; z < height; z++)
            {
                Vector3 position = new Vector3(x * spacing, 0, z * spacing);
                GameObject nodeGO = Instantiate(nodePrefab, position, nodePrefab.transform.rotation, transform);
                nodeGO.name = $"Node ({x}, {z})";

                Node node = nodeGO.GetComponent<Node>();

                if (x == 0 || x == width - 1 || z == 0 || z == height - 1)
                {
                    node.isSpawner = true;
                    SpawnerNodes.Add(node);
                }

                grid[x, z] = node;
                Nodes.Add(node);
            }
        }

        for (int x = 0; x < width; x++)
        {
            for (int z = 0; z < height; z++)
            {
                Node node = grid[x, z];

                TryConnect(node, x + 1, z);
                TryConnect(node, x - 1, z);
                TryConnect(node, x, z + 1);
                TryConnect(node, x, z - 1);
            }
        }
    }

    public override Node GetStartNode()
    {
        return Nodes.Count > 0 ? Nodes[0] : null;
    }

    public override float GetPlayerYOffset()
    {
        return playerYOffset;
    }

    private void TryConnect(Node node, int x, int z)
    {
        if (x < 0 || x >= width || z < 0 || z >= height) return;

        Node neighbor = grid[x, z];
        if (neighbor != null)
        {
            node.ConnectTo(neighbor);
        }
    }
}
