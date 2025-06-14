using UnityEngine;

public class UpgradeFacility : Facility
{
    private int maxUpgradeLevel = 1;
    public override int MaxUpgradeLevel => maxUpgradeLevel;

    protected override void Initialize()
    {
        base.Initialize();

        foreach (Pillar pillar in CurrentPillar.NeighborPillars)
        {
            pillar.FacilityUpgradeLevel += 1;
        }
    }
}