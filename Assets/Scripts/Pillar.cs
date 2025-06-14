using System;
using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
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

    public bool HasFacility { get; set; } = false;

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
}
