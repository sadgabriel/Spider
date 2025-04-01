using UnityEngine;
using System.Collections.Generic;

public class Node : MonoBehaviour
{
    public List<Node> neighbors = new();
    
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
