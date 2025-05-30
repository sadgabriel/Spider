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

    public bool HasFacility { get; set; } = false;
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
}
