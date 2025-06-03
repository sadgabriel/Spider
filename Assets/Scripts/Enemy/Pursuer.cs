using UnityEngine;
using System.Collections.Generic;
using System.Linq;

class Pursuer : Enemy
{
    private Node currentTargetPillar;

    public override void Act()
    {
        base.Act();

        if (currentTargetPillar == null)
        {
            currentTargetPillar = FindNextPillar();
        }

        Node nextNode = FindNextStepTowards(currentTargetPillar);

        if (IsAttackable())
        {
            Attack();
            Die();
        }
        else
        {
            TryMoveTo(nextNode);

            if (CurrentNode == currentTargetPillar)
            {
                currentTargetPillar = null;
                LookAt(null);
            }
            else
            {
                nextNode = FindNextStepTowards(currentTargetPillar);
                LookAt(nextNode);
            }
        }
    }

    private Node FindNextStepTowards(Node targetNode)
    {
        var route = Map.Instance.FindPath(CurrentNode, targetNode);
        return (route != null && route.Count > 1) ? route[1] : null;
    }

    private Node FindNextPillar(int recognitionDistance = 4)
    {
        var route = Map.Instance.FindPath(CurrentNode, player.CurrentNode);
        if (route != null && route.Count > 1 && route.Count <= recognitionDistance + 1)
        {
            for (int i = 1; i < route.Count; i++)
            {
                if (route[i] is Pillar)
                {
                    State = EnemyState.Alerted;
                    return route[i];
                }
            }
        }
        State = EnemyState.Idle;
        return FindRandomAdjacentPillar();
    }

    private Node FindRandomAdjacentPillar()
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

    private bool IsAttackable()
    {
        Node nextNode = FindNextStepTowards(currentTargetPillar);
        return player != null &&
               CurrentNode != null &&
               nextNode != null &&
               player.CurrentNode == nextNode;
    }    
}