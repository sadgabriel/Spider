using UnityEngine;
using System.Collections.Generic;
using System.Linq;

class Pursuer : Mover
{
    [SerializeField] private int recognitionDistance = 4;
    public override void Act()
    {
        base.Act();

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
    
    private void SetNextPillar()
    {
        Node nextPillar = FindNextPillarTowardsPlayerIfInRange(recognitionDistance);
        if (nextPillar != null)
        {
            State = EnemyState.Alerted;
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