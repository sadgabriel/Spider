using UnityEngine;
using System.Collections.Generic;
using System.Linq;

enum EnemyState
{
    Idle,
    Alerted
}

public class Enemy : Unit
{
    [SerializeField] private int lifeTime = 5;

    [SerializeField] private int attackDamage = 1;

    [SerializeField] private Material defaultMaterial;
    [SerializeField] private Material alertedMaterial;

    private Node currentTargetPillar;
    private Player player;

    private Renderer[] childrenRenderers;

    private EnemyState state = EnemyState.Idle;
    private EnemyState State
    {
        get => state;
        set
        {
            if (state == value) return;
            state = value;
            UpdateMaterial();
        }
    }

    private void Awake()
    {
        childrenRenderers = GetComponentsInChildren<Renderer>();
    }

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
        if (CurrentNode == null)
        {
            Debug.LogError("Current node is null.");
            return;
        }

        if (targetNode == null)
        {
            transform.rotation = CalcUnitRotation(CurrentNode);
            return;
        }
        
        Vector3 surfaceNormal = CurrentNode.transform.up;

        Vector3 forward = Vector3.ProjectOnPlane(
            targetNode.TopPosition - CurrentNode.TopPosition,
            surfaceNormal
        ).normalized;

        if (forward == Vector3.zero)
        {
            forward = Vector3.Cross(surfaceNormal, Vector3.right);
        }

        transform.rotation = Quaternion.LookRotation(forward, surfaceNormal);
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

    private void Attack()
    {
        player.TakeDamage(attackDamage);
    }
    
    private void Die()
    {
        Destroy(gameObject);
    }

    private void UpdateMaterial()
    {
        foreach (Renderer childRenderer in childrenRenderers)
        {
            switch (State)
            {
                case EnemyState.Idle:
                    childRenderer.material = defaultMaterial;
                    break;
                case EnemyState.Alerted:
                    childRenderer.material = alertedMaterial;
                    break;
            }
        }
    }
}
