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
        
        for (int x  = 0; x < width; x++)
        {
            for (int z = 0; z < height; z++)
            {
                Vector3 position = new Vector3(x * spacing, 0, z * spacing);
                GameObject nodeGO = Instantiate(nodePrefab, position, nodePrefab.transform.rotation, transform);
                nodeGO.name = $"Node ({x}, {z})";

                Node node = nodeGO.GetComponent<Node>();
                grid[x, z] = node;
                nodes.Add(node);
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
        return nodes[0];
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
