using UnityEngine;

public class EnemyController : UnitController
{
    [SerializeField] private int lifeTime = 5;

    private PlayerController player;
    public void Initialize(PlayerController player)
    {
        this.player = player;
    }

    public void Act()
    {
        lifeTime--;
        if (lifeTime <= 0)
        {
            Destroy(gameObject);
            return;
        }

        if (IsAttackable())
        {
            Attack();
            Destroy(gameObject);
        }
        else
        {
            Node targetNode = FindNextStepTowards();
            TryMoveTo(targetNode);
        }
    }

    protected virtual bool IsAttackable()
    {
        return player != null && currentNode != null && currentNode.neighbors.Contains(player.currentNode);
    }

    protected virtual void Attack()
    {
        player.TakeDamage(1);
    }

    private Node FindNextStepTowards()
    {
        if (player == null)
        {
            return null;
        }

        Node next = null;
        float minDist = float.MaxValue;

        foreach (Node neighbor in currentNode.neighbors)
        {
            float dist = Vector3.Distance(neighbor.transform.position, player.currentNode.transform.position);
            if (dist < minDist)
            {
                minDist = dist;
                next = neighbor;
            }
        }

        return next;
    }
}
