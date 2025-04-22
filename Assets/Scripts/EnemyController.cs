using UnityEngine;

public class EnemyController : UnitController
{
    // [SerializeField] private int lifeTime = 5;

    // private PlayerController player;

    // public void Initialize(PlayerController player)
    // {
    //     this.player = player;
    // }

    // public void Act()
    // {
    //     lifeTime--;

    //     if (lifeTime <= 0)
    //     {
    //         Destroy(gameObject);
    //         return;
    //     }

    //     if (IsAttackable())
    //     {
    //         Attack();
    //         Destroy(gameObject);
    //     }
    //     else
    //     {
    //         Node targetNode = FindNextStepTowards();
    //         TryMoveTo(targetNode);
    //     }
    // }

    // protected virtual bool IsAttackable()
    // {
    //     return player != null &&
    //            CurrentNode != null &&
    //            CurrentNode.Neighbors.Contains(player.CurrentNode);
    // }

    // protected virtual void Attack()
    // {
    //     player.TakeDamage(1);
    // }

    // protected virtual Node FindNextStepTowards()
    // {
    //     var route = MapManager.Instance.FindRoute(CurrentNode, player.CurrentNode);
    //     return (route != null && route.Count > 1) ? route[1] : null;
    // }
}
