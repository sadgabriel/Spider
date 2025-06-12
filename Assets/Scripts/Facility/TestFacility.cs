using UnityEngine;

class TestFacility : Facility
{
    private int maxUpgradeLevel = 7;
    public override int MaxUpgradeLevel => maxUpgradeLevel;
}