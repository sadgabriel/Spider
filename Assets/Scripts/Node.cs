using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Node : MonoBehaviour
{
    public List<Node> Neighbors { get; private set; } = new();

    public bool IsOccupied { get; set; } = false;

    public Vector3 Position
    {
        get => transform.position + transform.up * GetComponent<Renderer>().bounds.extents.y;
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
