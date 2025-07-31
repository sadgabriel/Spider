using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using Unity.Mathematics;

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
    [SerializeField] protected int expOnDeath = 0;

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
                if (state == EnemyState.Alerted)
                {
                    DieWithExp();
                }
                else
                {
                    DieWithoutExp();
                }
            }
        }
    }

    private Renderer[] childrenRenderers;

    protected override void Awake()
    {
        base.Awake();
        childrenRenderers = GetComponentsInChildren<Renderer>();
    }

    public virtual void Initialize(Node startNode, Player player)
    {
        SetStartNode(startNode);
        AssignTargetPlayer(player);
    }

    public override void Die()
    {
        DieWithoutExp();
    }

    protected void DieWithExp()
    {
        if (IsDead) return;

        IsDead = true;
        if (expOnDeath > 0)
        {
            Map.Instance.AddExpOn(CurrentNode, expOnDeath);
        }
        Destroy(gameObject);
    }

    protected void DieWithoutExp()
    {
        if (IsDead) return;

        IsDead = true;
        Destroy(gameObject);
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

    protected void AttackWithoutMotion()
    {
        player.TakeDamage(attackDamage);
        AudioManager.Instance.PlayEnemyAttackSfx();
    }

    protected IEnumerator DoAttackWithMotion(float duration = 0.2f)
    {
        float elapsed = 0f;

        while (elapsed < duration)
        {
            Vector3 startPosition = CalcUnitPosition(CurrentNode);
            Vector3 endPosition = CalcUnitPosition(player.CurrentNode);

            float t = elapsed / duration;
            transform.position = Vector3.Lerp(startPosition, endPosition, t);
            
            elapsed += Time.deltaTime;
            yield return null;
        }

        player.TakeDamage(attackDamage);
        AudioManager.Instance.PlayEnemyAttackSfx();
    }

    private void UpdateAlertEffect()
    {
        if (alertedEffect == null) return;
        alertedEffect.SetActive(State == EnemyState.Alerted);
    }
}
