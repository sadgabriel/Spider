using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements;

public class PlayerController : MonoBehaviour
{
    public Node currentNode { get; private set; }
    private float yOffset;

    public void SetStartNode(Node startNode, float yOffset)
    {
        this.yOffset = yOffset;
        currentNode = startNode;
        transform.position = startNode.transform.position + Vector3.up * yOffset;  
    }

    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            TryMoveToClickedNode();
        }
    }

    private void TryMoveToClickedNode()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out RaycastHit hit))
        {
            Node targetNode = hit.collider.GetComponent<Node>();
            if (targetNode != null && CanMoveTo(targetNode))
            {
                MoveTo(targetNode);
            }
        }
    }

    public void MoveTo(Node target)
    {
        currentNode = target;
        transform.position = target.transform.position + Vector3.up * yOffset;
    }

    private bool CanMoveTo(Node targetNode)
    {
        if (targetNode != null)
        {
            return currentNode.neighbors.Contains(targetNode);
        }
        return false;
    }
}
