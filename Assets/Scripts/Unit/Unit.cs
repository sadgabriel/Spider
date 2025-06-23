using UnityEngine;

public abstract class Unit : MonoBehaviour
{
    [SerializeField] protected float verticalOffset = 0.5f;

    private Node currentNode;

    public Node CurrentNode
    {
        get => currentNode;
        protected set => currentNode = value;
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

    public virtual void MoveTo(Node targetNode)
    {
        if (currentNode != null)
        {
            CurrentNode.OccupyingUnit = null;
        }

        CurrentNode = targetNode;
        transform.position = CalcUnitPosition(targetNode);
        transform.rotation = CalcUnitRotation(targetNode);
        CurrentNode.OccupyingUnit = this;
    }

    public abstract void Die();
    
    protected Vector3 CalcUnitPosition(Node node)
    {
        return node.TopPosition + node.transform.up * verticalOffset;
    }

    protected Quaternion CalcUnitRotation(Node node)
    {
        Vector3 forward = Vector3.ProjectOnPlane(transform.forward, node.DirectionFromOrigin).normalized;

        if (forward == Vector3.zero)
            forward = Vector3.Cross(node.DirectionFromOrigin, Vector3.right);

        return Quaternion.LookRotation(forward, node.DirectionFromOrigin);
    }

    protected virtual void OnDestroy()
    {
        if (CurrentNode != null)
        {
            CurrentNode.OccupyingUnit = null;
        }
    }
}