using UnityEngine;
using System.Linq;

public class Player : Unit
{
    public static Player Instance { get; private set; }
    [SerializeField] private int life = 3;
    [SerializeField] private int maxLife = 3;
    [SerializeField] private int stamina = 0;
    [SerializeField] private int maxStamina = 3;
    
    public int Life
    {
        get => life;
        private set
        {
            life = Mathf.Clamp(value, 0, maxLife);
            if (life <= 0)
            {
                Debug.Log("Game Over");
            }
        }
    }

    public int Stamina
    {
        get => stamina;
        private set
        {
            stamina = Mathf.Clamp(value, 0, maxStamina);
        }
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


    private void OnEnable()
    {
        GameStateManager.Instance.OnTurnChanged += HandleTurnChanged;
        InputManager.Instance.OnMouseClicked += HandleMouseClicked;
    }

    private void OnDisable()
    {
        GameStateManager.Instance.OnTurnChanged -= HandleTurnChanged;
        InputManager.Instance.OnMouseClicked -= HandleMouseClicked;
    }

    public void Initialize(Node startNode)
    {
        MoveTo(startNode);
    }

    public void TakeDamage(int damage)
    {
        Life -= damage;
    }

    public override bool CanMoveTo(Node targetNode)
    {
        return targetNode != null &&
            CurrentNode.Neighbors
                .Where(n => !n.IsOccupied)
                .SelectMany(n => n.Neighbors)
                .Any(n => n == targetNode && !n.IsOccupied);
    }

    private void HandleMouseClicked(int button, Vector3 position)
    {
        switch (button)
        {
            case 0: // Left click
                if (TryMoveToClickedNode(position))
                {
                    GameStateManager.Instance.EndPlayerTurn();
                }
                break;
        }
    }

    private void HandleTurnChanged(TurnState turnState)
    {
        if (turnState == TurnState.PlayerTurn)
        {
            Stamina += 1;
        }
    }

    private bool TryMoveToClickedNode(Vector3 position)
    {
        Ray ray = Camera.main.ScreenPointToRay(position);
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
