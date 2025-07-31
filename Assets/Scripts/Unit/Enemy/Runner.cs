using System;
using UnityEngine;
using System.Collections;

class Runner : Mover
{
    [SerializeField] private int recognitionDistance = 6;
    [SerializeField] private int runningSpeed = 2;
    [SerializeField] private int alertedLifeTime = 5;

    public override IEnumerator DoAct()
    {
        yield return base.DoAct();

        for (int i = 0; i < runningSpeed; i++)
        {
            if (currentTargetPillar == null)
            {
                SetNextPillar();
            }

            Node nextNode = FindNextNodeTowards(currentTargetPillar);

            if (IsAttackable())
            {
                yield return DoAttackWithMotion();
                DieWithoutExp();
                yield break;
            }
            else
            {
                yield return DoTryMoveTo(nextNode);

                if (this == null || gameObject == null || this is Enemy enemy && enemy.IsDead)
                {
                    yield break;
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
        }
    }
    
    private void SetNextPillar()
    {
        Node nextPillar = FindNextPillarTowardsPlayerIfInRange(recognitionDistance);
        if (nextPillar != null)
        {
            State = EnemyState.Alerted;
            LifeTime = Math.Min(LifeTime, alertedLifeTime);
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
