using System;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public enum PillarSize
{
    Small,
    Large
}

public class Pillar : Node
{
    [SerializeField] private PillarSize size;

    public Facility BuiltFacility { get; set; } = null;
    public bool HasFacility => BuiltFacility != null;

    public Action<int> OnFacilityUpgradeLevelChange;

    private int facilityUpgradeLevel = 1;
    public int FacilityUpgradeLevel
    {
        get
        {
            return facilityUpgradeLevel;
        }
        set
        {
            if (facilityUpgradeLevel != value)
            {
                facilityUpgradeLevel = value;
                OnFacilityUpgradeLevelChange?.Invoke(value);
            }
        }
    }

    public PillarSize Size
    {
        get => size;
        set => size = value;
    }

    public float Height
    {
        get
        {
            var mesh = GetComponent<MeshFilter>().sharedMesh;
            return mesh.bounds.size.y * transform.lossyScale.y;
        }
    }

    public float Diameter
    {
        get
        {
            var mesh = GetComponent<MeshFilter>().sharedMesh;
            return mesh.bounds.size.x * transform.lossyScale.x;
        }
    }

    public List<Pillar> NeighborPillars
    {
        get
        {
            return Neighbors.SelectMany(node => node.Neighbors)
                            .Where(node => node != this)
                            .Select(node => (Pillar)node)
                            .ToList();
        }
    }

    public List<Pillar> AllNeighborPillars
    {
        get
        {
            return AllNeighbors.SelectMany(node => node.AllNeighbors)
                            .Where(node => node != this)
                            .Select(node => (Pillar)node)
                            .ToList();
        }
    }

    public Bridge GetBridgeTo(Pillar other)
    {
        if (other == null) return null;

        return Neighbors
            .Where(n => n is Bridge)
            .Select(n => (Bridge)n)
            .FirstOrDefault(b => b.Neighbors.Contains(other));
    }

    public override void Clear()
    {
        base.Clear();
        
        if (HasFacility)
        {
            BuiltFacility.Demolish();
            BuiltFacility = null;
        }
    }
}
