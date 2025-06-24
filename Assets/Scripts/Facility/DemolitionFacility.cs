using UnityEngine;
using System.Collections.Generic;

public class DemolitionFacility : Facility
{
    private const int MaxUpgrade = 7;
    private const int MinUpgrade = 0;
    public override int MaxUpgradeLevel => MaxUpgrade;
    public override int MinUpgradeLevel => MinUpgrade;

    private readonly Demolition demolition = Demolition.Instance;

    protected override void HandleUpgradeLevelIncrease(int level)
    {
        base.HandleUpgradeLevelIncrease(level);
        switch (level)
        {
            case 1:
                SpecialActionManager.Instance.AddSpecialAction(demolition);
                break;
            case 2:
            case 3:
            case 5:
            case 6:
                demolition.StaminaConsume -= 10;
                break;
            case 4:
                demolition.NeedTurnEnd = false;
                break;
            case 7:
                demolition.HpGain += 20;
                break;
        }
    }

    protected override void HandleUpgradeLevelDecrease(int level)
    {
        base.HandleUpgradeLevelDecrease(level);
        switch (level)
        {
            case 1:
                SpecialActionManager.Instance.RemoveSpecialAction(demolition);
                break;
            case 2:
            case 3:
            case 5:
            case 6:
                demolition.StaminaConsume += 10;
                break;
            case 4:
                demolition.NeedTurnEnd = true;
                break;
            case 7:
                demolition.HpGain -= 20;
                break;
        }
    }
}