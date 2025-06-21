using UnityEngine;
using System.Collections.Generic;

public class SprintFacility : Facility
{
    private const int MaxUpgrade = 7;
    private const int MinUpgrade = 0;
    public override int MaxUpgradeLevel => MaxUpgrade;
    public override int MinUpgradeLevel => MinUpgrade;
    

    private readonly Sprint sprint = Sprint.Instance;

    private static readonly HashSet<int> StaminaReductionLevels = new() { 2, 3, 5, 6 };
    private static readonly HashSet<int> ReachIncreaseLevels = new() { 4, 7 };

    protected override void HandleUpgradeLevelIncrease(int level)
    {
        base.HandleUpgradeLevelIncrease(level);
        AddSprintToSpecialAction(level);
        UpgradeSprint(level);
    }

    protected override void HandleUpgradeLevelDecrease(int level)
    {
        base.HandleUpgradeLevelDecrease(level);
        RemoveSprintFromSpecialAction(level);
        DowngradeSprint(level);
    }

    private void AddSprintToSpecialAction(int level)
    {
        if (level == 1)
        {
            SpecialActionManager.Instance.AddSpecialAction(sprint);
        }
    }

    private void RemoveSprintFromSpecialAction(int level)
    {
        if (level == 1)
        {
            SpecialActionManager.Instance.RemoveSpecialAction(sprint);
        }
    }

    private void UpgradeSprint(int level)
    {
        if (StaminaReductionLevels.Contains(level)) sprint.BaseStaminaConsume -= 10;
        if (ReachIncreaseLevels.Contains(level)) sprint.Reach += 1;
    }

    private void DowngradeSprint(int level)
    {
        if (StaminaReductionLevels.Contains(level)) sprint.BaseStaminaConsume += 10;
        if (ReachIncreaseLevels.Contains(level)) sprint.Reach -= 1;
    }
}
