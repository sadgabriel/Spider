using UnityEngine;
using System.Linq;
using System.Collections;
using System.Collections.Generic;

public class Player : Unit
{
    public static Player Instance { get; private set; }
    [SerializeField] private int life = 3;
    [SerializeField] private int maxLife = 3;
    [SerializeField] private int stamina = 0;
    [SerializeField] private int maxStamina = 3;
    [SerializeField] private int staminaRegen = 1;
    
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

    public void Initialize(Node startNode)
    {
        MoveTo(startNode);
    }

    public void TakeDamage(int damage)
    {
        Life -= damage;
    }

    public void RegenerateStamina(int amount)
    {
        Stamina += amount;
    }

    public void RegenerateStamina()
    {
        RegenerateStamina(staminaRegen);
    }

    public override bool CanMoveTo(Node targetNode)
    {
        return CanMoveTo(targetNode, 1);
    }

    public bool CanMoveTo(Node targetNode, int maxDistance)
    {
        if (targetNode == null || targetNode is not Pillar || targetNode.IsOccupied || maxDistance <= 0)
        {
            return false;
        }

        IEnumerable<Node> reachableNodes = new List<Node> { CurrentNode };

        for (int i = 0; i < 2 * maxDistance; i++)
        {
            reachableNodes = reachableNodes
                .SelectMany(n => n.Neighbors)
                .Where(n => !n.IsOccupied)
                .Distinct();
        }
        
        return reachableNodes.Contains(targetNode);
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
