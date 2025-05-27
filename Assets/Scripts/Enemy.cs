using UnityEngine;
using System.Collections.Generic;
using System.Linq;

enum EnemyState
{
    Idle,
    Alerted
}

public abstract class Enemy : Unit
{
    [SerializeField] protected int lifeTime = 5;
    [SerializeField] protected int attackDamage = 1;

    protected Player player;

    public virtual void Initialize(Node startNode, Player player)
    {
        MoveTo(startNode);
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

        transform.rotation = Quaternion.LookRotation(forward, surfaceNormal);
    }

    protected void Attack()
    {
        player.TakeDamage(attackDamage);
    }
    
    protected void Die()
    {
        Destroy(gameObject);
    }

    public abstract void Act();
}
