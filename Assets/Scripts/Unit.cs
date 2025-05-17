using UnityEngine;

public abstract class Unit : MonoBehaviour
{
    private Node currentNode;

    public Node CurrentNode
    {
        get => currentNode;
        protected set => currentNode = value;
    }

    [SerializeField] protected float verticalOffset = 0.5f;

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
        if (targetNode == null)
        {
            Debug.LogError("Target node is null.");
            return;
        }

        if (targetNode.IsOccupied)
        {
            Debug.LogError("Target node is occupied.");
            return;
        }
        
        if (currentNode != null)
        {
            CurrentNode.IsOccupied = false;
        }

        CurrentNode = targetNode;
        transform.position = CalcUnitPosition(targetNode);
        transform.rotation = CalcUnitRotation(targetNode);
        CurrentNode.IsOccupied = true;
    }

    protected Vector3 CalcUnitPosition(Node node)
    {
        return node.TopPosition + node.transform.up * verticalOffset;
    }

    protected Quaternion CalcUnitRotation(Node node)
    {
        Vector3 forward = Vector3.ProjectOnPlane(transform.forward, node.transform.up).normalized;

        if (forward == Vector3.zero)
            forward = Vector3.Cross(node.transform.up, Vector3.right);

        return Quaternion.LookRotation(forward, node.transform.up);
    }

    protected virtual void OnDestroy()
    {
        if (CurrentNode != null)
        {
            CurrentNode.IsOccupied = false;
        }
    }
}