using UnityEngine;
using System.Linq;

public class Player : Unit
{
    public static Player Instance { get; private set; }
    [SerializeField] private int life = 1;
    
    public int Life
    {
        get => life;
        private set => life = value;
    }

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
    }

    private void Update()
    {
        if (!GameStateManager.Instance.IsPlayerTurn()) return;

        if (Input.GetMouseButtonDown(0))
        {
            if (TryMoveToClickedNode())
            {
                GameStateManager.Instance.EndPlayerTurn();
            }
        }
    }

    public void Initialize(Node startNode)
    {
        MoveTo(startNode);
    }

    public void TakeDamage(int damage)
    {
        life -= damage;

        if (life <= 0)
        {
            Debug.Log("Game Over");
        }
    }

    public override bool CanMoveTo(Node targetNode)
    {
        return targetNode != null &&
            CurrentNode.Neighbors
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

    protected override void OnDestroy()
    {
        base.OnDestroy();
        if (Instance == this)
        {
            Instance = null;
        }
    }
}
