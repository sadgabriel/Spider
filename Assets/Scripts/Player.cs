using UnityEngine;
using System.Linq;

public class Player : MonoBehaviour, IPlayer
{
    private Node currentNode;
    
    [SerializeField] private int life;
    [SerializeField] private float yOffset = 0.5f;
    
    public int Life
    {
        get => life;
        protected set => life = value;
    }

    public Node CurrentNode
    {
        get => currentNode;
        protected set => currentNode = value;
    }

    private void Update()
    {
        if (!TurnSystem.Instance.IsPlayerTurn()) return;

        if (Input.GetMouseButtonDown(0))
        {
            if (TryMoveToClickedNode())
            {
                TurnSystem.Instance.EndPlayerTurn();
            }
        }
    }

    public void Initialize(Node startNode)
    {
        currentNode = startNode;
        transform.position = GetPosition(startNode);
        currentNode.IsOccupied = true;
    }

    public void TakeDamage(int damage)
    {
        life -= damage;

        if (life <= 0)
        {
            Debug.Log("Game Over");
        }
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

    public bool CanMoveTo(Node targetNode)
    {
        return targetNode != null &&
            currentNode.Neighbors
                .Where(n => !n.IsOccupied)
                .SelectMany(n => n.Neighbors)
                .Any(n => n == targetNode && !n.IsOccupied);
    }

    private bool TryMoveToClickedNode()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out RaycastHit hit))
        {
            Node targetNode = hit.collider.GetComponent<Node>();

            if (targetNode != null && TryMoveTo(targetNode))
            {
                return true;
            }
        }

        return false;
    }

    private void MoveTo(Node targetNode)
    {
        if (currentNode != null)
        {
            currentNode.IsOccupied = false;
        }

        currentNode = targetNode;
        transform.position = GetPosition(targetNode);
        currentNode.IsOccupied = true;
    }

    private Vector3 GetPosition(Node node)
    {
        return node.Position + Vector3.up * yOffset;
    }

    private void OnDestroy()
    {
        if (currentNode != null)
        {
            currentNode.IsOccupied = false;
        }
    }
}
