using System.Collections.Generic;
using UnityEngine;
using System.Linq;

class RandomSphereMap : Map
{
    [SerializeField] private int largePillarCount = 20;
    [SerializeField] private int totalPillarCount = 60;
    [SerializeField] private float largePillarAngle = 30f;
    [SerializeField] private float smallPillarAngle = 15f;
    [SerializeField] private float radius = 8f;
    [SerializeField] private int maxAttempts = 10000;
    [SerializeField] private float bridgeMinHeight = 9f;

    public override List<Node> SpawnPoints
    {
        get
        {
            return Pillars
                .Where(p => p.isActiveAndEnabled)
                .Where(p => p.Size == PillarSize.Large)
                .Where(p => !p.HasFacility)
                .Where(p => !p.IsOccupied)
                .Select(p => p as Node)
                .ToList();
        }
    }

    public override Pillar StartPillar
    {
        get
        {
            if (Nodes.Count == 0) return null;
            Camera mainCamera = Camera.main;
            if (mainCamera == null) return null;
            Vector3 camPos = mainCamera.transform.position;

            Pillar startPillar = Pillars
                .OrderBy(p => Vector3.Distance(p.transform.position, camPos))
                .FirstOrDefault();

            return startPillar;
        }
    }

    public override void GenerateMap()
    {
        GeneratePillars();
        GenerateBridges();
    }

    public override Bridge ConnectPillars(Pillar pillar1, Pillar pillar2)
    {
        if (pillar1 == null || pillar2 == null) return null;

        Bridge bridge = Instantiate(bridgePrefab, transform).GetComponent<Bridge>();

        Vector3 direction = pillar2.TopPosition - pillar1.TopPosition;
        Vector3 from = CalcBridgeJointPosition(pillar1, direction);
        Vector3 to = CalcBridgeJointPosition(pillar2, -direction);

        Vector3 upwards = (from + to) / 2 - Origin;

        bridge.Initialize(from, to, upwards);
        
        pillar1.ConnectTo(bridge);
        pillar2.ConnectTo(bridge);
        nodes.Add(bridge);
        bridges.Add(bridge);
        
        return bridge;
    }

    private void GeneratePillars()
    {
        List<Vector3> largePillarVectors = PoissonSampler.GeneratePoissonSpherePoints(largePillarCount, largePillarAngle, maxAttempts);
        List<Vector3> allPillarVectors = PoissonSampler.GeneratePoissonSpherePoints(totalPillarCount, smallPillarAngle, maxAttempts, largePillarVectors);

        HashSet<Vector3> largePillarPositions = largePillarVectors.Select(v => v * radius + Origin).ToHashSet();

        foreach (Vector3 pos in allPillarVectors.Select(v => v * radius + Origin))
        {
            Vector3 v = pos - Origin;
            GameObject prefab = largePillarPositions.Contains(pos) ? LargePillarPrefab : SmallPillarPrefab;
            Quaternion rotation = Quaternion.LookRotation(Vector3.ProjectOnPlane(Vector3.back, v).normalized, v);
            string name = (prefab == LargePillarPrefab ? "LargePillar" : "SmallPillar") + $" ({pos})";
            Pillar pillar = InstantiatePillar(prefab, pos, rotation, name);
        }
    }

    private void GenerateBridges()
    {
        List<(Pillar, Pillar)> possibleBridges = new List<(Pillar, Pillar)>();

        for (int i = 0; i < Pillars.Count; i++)
        {
            for (int j = i + 1; j < Pillars.Count; j++)
            {
                Pillar pillar1 = Pillars[i];
                Pillar pillar2 = Pillars[j];

                Vector3 bridgePosition = (CalcBridgeJointPosition(pillar1) + CalcBridgeJointPosition(pillar2)) / 2;

                if ((bridgePosition - Origin).magnitude > bridgeMinHeight)
                {
                    possibleBridges.Add((pillar1, pillar2));
                }
            }
        }

        List<(Pillar, Pillar)> validBridges = RemoveCrossingBridges(possibleBridges);
        foreach (var (pillar1, pillar2) in validBridges)
        {
            ConnectPillars(pillar1, pillar2);
        }
    }

    private List<(Pillar, Pillar)> RemoveCrossingBridges(List<(Pillar, Pillar)> possibleBridges)
    {
        int n = possibleBridges.Count;
        int[] crossCount = new int[n];
        List<int>[] crossWith = new List<int>[n];
        for (int i = 0; i < n; i++)
        {
            crossCount[i] = 0;
            crossWith[i] = new List<int>();
        }

        for (int i = 0; i < n - 1; i++)
        {
            var (a1, a2) = possibleBridges[i];
            for (int j = i + 1; j < n; j++)
            {
                var (b1, b2) = possibleBridges[j];
                if (SegmentsCross(a1, a2, b1, b2, Origin))
                {
                    crossCount[i]++;
                    crossCount[j]++;
                    crossWith[i].Add(j);
                    crossWith[j].Add(i);
                }
            }
        }

        int[] order = Enumerable.Range(0, n)
            .OrderByDescending(idx => crossCount[idx])
            .ThenByDescending(idx =>
                System.Math.Min(possibleBridges[idx].Item1.Neighbors.Count, possibleBridges[idx].Item2.Neighbors.Count))
            .ToArray();

        HashSet<(Pillar, Pillar)> vaild = new HashSet<(Pillar, Pillar)>(possibleBridges);

        foreach (int idx in order)
        {
            var bridge = possibleBridges[idx];

            if (crossCount[idx] == 0) continue;

            bool removed = true;
            vaild.Remove(bridge);

            if (removed)
            {
                foreach (int crossIdx in crossWith[idx])
                {
                    crossCount[crossIdx]--;
                    crossWith[crossIdx].Remove(idx);
                }
            }
        }
        return vaild.ToList();
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
}