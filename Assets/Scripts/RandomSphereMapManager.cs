using System.Collections.Generic;
using UnityEngine;
using System.Linq;

class RandomSphereMapManager : MapManager
{
    [SerializeField] private int largePillarCount = 20;
    [SerializeField] private int totalPillarCount = 60;
    [SerializeField] private float largePillarAngle = 30f;
    [SerializeField] private float smallPillarAngle = 15f;
    [SerializeField] private float radius = 8f;
    [SerializeField] private int maxAttempts = 10000;
    [SerializeField] private float bridgeMinHeight = 9f;

    public override List<Node> Spawners
    {
        get
        {
            return Pillars
                .Where(p => p.Size == PillarSize.Small)
                .Select(p => p as Node)
                .ToList();
        }
    }

    public override Pillar StartPillar
    {
        get
        {
            if (Nodes.Count == 0) return null;
            return Pillars[Random.Range(0, Pillars.Count)];
        }
    }

    public override void GenerateMap()
    {
        GeneratePillars();
        GenerateBridges();
        RemoveIsolatedPillars();
    }

    private void GeneratePillars()
    {
        List<Vector3> largePillarVectors = PoissonSampler.GeneratePoissonSpherePoints(largePillarCount, largePillarAngle, maxAttempts);
        List<Vector3> allPillarVectors = PoissonSampler.GeneratePoissonSpherePoints(totalPillarCount, smallPillarAngle, maxAttempts, largePillarVectors);

        HashSet<Vector3> largePillarPositions = largePillarVectors.Select(v => v * radius + Origin.position).ToHashSet();

        foreach (var pos in allPillarVectors.Select(v => v * radius + Origin.position))
        {
            Vector3 v = pos - Origin.position;
            GameObject prefab = largePillarPositions.Contains(pos) ? LargePillarPrefab : SmallPillarPrefab;
            Quaternion rotation = Quaternion.LookRotation(Vector3.ProjectOnPlane(Vector3.back, v).normalized, v);
            string name = (prefab == LargePillarPrefab ? "LargePillar" : "SmallPillar") + $" ({pos})";
            Pillar pillar = InstantiatePillar(prefab, pos, rotation, name);
        }
    }

    private void GenerateBridges()
    {
        List<(Pillar, Pillar)> possibleBridges = new List<(Pillar, Pillar)>();
        HashSet<(int, int)> seen = new HashSet<(int, int)>();


        for (int i = 0; i < Pillars.Count; i++)
        {
            for (int j = i + 1; j < Pillars.Count; j++)
            {
                Pillar pillar1 = Pillars[i];
                Pillar pillar2 = Pillars[j];

                Vector3 bridgePosition = (CalcBridgeJointPosition(pillar1) + CalcBridgeJointPosition(pillar2)) / 2;

                if ((bridgePosition - Origin.position).magnitude > bridgeMinHeight)
                {
                    int id1 = pillar1.GetInstanceID();
                    int id2 = pillar2.GetInstanceID();
                    if (!seen.Add((id1, id2))) continue;

                    possibleBridges.Add((pillar1, pillar2));
                }
            }
        }

        foreach (var (pillar1, pillar2) in RemoveCrossingBridges(possibleBridges))
        {
            ConnectPillars(pillar1, pillar2);
        }
    }

    private List<(Pillar, Pillar)> RemoveCrossingBridges(List<(Pillar, Pillar)> possibleBridges)
    {
        int n = possibleBridges.Count;
        int[] cross = new int[n];

        for (int i = 0; i < n - 1; i++)
            for (int j = i + 1; j < n; j++)
                if (SegmentsCross(possibleBridges[i].Item1, possibleBridges[i].Item2, possibleBridges[j].Item1, possibleBridges[j].Item2, Origin.position))
                {
                    cross[i]++; cross[j]++;
                }

        var order = Enumerable.Range(0, n)
            .OrderByDescending(idx => cross[idx])
            .ThenByDescending(idx =>
                System.Math.Min(possibleBridges[idx].Item1.Neighbors.Count, possibleBridges[idx].Item2.Neighbors.Count))
            .ToList();

        var keep = new HashSet<(Pillar,Pillar)>(possibleBridges);

        foreach (int idx in order)
        {
            var e = possibleBridges[idx];

            bool stillCross = keep.Any(k => SegmentsCross(e.Item1, e.Item2, k.Item1, k.Item2, Origin.position));

            if (!stillCross) continue;

            keep.Remove(e);

            if (!IsValidBridgeSet(keep))
                keep.Add(e);  
        }
        return keep.ToList();
    }

