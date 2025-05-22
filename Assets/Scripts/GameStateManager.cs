using UnityEngine;
using System;

public enum TurnState
{
    PlayerTurn,
    EnemyTurn
}

public enum GameState
{
    Idle,
    SpecialAction
}

class GameStateManager : MonoBehaviour
{
    public static GameStateManager Instance { get; private set; }
    private TurnState currentTurn = TurnState.PlayerTurn;
    private GameState currentState = GameState.Idle;
    private int turnCount = 1;

    public event Action<TurnState> OnTurnChanged;
    public event Action<GameState> OnStateChanged;

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

    public GameState CurrentState
    {
        get => currentState;
        private set
        {
            if (currentState == value) return;
            currentState = value;
            OnStateChanged?.Invoke(currentState);
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

    public bool IsPlayerTurn()
    {
        return currentTurn == TurnState.PlayerTurn;
    }

    public bool IsEnemyTurn()
    {
        return currentTurn == TurnState.EnemyTurn;
    }

    public void EndPlayerTurn()
    {
        if (CurrentTurn == TurnState.PlayerTurn)
        {
            CurrentTurn = TurnState.EnemyTurn;
        }
    }

    public void EndEnemyTurn()
    {
        if (CurrentTurn == TurnState.EnemyTurn)
        {
            CurrentTurn = TurnState.PlayerTurn;
            turnCount++;
        }
    }

    public void UseSpecialAction()
    {
        if (CurrentState == GameState.Idle)
        {
            CurrentState = GameState.SpecialAction;
        }
    }

    public void ResetSpecialAction()
    {
        if (CurrentState == GameState.SpecialAction)
        {
            CurrentState = GameState.Idle;
        }
    }
}