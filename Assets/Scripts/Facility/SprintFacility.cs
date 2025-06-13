using UnityEngine;

class SprintFacility : Facility
{
    private int maxUpgradeLevel = 7;
    public override int MaxUpgradeLevel => maxUpgradeLevel;

    private Sprint sprint = new Sprint();

    protected override void SetUpgradeHandlers()
    {
        base.SetUpgradeHandlers();

        OnUpgradeLevelIncrease += AddSprintToSpecialAction;
        OnUpgradeLevelIncrease += UpgradeSprint;

        OnUpgradeLevelDecrease += RemoveSprintFromSpecialAction;
        OnUpgradeLevelDecrease += DowngradeSprint;
    }

    private void AddSprintToSpecialAction(int upgradeLevel)
    {
        if (upgradeLevel == 1)
        {
            SpecialActionManager.Instance.AddSpecialAction(sprint);
        }
    }

    private void RemoveSprintFromSpecialAction(int upgradeLevel)
    {
        if (upgradeLevel == 1)
        {
            SpecialActionManager.Instance.RemoveSpecialAction(sprint);
        }
    }

    private void UpgradeSprint(int upgradeLevel)
    {
        switch (upgradeLevel)
        {
            case 2:
            case 3:
            case 5:
            case 6:
                sprint.BaseStaminaConsume -= 10;
                break;
            case 4:
            case 7:
                sprint.Reach += 1;
                break;
        }
    }

    private void DowngradeSprint(int upgradeLevel)
    {
        switch (upgradeLevel)
        {
            case 2:
            case 3:
            case 5:
            case 6:
                sprint.BaseStaminaConsume += 10;
                break;
            case 4:
            case 7:
                sprint.Reach -= 1;
                break;
        }
    }
}