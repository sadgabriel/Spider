using UnityEngine;
using System;

public enum TurnState
{
    PlayerTurn,
    EnemyTurn
}

class GameStateManager : MonoBehaviour
{
    public static GameStateManager Instance { get; private set; }
    private TurnState currentTurn = TurnState.PlayerTurn;
    private int turnCount = 1;

    public event Action<TurnState> OnTurnChanged;

    public TurnState CurrentTurn
    {
        get => currentTurn;
        private set
        {
            if (currentTurn == value) return;
            currentTurn = value;
            OnTurnChanged?.Invoke(currentTurn);
        }
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
        if (CurrentTurn == TurnState.PlayerTurn)
        {
            CurrentTurn = TurnState.EnemyTurn;
        }
    }

    public void EndEnemyTurn(){
        if (CurrentTurn == TurnState.EnemyTurn)
        {
            CurrentTurn = TurnState.PlayerTurn;
            turnCount++;
        }
    }
}