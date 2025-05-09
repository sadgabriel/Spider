using UnityEngine;
using System.Collections.Generic;
using System.Linq;

public class Enemy : MonoBehaviour, IEnemy
{
    private Node currentNode;
    
    [SerializeField] private int lifeTime = 5;
    [SerializeField] private float yOffset = 0.5f;

    [SerializeField] private int attackDamage = 1;

    private Node currentTargetPillar;

    private IPlayer player;

    public Node CurrentNode
    {
        get => currentNode;
        protected set => currentNode = value;
    }

    public void Initialize(Node startNode, IPlayer player)
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

            if (currentNode == currentTargetPillar)
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

    public bool TryMoveTo(Node targetNode)
    {
        if (CanMoveTo(targetNode))
        {
            MoveTo(targetNode);
            return true;
        }
        return false;
    }

    public bool CanMoveTo(Node targetNode)
    {
        return targetNode != null &&
               currentNode.Neighbors.Contains(targetNode) &&
               !targetNode.IsOccupied;
    }

    private void MoveTo(Node targetNode)
    {
        if (targetNode == null)
        {
            Debug.LogError("Target node is null.");
            return;
        }

        if (targetNode.IsOccupied)
        {
            Debug.LogError("Target node is occupied.");
            return;
        }
        
        if (currentNode != null)
        {
            currentNode.IsOccupied = false;
        }

        currentNode = targetNode;
        transform.position = GetPosition(targetNode);
        currentNode.IsOccupied = true;
    }

    private void LookAt(Node targetNode = null)
    {
        Vector3 direction;
        if (currentNode == null)
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
            direction = (targetNode.Position - currentNode.Position).normalized;
            transform.rotation = Quaternion.LookRotation(direction);
        }   
    }

    private Node FindNextStepTowards(Node targetNode)
    {
        var route = MapManager.Instance.FindRoute(currentNode, targetNode);
        return (route != null && route.Count > 1) ? route[1] : null;
    }

    private Node FindNextPillar(int recognitionDistance = 4)
    {
        var route = MapManager.Instance.FindRoute(currentNode, player.CurrentNode);
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
        if (currentNode is Pillar)
        {
            adjacentPillars = currentNode.Neighbors
            .Where(n => !n.IsOccupied)
            .SelectMany(n => n.Neighbors)
            .Where(n => n is Pillar && !n.IsOccupied).ToList();
        }
        else
        {
            adjacentPillars = currentNode.Neighbors
            .Where(n => n is Pillar && !n.IsOccupied).ToList();
        }

        if (adjacentPillars.Count > 0)
        {
            return adjacentPillars[Random.Range(0, adjacentPillars.Count)];
        }

        return null;
    }

    private Vector3 GetPosition(Node node)
    {
        return node.Position + Vector3.up * yOffset;
    }

    private bool IsAttackable()
    {
        Node nextNode = FindNextStepTowards(currentTargetPillar);
        return player != null &&
               currentNode != null &&
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

    private void OnDestroy()
    {
        if (currentNode != null)
        {
            currentNode.IsOccupied = false;
        }
    }
}
