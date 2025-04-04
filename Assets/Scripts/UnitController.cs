using UnityEngine;

public class UnitController : MonoBehaviour
{
    public Node currentNode { get; protected set; }
    protected float yOffset;

    public void SetStartNode(Node startNode, float yOffset)
    {
        this.yOffset = yOffset;
        currentNode = startNode;
        transform.position = startNode.transform.position + Vector3.up * yOffset;
        startNode.SetOccupier(this);
    }

    public bool TryMoveTo(Node targetNode)
    {
        if (targetNode != null && CanMoveTo(targetNode))
        {
            MoveTo(targetNode);
            return true;
        }
        return false;
    }

    protected void MoveTo(Node targetNode)
    {
        currentNode.ClearOccupier();

        currentNode = targetNode;
        transform.position = targetNode.transform.position + Vector3.up * yOffset;
    
        currentNode.SetOccupier(this);
    }

    public virtual bool CanMoveTo(Node targetNode)
    {
        if (targetNode != null)
        {
            return currentNode.neighbors.Contains(targetNode) && !targetNode.IsOccupied;
        }
        return false;
    }

    protected virtual void OnDestroy()
    {
        currentNode.ClearOccupier();
    }
}
