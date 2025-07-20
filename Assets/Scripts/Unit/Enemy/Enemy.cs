using UnityEngine;
using System.Collections;
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
    [SerializeField] protected GameObject alertedEffect;

    public bool IsDead { get; protected set; } = false;

    private EnemyState state = EnemyState.Idle;
    public EnemyState State
    {
        get => state;
        protected set
        {
            if (state == value) return;
            state = value;
            UpdateAlertEffect();
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
    }

    public virtual void Initialize(Node startNode, Player player)
    {
        SetStartNode(startNode);
        AssignTargetPlayer(player);
    }

    public override void Die()
    {
        IsDead = true;
        Destroy(gameObject);
    }

    public virtual void Act()
    {
        LifeTime--;
    }

    public virtual IEnumerator DoAct()
    {
        LifeTime--;
        yield break;
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
        PutOn(node);
    }

    private void AssignTargetPlayer(Player player)
    {
        this.player = player;
    }

    protected void Attack()
    {
        player.TakeDamage(attackDamage);
    }

    private void UpdateAlertEffect()
    {
        if (alertedEffect == null) return;
        alertedEffect.SetActive(State == EnemyState.Alerted);
    }
}
