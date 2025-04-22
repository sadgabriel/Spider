using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

public enum PillarSize
{
    Small,
    Large
}

public class Pillar : Node
{
    [SerializeField] private PillarSize size;
    public PillarSize Size 
    { 
        get => size; 
        set => size = value;
    }

    public List<Pillar> AdjacentPillars { get; private set; } = new();

    public Bridge CreateBridgeTo(Pillar other, GameObject bridgePrefab)
    {
        if (other == null || bridgePrefab == null) return null;

        Vector3 position = (transform.position + other.transform.position) / 2;
        Quaternion rotation = Quaternion.LookRotation(other.transform.position - transform.position);
        
        GameObject bridgeGO = Instantiate(bridgePrefab, position, rotation, transform);
        Bridge bridge = bridgeGO.GetComponent<Bridge>();

        float length = Vector3.Distance(transform.position, other.transform.position);
        Vector3 scale = bridge.transform.localScale;
        scale.z = length;

        bridge.transform.localScale = scale;
        
        ConnectTo(bridge);
        other.ConnectTo(bridge);

        AdjacentPillars.Add(other);
        other.AdjacentPillars.Add(this);

        return bridge;
    }
}
