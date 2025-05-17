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
    [SerializeField] private float margin = 2f;
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
        List<Vector2> largePillarPositions = PoissonSampler.GeneratePoissonPoints(largePillarCount, largePillarRadius, mapXRange, mapZRange, maxAttempts);
        List<Vector2> pillarPositions = PoissonSampler.GeneratePoissonPoints(totalPillarCount, smallPillarRadius, mapXRange, mapZRange, maxAttempts, largePillarPositions);

        List<Vector2> hullPoints = GeometryUtils.ComputeConvexHull(largePillarPositions);
        List<(Vector2, Vector2)> constraints = GeometryUtils.GetHullEdges(hullPoints);

        List<(Vector2, Vector2)> edges = TriangleNetWrapper.GenerateConstrainedDelaunayEdges(pillarPositions, constraints);

        pillarPositions.RemoveAll(pos => !edges.Exists(edge => edge.Item1 == pos || edge.Item2 == pos));

        Dictionary<Vector2, Pillar> pillarMap = new Dictionary<Vector2, Pillar>();

        foreach (var pos in largePillarPositions)
        {
            Vector3 pillarPosition = new Vector3(pos.x, 0, pos.y);

            Pillar pillar = InstantiatePillar(LargePillarPrefab, pillarPosition, Quaternion.identity, $"LargePillar ({pos.x}, {pos.y})");
            pillarMap[pos] = pillar;

            if (hullPoints.Contains(pos))
            {
                Vector3 center = new Vector3(mapXRange.Item1 + mapXRange.Item2, 0, mapZRange.Item1 + mapZRange.Item2) / 2;
                Vector3 direction = (new Vector3(pos.x, 0, pos.y) - center).normalized;
                
                float t = Mathf.Min(
                    direction.x > 0 ? (mapXRange.Item2 + margin - pos.x) / direction.x : (mapXRange.Item1 - margin - pos.x) / direction.x,
                    direction.z > 0 ? (mapZRange.Item2 + margin - pos.y) / direction.z : (mapZRange.Item1 - margin - pos.y) / direction.z
                );

                Vector3 edgePoint = new Vector3(pos.x + direction.x * t, 0, pos.y + direction.z * t);

                GameObject bridgeGO = Instantiate(bridgePrefab);
                Bridge bridge = bridgeGO.GetComponent<Bridge>();
                bridge.Initialize(CalcBridgeJunctionPosition(pillar), edgePoint);
                pillar.ConnectTo(bridge);
                Nodes.Add(bridge);
                Bridges.Add(bridge);
                Spawners.Add(bridge);
            }
        }
        
        foreach (var pos in pillarPositions)
        {
            if (largePillarPositions.Contains(pos)) continue;

            Pillar pillar = InstantiatePillar(SmallPillarPrefab, new Vector3(pos.x, 0, pos.y), Quaternion.identity, $"SmallPillar ({pos.x}, {pos.y})");
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
}
