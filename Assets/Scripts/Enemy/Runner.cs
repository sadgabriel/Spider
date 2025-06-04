using System;
using UnityEngine;

class Runner : Mover
{
    [SerializeField] private int recognitionDistance = 6;
    [SerializeField] private int runningSpeed = 2;
    [SerializeField] private int runningLifeTime = 3;

    public override void Act()
    {
        base.Act();

        for (int i = 0; i < runningSpeed; i++)
        {
            if (currentTargetPillar == null)
            {
                SetNextPillar();
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
    }
    
    private void SetNextPillar()
    {
        Node nextPillar = FindNextPillarTowardsPlayerIfInRange(recognitionDistance);
        if (nextPillar != null)
        {
            State = EnemyState.Alerted;
            LifeTime = Math.Min(LifeTime, runningLifeTime);
            currentTargetPillar = nextPillar;
            return;
        }

        State = EnemyState.Idle;
        currentTargetPillar = FindRandomAdjacentPillar();
    }

    private bool IsAttackable()
    {
        Node nextNode = FindNextStepTowards(currentTargetPillar);
        return player != null &&
               CurrentNode != null &&
               nextNode != null &&
               player.CurrentNode == nextNode;
    }    
}
