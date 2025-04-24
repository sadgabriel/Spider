using UnityEngine;
using System.Collections.Generic;

public class RandomMapManager : MapManager
{
    [SerializeField] private int largePillarCount = 20;
    [SerializeField] private int totalPillarCount = 60;
    [SerializeField] private float largePillarRadius = 3f;
    [SerializeField] private float smallPillarRadius = 1f;
    [SerializeField] private (float, float) mapXRange = (-10f, 10f);
    [SerializeField] private (float, float) mapZRange = (-10f, 10f);
    [SerializeField] private int maxAttempts = 10000;

    private void Start()
    {
        GenerateMap();
    }
    public override void GenerateMap()
    {
        List<Vector2> largePillarPositions = PoissonSampler.GeneratePoissonPoints(largePillarCount, largePillarRadius, mapXRange, mapZRange, maxAttempts);
        List<Vector2> pillarPositions = PoissonSampler.GeneratePoissonPoints(totalPillarCount, smallPillarRadius, mapXRange, mapZRange, maxAttempts, largePillarPositions);

        List<Vector2> hullPoints = GeometryUtils.ComputeConvexHull(largePillarPositions);
        List<(Vector2, Vector2)> constraints = GeometryUtils.GetHullEdges(hullPoints);

        List<(Vector2, Vector2)> edges = TriangleNetWrapper.GenerateConstrainedDelaunayEdges(pillarPositions, constraints);

        pillarPositions.RemoveAll(pos => !edges.Exists(edge => edge.Item1 == pos || edge.Item2 == pos));

        Dictionary<Vector2, Pillar> pillarMap = new Dictionary<Vector2, Pillar>();

        foreach (var pos in largePillarPositions)
        {
            Pillar pillar = InstantiatePillar(new Vector3(pos.x, 0, pos.y), LargePillarPrefab, $"LargePillar ({pos.x}, {pos.y})");
            pillarMap[pos] = pillar;
        }
        
        foreach (var pos in pillarPositions)
        {
            if (largePillarPositions.Contains(pos)) continue;

            Pillar pillar = InstantiatePillar(new Vector3(pos.x, 0, pos.y), SmallPillarPrefab, $"SmallPillar ({pos.x}, {pos.y})");
            pillarMap[pos] = pillar;
        }

        foreach (var edge in edges)
        {
            Vector2 start = edge.Item1;
            Vector2 end = edge.Item2;

            if (pillarMap.TryGetValue(start, out Pillar startPillar) && pillarMap.TryGetValue(end, out Pillar endPillar))
            {
                ConnectPillars(startPillar, endPillar);
            }
        }
    }

    private Pillar InstantiatePillar(Vector3 position, GameObject prefab, string name)
    {
        GameObject pillarGO = Instantiate(prefab, position, Quaternion.identity);
        pillarGO.name = name;
        Pillar pillar = pillarGO.GetComponent<Pillar>();
        Nodes.Add(pillar);

        return pillar;
    }
}
