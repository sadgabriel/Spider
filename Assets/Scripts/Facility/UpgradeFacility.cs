using UnityEngine;

public class UpgradeFacility : Facility
{
    private const int MaxUpgrade = 1;
    private const int MinUpgrade = 1;
    public override int MaxUpgradeLevel => MaxUpgrade;
    public override int MinUpgradeLevel => MinUpgrade;

    protected override void Initialize()
    {
        foreach (Pillar pillar in CurrentPillar.NeighborPillars)
        {
            pillar.FacilityUpgradeLevel += 1;
        }
    }
}