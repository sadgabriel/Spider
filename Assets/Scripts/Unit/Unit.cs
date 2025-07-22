using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;

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
        PutOn(targetNode);
    }

    public virtual IEnumerator DoTryMoveTo(Node targetNode)
    {
        if (CanMoveTo(targetNode))
        {
            yield return DoMoveTo(targetNode);
        }
    }

    protected virtual IEnumerator DoMoveTo(Node targetNode, float duration = 0.5f)
    {
        if (currentNode == null)
        {
            Debug.LogWarning("Current node is null, cannot move to target node.");
            yield break;
        }

        List<Node> path = Map.Instance.FindPath(CurrentNode, targetNode);
        if (path == null || path.Count == 0)
        {
            Debug.LogWarning("No valid path found from " + CurrentNode + " to " + targetNode);
            yield break;
        }

        if (path.Count == 1)
        {
            yield break;
        }

        CurrentNode.OccupyingUnit = null;
        targetNode.OccupyingUnit = this;
        CurrentNode = targetNode;

        for (int i = 0; i < path.Count - 1; i++)
        {
            Node startNode = path[i];
            Node endNode = path[i + 1];

            yield return DoMoveStepTo(startNode, endNode, duration / (path.Count - 1));
        }
    }

    protected virtual IEnumerator DoMoveStepTo(Node startNode, Node endNode, float duration = 0.25f)
    {
        float elapsed = 0f;

        while (elapsed < duration)
        {
            if (this == null || gameObject == null || this is Enemy enemy && enemy.IsDead)
            {
                yield break;
            }

            Vector3 startPosition = CalcUnitPosition(startNode);
            Vector3 endPosition = CalcUnitPosition(endNode);

            float t = elapsed / duration;
            transform.position = Vector3.Lerp(startPosition, endPosition, t);

            LookAt(endNode);
            elapsed += Time.deltaTime;
            yield return null;
        }

        transform.position = CalcUnitPosition(endNode);
        transform.rotation = CalcUnitRotation(endNode);
    }

    public void PutOn(Node targetNode)
    {
        if (targetNode == null || (targetNode.IsOccupied && targetNode.OccupyingUnit != this && !(targetNode.OccupyingUnit is Enemy enemy && enemy.IsDead)))
        {
            Debug.LogWarning("Cannot put unit on the node: " + (targetNode == null ? "Node is null" : "Node is occupied"));
            return;
        }

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

    protected void LookAt(Node targetNode = null)
    {
        if (this == null || gameObject == null || this is Enemy enemy && enemy.IsDead)
        {
            return;
        }

        if (targetNode == null)
        {
            transform.rotation = CalcUnitRotation(CurrentNode);
            return;
        }

        transform.rotation = CalcRotationTo(targetNode);
    }

    protected Quaternion CalcRotationTo(Node targetNode)
    {
        Vector3 surfaceNormal = transform.position - Map.Instance.Origin;

        Vector3 forward = Vector3.ProjectOnPlane(
            CalcUnitPosition(targetNode) - transform.position,
            surfaceNormal
        ).normalized;

        if (forward == Vector3.zero)
        {
            forward = Vector3.Cross(surfaceNormal, Vector3.right);
        }

        if (forward == Vector3.zero)
        {
            forward = Vector3.forward;
        }

        return Quaternion.LookRotation(forward, surfaceNormal);
    }

    protected virtual void OnDestroy()
    {
        if (CurrentNode != null)
        {
            CurrentNode.OccupyingUnit = null;
        }
    }
}