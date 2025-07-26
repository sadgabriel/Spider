using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public enum BossPhase
{
    Waiting,
    SpawningPreparation,
    Spawning,
    JumpingPreparation,
    Jumping,
    Landing,
}

public class Boss : Enemy
{
    [SerializeField] private GameObject arrowPrefab;

    public BossPhase CurrentPhase = BossPhase.Waiting;
    public bool IsReadyToSpawn { get; set; } = false;
    public List<Node> SpawnPoints
    {
        get
        {
            List<Node> spawnPoints = new();
            foreach (Node neighbor in CurrentNode.Neighbors)
            {
                if (!neighbor.IsOccupied)
                {
                    spawnPoints.Add(neighbor);
                }
            }

            return spawnPoints;
        }
    }
    private Pillar currentTargetPillar;

    private GameObject arrow;

    public override void Act()
    {
        base.Act();

        switch (CurrentPhase)
        {
            case BossPhase.Waiting:
                CurrentPhase = BossPhase.SpawningPreparation;
                break;

            case BossPhase.SpawningPreparation:
                State = EnemyState.Alerted;
                CurrentPhase = BossPhase.Spawning;
                break;

            case BossPhase.Spawning:
                State = EnemyState.Idle;
                IsReadyToSpawn = true;
                CurrentPhase = BossPhase.JumpingPreparation;
                break;

            case BossPhase.JumpingPreparation:
                CurrentPhase = BossPhase.Jumping;
                break;

            case BossPhase.Jumping:
                currentTargetPillar = FindNextPillar();
                JumpToAir();
                NotifyNextPillar(currentTargetPillar);
                CurrentPhase = BossPhase.Landing;
                break;

            case BossPhase.Landing:
                JumpToGround(currentTargetPillar);
                CurrentPhase = BossPhase.Waiting;
                break;
        }
    }

    public override IEnumerator DoAct()
    {
        yield return base.DoAct();

        switch (CurrentPhase)
        {
            case BossPhase.Waiting:
                CurrentPhase = BossPhase.SpawningPreparation;
                break;

            case BossPhase.SpawningPreparation:
                State = EnemyState.Alerted;
                CurrentPhase = BossPhase.Spawning;
                break;

            case BossPhase.Spawning:
                State = EnemyState.Idle;
                IsReadyToSpawn = true;
                CurrentPhase = BossPhase.JumpingPreparation;
                break;

            case BossPhase.JumpingPreparation:
                CurrentPhase = BossPhase.Jumping;
                break;

            case BossPhase.Jumping:
                currentTargetPillar = FindNextPillar();
                JumpToAir();
                NotifyNextPillar(currentTargetPillar);
                CurrentPhase = BossPhase.Landing;
                break;

            case BossPhase.Landing:
                Destroy(arrow);
                yield return new WaitForSeconds(0.55f);
                JumpToGround(currentTargetPillar);
                CurrentPhase = BossPhase.Waiting;
                break;
        }
    }

    public override bool CanMoveTo(Node targetNode)
    {
        return targetNode != null &&
               targetNode is Pillar pillar &&
               targetNode != CurrentNode &&
               pillar.Size == PillarSize.Large;
    }

    private Pillar FindNextPillar()
    {
        List<Pillar> pillars = Map.Instance.Pillars
            .Where(p => CanMoveTo(p))
            .ToList();

        if (pillars.Count == 0)
            return null;

        Pillar closestPillar = null;
        int minDistance = int.MaxValue;

        foreach (Pillar pillar in pillars)
        {
            int distance = Map.Instance.CalcTruePillarDistance(Player.Instance.CurrentNode, pillar);
            if (distance < minDistance)
            {
                minDistance = distance;
                closestPillar = pillar;
            }
        }

        return closestPillar;
    }

    private void JumpToAir()
    {
        CurrentNode.OccupyingUnit = null;
        Map.Instance.SmashPillar(CurrentNode as Pillar);
        CurrentNode = null;
        SetVisualsVisible(false);
    }

    private void JumpToGround(Pillar targetPillar)
    {
        if (targetPillar.IsOccupied)
        {
            Unit unit = targetPillar.OccupyingUnit;
            if (unit is Player player)
            {
                Pillar playerCurrentPillar = player.CurrentNode as Pillar;
                Pillar adjacentPillar = playerCurrentPillar.NeighborPillars.FirstOrDefault();
                if (adjacentPillar != null && adjacentPillar != targetPillar)
                {
                    player.PutOn(adjacentPillar);
                    Attack();
                }
                else
                {
                    player.Die();
                }
            }
            else if (unit is Enemy enemy)
            {
                enemy.Die();
            }
        }

        PutOn(targetPillar);
        SetVisualsVisible(true);
    }

    private void NotifyNextPillar(Pillar nextPillar)
    {
        Vector3 arrowPosition = nextPillar.TopPosition + nextPillar.transform.up * 4f;

        Vector3 forward;
        if (nextPillar.DirectionFromOrigin != Vector3.up)
        {
            forward = Vector3.Cross(nextPillar.DirectionFromOrigin, Vector3.up).normalized;
        }
        else
        {
            forward = Vector3.Cross(nextPillar.DirectionFromOrigin, Vector3.right).normalized;
        }
        Vector3 upward = Vector3.Cross(forward, nextPillar.DirectionFromOrigin).normalized;

        Quaternion arrowRotation = Quaternion.LookRotation(forward, upward);

        arrow = Instantiate(arrowPrefab, arrowPosition, arrowRotation, Map.Instance.transform);
    }
}