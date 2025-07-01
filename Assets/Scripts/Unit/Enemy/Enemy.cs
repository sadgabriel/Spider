using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;

public enum EnemyState
{
    Idle,
    Alerted,
}

public abstract class Enemy : Unit
{
    [SerializeField] protected int lifeTime = 5;
    [SerializeField] protected int attackDamage = 1;

    [SerializeField] private Material defaultMaterial;
    [SerializeField] private Material alertedMaterial;

    public bool IsDestroyed { get; protected set; } = false;

    private EnemyState state = EnemyState.Idle;
    public EnemyState State
    {
        get => state;
        protected set
        {
            if (state == value) return;
            state = value;
            UpdateMaterial();
        }
    }

    protected Player player;

    protected int LifeTime
    {
        get => lifeTime;
        set
        {
            lifeTime = value;
            if (lifeTime <= 0)
            {
                Die();
            }
        }
    }

    private Renderer[] childrenRenderers;

    protected virtual void Awake()
    {
        childrenRenderers = GetComponentsInChildren<Renderer>();
        UpdateMaterial();
    }

    public virtual void Initialize(Node startNode, Player player)
    {
        SetStartNode(startNode);
        AssignTargetPlayer(player);
    }

    public override void Die()
    {
        IsDestroyed = true;
        Destroy(gameObject);
    }

    protected void SetVisualsVisible(bool visible)
    {
        if (childrenRenderers == null) return;

        foreach (var r in childrenRenderers)
        {
            r.enabled = visible;
        }
    }

    private void SetStartNode(Node node)
    {
        MoveTo(node);
    }

    private void AssignTargetPlayer(Player player)
    {
        this.player = player;
    }

    protected void LookAt(Node targetNode = null)
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

        if (forward == Vector3.zero)
        {
            forward = Vector3.forward;
        }

        transform.rotation = Quaternion.LookRotation(forward, surfaceNormal);
    }

    protected void Attack()
    {
        player.TakeDamage(attackDamage);
    }

    public virtual void Act()
    {
        LifeTime--;
    }

    private void UpdateMaterial()
    {
        if (childrenRenderers == null || childrenRenderers.Length == 0)
        {
            Debug.LogWarning("No renderers found for enemy material update.");
            return;
        }

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
