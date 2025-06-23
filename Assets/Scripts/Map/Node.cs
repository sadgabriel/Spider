using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;

public class Node : MonoBehaviour
{
    public List<Node> Neighbors
    {
        get => neighbors.Where(n => n.isActiveAndEnabled).ToList();
    }

    public List<Node> AllNeighbors
    {
        get => neighbors;
    }

    public bool IsOccupied => OccupyingUnit != null;
    public Unit OccupyingUnit { get; set; } = null;

    private List<Node> neighbors = new();

    public Vector3 TopPosition
    {
        get
        {
            var mesh = GetComponent<MeshFilter>().sharedMesh;
            float halfHeight = mesh.bounds.extents.y * transform.lossyScale.y;
            return transform.position + transform.up * halfHeight;
        }
    }

    public Vector3 DirectionFromOrigin
    {
        get
        {
            Vector3 origin = transform.parent.position;
            return (transform.position - origin).normalized;
        }
    }

    public void ConnectTo(Node other)
    {
        if (!neighbors.Contains(other))
        {
            neighbors.Add(other);
            other.neighbors.Add(this);
        }
    }

    public void DisconnectFrom(Node other)
    {
        if (neighbors.Contains(other))
        {
            neighbors.Remove(other);
            other.neighbors.Remove(this);
        }
    }
}
