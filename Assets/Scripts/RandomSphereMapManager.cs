using System.Collections.Generic;
using UnityEngine;
using System.Linq;

class RandomSphereMapManager : MapManager
{
    [SerializeField] private int largePillarCount = 20;
    [SerializeField] private int totalPillarCount = 60;
    [SerializeField] private float largePillarAngle = 30f;
    [SerializeField] private float smallPillarAngle = 15f;
    [SerializeField] private float radius = 10f;
    [SerializeField] private int maxAttempts = 10000;

    public override Pillar StartPillar {
        get
        {
            if (Nodes.Count == 0) return null;
            return Pillars[Random.Range(0, Pillars.Count)];
        }
    }

    public override void GenerateMap()
    {
        List<Vector3> largePillarVectors = PoissonSampler.GeneratePoissonSpherePoints(largePillarCount, largePillarAngle, maxAttempts);

        List<Vector3> allPillarVectors = PoissonSampler.GeneratePoissonSpherePoints(totalPillarCount, smallPillarAngle, maxAttempts, largePillarVectors);

        float largeConnectDist = 10f;
        float smallConnectDist = 6f;

        List<Vector3> largePillarPositions = largePillarVectors.Select(v => v * radius).ToList();
        List<Vector3> allPillarPositions = allPillarVectors.Select(v => v * radius).ToList();

        var largeEdges = ConnectByDistance(largePillarPositions, largeConnectDist);
        var smallEdges = ConnectByDistance(allPillarPositions, smallConnectDist);

        Dictionary<Vector3, Pillar> pillarMap = new();

        foreach (var pos in allPillarPositions)
        {
            GameObject prefab = largePillarPositions.Contains(pos) ? LargePillarPrefab : SmallPillarPrefab;
            Quaternion rotation = Quaternion.LookRotation(Vector3.ProjectOnPlane(Vector3.back, pos).normalized, pos);
            string name = (prefab == LargePillarPrefab ? "LargePillar" : "SmallPillar") + $" ({pos})";
            Pillar pillar = InstantiatePillar(prefab, pos, rotation, name);
            pillarMap[pos] = pillar;
        }

        foreach (var (start, end) in largeEdges.Concat(smallEdges))
        {
            if (pillarMap.TryGetValue(start, out var p1) && pillarMap.TryGetValue(end, out var p2))
            {
                ConnectPillars(p1, p2);
            }
        }
    }

    private List<(Vector3, Vector3)> ConnectByDistance(List<Vector3> points, float maxDistance)
    {
        var edges = new List<(Vector3, Vector3)>();

        for (int i = 0; i < points.Count; i++)
        {
            for (int j = i + 1; j < points.Count; j++)
            {
                if (Vector3.Distance(points[i], points[j]) <= maxDistance)
                {
                    edges.Add((points[i], points[j]));
                }
            }
        }

        return edges;
    }
}