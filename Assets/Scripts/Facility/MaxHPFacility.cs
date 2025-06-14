using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class MaxHPFacility : Facility
{
    private int maxUpgradeLevel = 7;
    public override int MaxUpgradeLevel => maxUpgradeLevel;

    protected override void SetUpgradeHandlers()
    {
        base.SetUpgradeHandlers();

        OnUpgradeLevelIncrease += IncreasePlayerMaxHPByUpgradeLevel;
        OnUpgradeLevelIncrease += IncreasePlayerHPRegenerationByUpgradeLevel;

        OnUpgradeLevelDecrease += DecreasePlayerMaxHPByUpgradeLevel;
        OnUpgradeLevelDecrease += DecreasePlayerHPRegenerationByUpgradeLevel;
    }

    private void IncreasePlayerMaxHPByUpgradeLevel(int upgradeLevel)
    {
        Player player = Player.Instance;

        switch (upgradeLevel)
        {
            case 1:
            case 2:
                player.MaxHP += 30;
                break;
            case 3:
                player.MaxHP += 40;
                break;
            case 5:
            case 6:
                player.MaxHP += 50;
                break;
        }
    }

    private void DecreasePlayerMaxHPByUpgradeLevel(int upgradeLevel)
    {
        Player player = Player.Instance;

        switch (upgradeLevel)
        {
            case 1:
            case 2:
                player.MaxHP -= 30;
                break;
            case 3:
                player.MaxHP -= 40;
                break;
            case 5:
            case 6:
                player.MaxHP -= 50;
                break;
        }
    }

    private void IncreasePlayerHPRegenerationByUpgradeLevel(int upgradeLevel)
    {
        Player player = Player.Instance;

        switch (upgradeLevel)
        {
            case 4:
                player.HPRegen += 10;
                break;
            case 7:
                player.HPRegen += 10;
                break;
        }
    }

    private void DecreasePlayerHPRegenerationByUpgradeLevel(int upgradeLevel)
    {
        Player player = Player.Instance;

        switch (upgradeLevel)
        {
            case 4:
                player.HPRegen -= 10;
                break;
            case 7:
                player.HPRegen -= 10;
                break;
        }
    }
}