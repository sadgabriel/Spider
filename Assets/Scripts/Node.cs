using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Node : MonoBehaviour
{
    public List<Node> Neighbors { get; private set; } = new();

    public bool IsOccupied { get; set; } = false;

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
