using System;
using UnityEngine;

public enum TurnState
{
    PlayerTurn,
    EnemyTurn
}

public class TurnSystem : MonoBehaviour
{
    private TurnState currentTurn = TurnState.PlayerTurn;
    private int turnCount = 1;

    public event Action<TurnState> OnTurnChanged;
    
    public static TurnSystem Instance { get; private set; }

    public TurnState CurrentTurn
    {
        get => currentTurn;
        private set => currentTurn = value;
    }
    public int TurnCount
    {
        get => turnCount;
        private set => turnCount = value;
    }

    private void Awake()
    {
        Instance = this;
    }

    public bool IsPlayerTurn(){
        return currentTurn == TurnState.PlayerTurn;
    }

    public bool IsEnemyTurn(){
        return currentTurn == TurnState.EnemyTurn;
    }

    public void EndPlayerTurn(){
        if (currentTurn == TurnState.PlayerTurn)
        {
            currentTurn = TurnState.EnemyTurn;
            OnTurnChanged?.Invoke(currentTurn);
        }
    }

    public void EndEnemyTurn(){
        if (currentTurn == TurnState.EnemyTurn)
        {
            currentTurn = TurnState.PlayerTurn;
            turnCount++;
            OnTurnChanged?.Invoke(currentTurn);
        }
    }
}
