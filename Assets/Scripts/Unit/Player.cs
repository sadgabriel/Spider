using UnityEngine;
using System.Linq;
using System.Collections;
using System.Collections.Generic;

public class Player : Unit
{
    public static Player Instance { get; private set; }
    [SerializeField] private int hp = 100;
    [SerializeField] private int maxHp = 100;
    [SerializeField] private int hpRegen = 0;
    [SerializeField] private int stamina = 0;
    [SerializeField] private int maxStamina = 100;
    [SerializeField] private int staminaRegen = 5;

    private int exp = 0;
    private int level = 1;

    public event System.Action<int> OnLevelUp;
    
    public int Hp
    {
        get => hp;
        set
        {
            hp = Mathf.Clamp(value, 0, maxHp);
            if (hp <= 0)
            {
                Debug.Log("Game Over");
            }
        }
    }

    public int MaxHp
    {
        get => maxHp;
        set
        {
            int diff = value - maxHp;
            maxHp = value;

            if (diff > 0)
            {
                hp += diff;
            }

            if (hp > maxHp)
            {
                hp = maxHp;
            }
        }
    }

    public int HpRegen
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
        set
        {
            int diff = value - maxStamina;
            maxStamina = value;

            if (diff > 0)
            {
                stamina += diff;
            }

            if (stamina > maxStamina)
            {
                stamina = maxStamina;
            }
        }
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
        Hp -= damage;
    }

    public void RegenerateHp(int amount)
    {
        Hp += amount;
    }

    public void RegenerateHp()
    {
        RegenerateHp(hpRegen);
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
        while (exp >= level * 100)
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
        if (targetNode != null && targetNode is Pillar && maxPillarDistance > 0 && (!targetNode.IsOccupied || targetNode.OccupyingUnit is Spawner || targetNode.OccupyingUnit == this))
        {
            int nodeDistance = Map.Instance.CalcPathNodeDistance(CurrentNode, targetNode);
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
            spawner.Die();
            GainExperience(spawner.ExpGain);
        }

        base.MoveTo(targetNode);
    }

    public bool TrySprintTo(Node targetNode, int maxPillarDistance, int baseStaminaConsume)
    {
        if (CanMoveTo(targetNode, maxPillarDistance))
        {
            int distance = Map.Instance.CalcPathPillarDistance(CurrentNode, targetNode);
            int staminaConsume;

            switch (distance)
            {
                case 2:
                    staminaConsume = baseStaminaConsume;
                    break;
                case 3:
                    staminaConsume = 2 * baseStaminaConsume;
                    break;
                case 4:
                    staminaConsume = 4 * baseStaminaConsume;
                    break;
                default:
                    staminaConsume = 0;
                    break;
            }

            if (Stamina >= staminaConsume)
            {
                Stamina -= staminaConsume;
                MoveTo(targetNode);
                return true;
            }
            else
            {
                Debug.Log("Not enough stamina.");
            }
        }
        else
        {
            Debug.Log("Target pillar is too far.");
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
