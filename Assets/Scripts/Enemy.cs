using UnityEngine;
using System.Collections.Generic;
using System.Linq;

public class Enemy : Unit
{
    [SerializeField] private int lifeTime = 5;

    [SerializeField] private int attackDamage = 1;

    private Node currentTargetPillar;
    private Player player;

    public void Initialize(Node startNode, Player player)
    {
        MoveTo(startNode);
        this.player = player;
    }

    public void Act()
    {
        lifeTime--;

        if (lifeTime <= 0)
        {
            Die();
            return;
        }

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

    private void LookAt(Node targetNode = null)
    {
        Vector3 direction;
        if (CurrentNode == null)
        {
            Debug.LogError("Current node is null.");
            return;
        }
        else if (targetNode == null)
        {
            transform.rotation = Quaternion.identity;
            return;
        }
        else
        {
            direction = (targetNode.Position - CurrentNode.Position).normalized;
            transform.rotation = Quaternion.LookRotation(direction);
        }   
    }

    private Node FindNextStepTowards(Node targetNode)
    {
        var route = MapManager.Instance.FindRoute(CurrentNode, targetNode);
        return (route != null && route.Count > 1) ? route[1] : null;
    }

    private Node FindNextPillar(int recognitionDistance = 4)
    {
        var route = MapManager.Instance.FindRoute(CurrentNode, player.CurrentNode);
        if (route != null && route.Count > 1 && route.Count <= recognitionDistance + 1)
        {
            for (int i = 1; i < route.Count; i++)
            {
                if (route[i] is Pillar)
                {
                    return route[i];
                }
            }
        }

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

    private void Attack()
    {
        player.TakeDamage(attackDamage);
    }
    
    private void Die()
    {
        Destroy(gameObject);
    }
}
