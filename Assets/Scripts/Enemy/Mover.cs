using UnityEngine;
using System.Collections.Generic;
using System.Linq;

abstract class Mover : Enemy
{
    protected Node currentTargetPillar;

    protected Node FindRandomAdjacentPillar()
    {
        List<Node> adjacentPillars;
        if (CurrentNode is Pillar)
        {
            adjacentPillars = CurrentNode.Neighbors
            .Where(n => !n.IsOccupied)
            .SelectMany(n => n.Neighbors)
            .Where(n => n is Pillar && !n.IsOccupied).ToList();
        }
        else
        {
            adjacentPillars = CurrentNode.Neighbors
            .Where(n => n is Pillar && !n.IsOccupied).ToList();
        }

        if (adjacentPillars.Count > 0)
        {
            return adjacentPillars[Random.Range(0, adjacentPillars.Count)];
        }

        return null;
    }

    protected Node FindNextStepTowards(Node targetNode)
    {
        var route = Map.Instance.FindPath(CurrentNode, targetNode);
        return (route != null && route.Count > 1) ? route[1] : null;
    }

    protected Node FindNextPillarTowardsPlayerIfInRange(int recognitionDistance)
    {
        int distance = Map.Instance.CalcPathNodeDistance(CurrentNode, player.CurrentNode);
        if (distance > 0 && distance <= recognitionDistance)
        {
            Node nextPillar = FindNextPillarTowardsPlayer();
            if (nextPillar != null)
            {
                return nextPillar;
            }
        }

        return null;
    }

    protected Node FindNextPillarTowardsPlayer()
    {
        return FindNextPillarTowards(player.CurrentNode);
    }

    protected Node FindNextPillarTowards(Node targetNode)
    {
        var route = Map.Instance.FindPath(CurrentNode, targetNode);
        if (route != null && route.Count > 1)
        {
            for (int i = 1; i < route.Count; i++)
            {
                if (route[i] is Pillar)
                {
                    return route[i];
                }
            }
        }
        return null;
    }
}