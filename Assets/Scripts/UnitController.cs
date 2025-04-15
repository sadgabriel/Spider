using UnityEngine;

public class UnitController : MonoBehaviour
{
    public Node CurrentNode { get; protected set; }

    protected float yOffset;

    public void SetStartNode(Node startNode, float yOffset)
    {
        this.yOffset = yOffset;
        CurrentNode = startNode;
        transform.position = startNode.transform.position + Vector3.up * yOffset;
        startNode.SetOccupier(this);
    }

    public bool TryMoveTo(Node targetNode)
    {
        if (CanMoveTo(targetNode))
        {
            MoveTo(targetNode);
            return true;
        }
        return false;
    }

    public virtual bool CanMoveTo(Node targetNode)
    {
        return targetNode != null &&
               CurrentNode.Neighbors.Contains(targetNode) &&
               !targetNode.IsOccupied;
    }

    protected void MoveTo(Node targetNode)
    {
        CurrentNode.ClearOccupier();

        CurrentNode = targetNode;
        transform.position = targetNode.transform.position + Vector3.up * yOffset;

        CurrentNode.SetOccupier(this);
    }

    protected virtual void OnDestroy()
    {
        if (CurrentNode != null)
        {
            CurrentNode.ClearOccupier();
        }
    }
}
