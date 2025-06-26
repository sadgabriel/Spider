using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using Unity.Burst.Intrinsics;

abstract class Mover : Enemy
{
    [SerializeField] protected int expOnDeath = 10;

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

    protected Node FindNextNodeTowards(Node targetNode)
    {
        var route = Map.Instance.FindPath(CurrentNode, targetNode);
        return (route != null && route.Count > 1) ? route[1] : null;
    }

    protected Node FindNextPillarTowardsPlayerIfInRange(int recognitionNodeDistance)
    {
        int distance = Map.Instance.CalcPathNodeDistance(CurrentNode, player.CurrentNode);
        if (distance > 0 && distance <= recognitionNodeDistance)
        {
            Node nextPillar = FindNextPillarTowardsPlayer();
            if (nextPillar != null)
            {
                return nextPillar;
            }
        }

        return null;
    }

    protected Node FindNextPillarTowardsPlayer() => FindNextPillarTowards(player.CurrentNode);

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

    public override void Die()
    {
        if (IsDestroyed) return;

        IsDestroyed = true;

        if (State == EnemyState.Alerted)
        {
            Map.Instance.AddExpOn(CurrentNode, expOnDeath);
        }

        Destroy(gameObject);
    }
}