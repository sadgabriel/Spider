using UnityEngine;
using System.Linq;
using System.Collections;
using System.Collections.Generic;

public class Player : Unit
{
    public static Player Instance { get; private set; }
    [SerializeField] private int hp = 100;
    [SerializeField] private int maxHP = 100;
    [SerializeField] private int hpRegen = 0;
    [SerializeField] private int stamina = 0;
    [SerializeField] private int maxStamina = 3;
    [SerializeField] private int staminaRegen = 1;

    private int exp = 0;
    private int level = 1;

    public event System.Action<int> OnLevelUp;
    
    public int HP
    {
        get => hp;
        set
        {
            hp = Mathf.Clamp(value, 0, maxHP);
            if (hp <= 0)
            {
                Debug.Log("Game Over");
            }
        }
    }

    public int MaxHP
    {
        get => maxHP;
        set
        {
            int diff = value - maxHP;
            maxHP = value;

            if (diff > 0)
            {
                HP += diff;
            }

            if (HP > maxHP)
            {
                HP = maxHP;
            }
        }
    }

    public int HPRegen
    {
        get => hpRegen;
        set => hpRegen = value;
    }

    public int Stamina
    {
        get => stamina;
        set
        {
            stamina = Mathf.Clamp(value, 0, maxStamina);
        }
    }

    public int MaxStamina
    {
        get => maxStamina;
        set => maxStamina = value;
    }

    public int StaminaRegen
    {
        get => staminaRegen;
        set => staminaRegen = value;
    }


    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void Initialize(Node startNode)
    {
        MoveTo(startNode);
    }

    public void TakeDamage(int damage)
    {
        HP -= damage;
    }

    public void RegenerateHP(int amount)
    {
        HP += amount;
    }

    public void RegenerateHP()
    {
        RegenerateHP(hpRegen);
    }

    public void RegenerateStamina(int amount)
    {
        Stamina += amount;
    }

    public void RegenerateStamina()
    {
        RegenerateStamina(staminaRegen);
    }

    public void GainExperience(int amount)
    {
        exp += amount;
        if (exp >= level * 100)
        {
            exp -= level * 100;
            level++;
            OnLevelUp?.Invoke(level);
        }
    }

    public override bool CanMoveTo(Node targetNode)
    {
        return CanMoveTo(targetNode, 1);
    }

    private bool CanMoveTo(Node targetNode, int maxPillarDistance)
    {
        if (targetNode != null && targetNode is Pillar && maxPillarDistance > 0 && (!targetNode.IsOccupied || targetNode.OccupyingUnit is Spawner))
        {
            int nodeDistance = Map.Instance.CalcPathDistance(CurrentNode, targetNode);
            if (nodeDistance != -1 && nodeDistance <= 2 * maxPillarDistance)
            {
                return true;
            }
        }
        return false;
    }

    public override void MoveTo(Node targetNode)
    {
        if (targetNode.OccupyingUnit is Spawner spawner)
        {
            Destroy(spawner.gameObject);
            GainExperience(100);
        }

        base.MoveTo(targetNode);
    }

    public bool TrySprintTo(Node targetNode, int maxPillarDistance)
    {
        if (CanMoveTo(targetNode, maxPillarDistance))
        {
            MoveTo(targetNode);
            return true;
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
