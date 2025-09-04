using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class MaxHpFacility : Facility
{
    private const int MaxUpgrade = 7;
    private const int MinUpgrade = 0;
    public override int MaxUpgradeLevel => MaxUpgrade;
    public override int MinUpgradeLevel => MinUpgrade;

    private static readonly Dictionary<int, int> MaxHpBonusByUpgradeLevel = new()
    {
        { 1, 30 }, { 2, 30 }, { 3, 40 }, { 5, 50 }, { 6, 50 }
    };

    private static readonly Dictionary<int, int> HpRegenBonusByUpgradeLevel = new()
    {
        { 4, 5 }, { 7, 5 }
    };

    protected override void HandleUpgradeLevelIncrease(int level)
    {
        base.HandleUpgradeLevelIncrease(level);
        ApplyMaxHpBonus(level);
        ApplyHpRegenBonus(level);
    }

    protected override void HandleUpgradeLevelDecrease(int level)
    {
        base.HandleUpgradeLevelDecrease(level);
        RemoveMaxHpBonus(level);
        RemoveHpRegenBonus(level);
    }

    private void ApplyMaxHpBonus(int upgradeLevel)
    {
        if (Player.Instance == null) return;
        if (MaxHpBonusByUpgradeLevel.TryGetValue(upgradeLevel, out int bonus))
            Player.Instance.MaxHp += bonus;
    }

    private void RemoveMaxHpBonus(int upgradeLevel)
    {
        if (Player.Instance == null) return;
        if (MaxHpBonusByUpgradeLevel.TryGetValue(upgradeLevel, out int bonus))
            Player.Instance.MaxHp -= bonus;
    }

    private void ApplyHpRegenBonus(int upgradeLevel)
    {
        if (Player.Instance == null) return;
        if (HpRegenBonusByUpgradeLevel.TryGetValue(upgradeLevel, out int bonus))
            Player.Instance.HpRegen += bonus;
    }

    private void RemoveHpRegenBonus(int upgradeLevel)
    {
        if (Player.Instance == null) return;
        if (HpRegenBonusByUpgradeLevel.TryGetValue(upgradeLevel, out int bonus))
            Player.Instance.HpRegen -= bonus;
    }
}