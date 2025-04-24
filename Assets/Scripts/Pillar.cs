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
}
