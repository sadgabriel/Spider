using UnityEngine;

class UpgradeFacility : Facility
{
    private int maxUpgradeLevel = 7;
    public override int MaxUpgradeLevel => maxUpgradeLevel;
}