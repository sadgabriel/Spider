using System.Collections.Generic;
using UnityEngine;

public enum NodeSize
{
    Small,
    Large
}

public class Node : MonoBehaviour
{
    public bool isSpawner;

    public NodeSize size;

    public bool IsLarge => size == NodeSize.Large;
    public bool IsSmall => size == NodeSize.Small;

    public List<Node> Neighbors { get; private set; } = new();

    public UnitController Occupier { get; private set; }
    public bool IsOccupied => Occupier != null;

    

    public bool SetOccupier(UnitController unit)
    {
        if (!IsOccupied)
        {
            Occupier = unit;
            return true;
        }

        return false;
    }

    public void ClearOccupier()
    {
        Occupier = null;
    }

    public void ConnectTo(Node other)
    {
        if (!Neighbors.Contains(other))
        {
            Neighbors.Add(other);
            other.Neighbors.Add(this);
        }
    }

    public void DisconnectFrom(Node other)
    {
        if (Neighbors.Contains(other))
        {
            Neighbors.Remove(other);
            other.Neighbors.Remove(this);
        }
    }
}