    private static bool SegmentsCross(Pillar a1, Pillar a2, Pillar b1, Pillar b2, Vector3 origin)
    {
        Vector3 posA1 = a1.transform.position;
        Vector3 posA2 = a2.transform.position;
        Vector3 posB1 = b1.transform.position;
        Vector3 posB2 = b2.transform.position;

        Vector3 center = (posA1 + posA2 + posB1 + posB2) / 4f;
        Vector3 normal = (center - origin).normalized;

        Vector3 xAxis = Vector3.Cross(normal, Vector3.up);
        if (xAxis == Vector3.zero)
            xAxis = Vector3.Cross(normal, Vector3.forward);
        xAxis.Normalize();

        Vector3 yAxis = Vector3.Cross(normal, xAxis);

        Vector2 p1 = GeometryUtils.ProjectOntoPlane2D(posA1, origin, xAxis, yAxis);
        Vector2 p2 = GeometryUtils.ProjectOntoPlane2D(posA2, origin, xAxis, yAxis);
        Vector2 q1 = GeometryUtils.ProjectOntoPlane2D(posB1, origin, xAxis, yAxis);
        Vector2 q2 = GeometryUtils.ProjectOntoPlane2D(posB2, origin, xAxis, yAxis);

        return GeometryUtils.DoIntersect(p1, p2, q1, q2);
    }

    private bool IsValidBridgeSet(HashSet<(Pillar, Pillar)> bridges)
    {
        return IsGraphConnected(bridges, Pillars) &&
               MinDegreeSatisfied(bridges, Pillars, 2) &&
               IsLargePillarConnected(bridges, Pillars);
    }

    private bool IsGraphConnected(HashSet<(Pillar, Pillar)> bridges, List<Pillar> pillars)
    {
        if (pillars.Count == 0) return true;

        HashSet<Pillar> visited = new();
        Stack<Pillar> stack = new();
        stack.Push(pillars[0]);

        while (stack.Count > 0)
        {
            var current = stack.Pop();
            if (!visited.Add(current)) continue;

            foreach (var (a, b) in bridges)
            {
                if (a == current && !visited.Contains(b)) stack.Push(b);
                else if (b == current && !visited.Contains(a)) stack.Push(a);
            }
        }

        return visited.Count == pillars.Count;
    }

    private bool IsLargePillarConnected(HashSet<(Pillar, Pillar)> bridges, List<Pillar> Pillars)
    {
        var largePillars = Pillars.Where(p => p.Size == PillarSize.Large).ToList();
        return IsGraphConnected(bridges.Where(edge => largePillars.Contains(edge.Item1) && largePillars.Contains(edge.Item2)).ToHashSet(), largePillars);
    }

    private bool MinDegreeSatisfied(HashSet<(Pillar, Pillar)> bridges, List<Pillar> pillars, int min)
    {
        Dictionary<Pillar, int> degree = new();
        foreach (var pillar in pillars)
            degree[pillar] = 0;

        foreach (var (a, b) in bridges)
        {
            degree[a]++;
            degree[b]++;
        }

        return degree.Values.All(d => d >= min);
    }

    private void RemoveIsolatedPillars()
    {
        var isolatedPillars = Pillars.Where(p => p.Neighbors.Count == 0).ToList();
        foreach (var pillar in isolatedPillars)
        {
            RemoveNode(pillar);
        }
    }
}