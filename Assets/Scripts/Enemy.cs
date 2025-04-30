using UnityEngine;
using System.Collections.Generic;

public class Enemy : MonoBehaviour, IEnemy
{
    private Node currentNode;
    
    [SerializeField] private int lifeTime = 5;
    [SerializeField] private float yOffset = 0.5f;

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

        if (IsAttackable())
        {
            Attack();
            Die();
        }
        else
        {
            Node next = FindNextStepTowards(player.CurrentNode);
            TryMoveTo(next);
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

    private Node FindNextStepTowards(Node targetNode)
    {
        var route = MapManager.Instance.FindRoute(currentNode, targetNode);
        return (route != null && route.Count > 1) ? route[1] : null;
    }

    private Vector3 GetPosition(Node node)
    {
        return node.Position + Vector3.up * yOffset;
    }

    private bool IsAttackable()
    {
        return player != null &&
               currentNode != null &&
               currentNode.Neighbors.Contains(player.CurrentNode);
    }

    private void Attack()
    {
        player.TakeDamage(1);
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
