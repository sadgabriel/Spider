using UnityEngine;
using System.Collections;

class Corroder : Mover
{
    [SerializeField] private int recognitionDistance = 4;
    [SerializeField] private int alertedLifeTime = 5;

    public override void Act()
    {
        base.Act();

        if (currentTargetPillar == null)
        {
            SetNextPillar();
        }

        Node nextNode = FindNextNodeTowards(currentTargetPillar);

        if (IsAttackable())
        {
            Attack();
            Die();
            return;
        }

        Node lastNode = CurrentNode;
        if (TryMoveTo(nextNode))
        {
            if (lastNode is Bridge bridge)
            {
                Map.Instance.CorrodeBridge(bridge);
            }
        }

        if (CurrentNode == currentTargetPillar)
        {
            currentTargetPillar = null;
            LookAt(null);
        }
        else
        {
            nextNode = FindNextNodeTowards(currentTargetPillar);
            LookAt(nextNode);
        }
    }

    public override IEnumerator DoAct()
    {
        yield return base.DoAct();

        if (currentTargetPillar == null)
        {
            SetNextPillar();
        }

        Node nextNode = FindNextNodeTowards(currentTargetPillar);

        if (IsAttackable())
        {
            Attack();
            Die();
            yield break;
        }

        Node lastNode = CurrentNode;
        if (TryMoveTo(nextNode))
        {
            if (lastNode is Bridge bridge)
            {
                Map.Instance.CorrodeBridge(bridge);
            }
        }

        if (CurrentNode == currentTargetPillar)
        {
            currentTargetPillar = null;
            LookAt(null);
        }
        else
        {
            nextNode = FindNextNodeTowards(currentTargetPillar);
            LookAt(nextNode);
        }
    }

    private void SetNextPillar()
    {
        Node nextPillar = FindNextPillarTowardsPlayerIfInRange(recognitionDistance);
        if (nextPillar != null)
        {
            State = EnemyState.Alerted;
            LifeTime = System.Math.Min(LifeTime, alertedLifeTime);
            currentTargetPillar = nextPillar;
            return;
        }

        State = EnemyState.Idle;
        currentTargetPillar = FindRandomAdjacentPillar();
    }

    private bool IsAttackable()
    {
        Node nextNode = FindNextNodeTowards(currentTargetPillar);
        return player != null &&
               CurrentNode != null &&
               nextNode != null &&
               player.CurrentNode == nextNode;
    }
}