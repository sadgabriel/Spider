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
    [SerializeField] private float bridgeMinRadius = 7f;

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

        List<Vector3> largePillarPositions = largePillarVectors.Select(v => v * radius).ToList();
        List<Vector3> allPillarPositions = allPillarVectors.Select(v => v * radius).ToList();

        foreach (var pos in allPillarPositions)
        {
            GameObject prefab = largePillarPositions.Contains(pos) ? LargePillarPrefab : SmallPillarPrefab;
            Quaternion rotation = Quaternion.LookRotation(Vector3.ProjectOnPlane(Vector3.back, pos).normalized, pos);
            string name = (prefab == LargePillarPrefab ? "LargePillar" : "SmallPillar") + $" ({pos})";
            Pillar pillar = InstantiatePillar(prefab, pos, rotation, name);
        }
    }

    private void GenerateBridges()
    {
        foreach (var pillar1 in Pillars)
        {
            foreach (var pillar2 in Pillars)
            {
                if (pillar1 == pillar2) continue;

                Vector3 bridgePosition = (CalcBridgeJointPosition(pillar1) + CalcBridgeJointPosition(pillar2)) / 2;

                if (bridgePosition.magnitude > bridgeMinRadius)
                {
                    ConnectPillars(pillar1, pillar2);
                }
            }
        }
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