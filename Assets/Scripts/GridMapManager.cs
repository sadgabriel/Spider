using UnityEngine;

public class GridMapManager : MapManager
{
    [SerializeField] private int width;
    [SerializeField] private int height;
    [SerializeField] private float spacing;
    [SerializeField] private GameObject pillarPrefab;
    [SerializeField] private GameObject bridgePrefab;
    [SerializeField] private float playerYOffset;

    private Pillar[,] grid;

    private void Start()
    {
        GenerateMap();
    }

    public override void GenerateMap()
    {
        grid = new Pillar[width, height];

        for (int x = 0; x < width; x++)
        {
            for (int z = 0; z < height; z++)
            {
                Vector3 position = new Vector3(x * spacing, 0, z * spacing);
                GameObject pillarGO = Instantiate(pillarPrefab, position, pillarPrefab.transform.rotation);
                pillarGO.name = $"Pillar ({x}, {z})";

                Pillar pillar = pillarGO.GetComponent<Pillar>();

                grid[x, z] = pillar;
                Nodes.Add(pillar);
            }
        }

        for (int x = 0; x < width; x++)
        {
            for (int z = 0; z < height; z++)
            {
                Pillar pillar = grid[x, z];

                TryConnect(pillar, x + 1, z);
                TryConnect(pillar, x, z + 1);
            }
        }
    }

    private void TryConnect(Pillar pillar, int x, int z)
    {
        if (x < 0 || x >= width || z < 0 || z >= height) return;

        Pillar neighbor = grid[x, z];
        if (neighbor != null)
        {
            Bridge bridge = pillar.CreateBridgeTo(neighbor, bridgePrefab);
            if (bridge != null)
            {
                Nodes.Add(bridge);
            }
        }
    }
}
