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

    public event Action<TurnState> OnTurnChange;
    public event Action<GameState> OnStateChange;

    public TurnState CurrentTurn
    {
        get => currentTurn;
        private set
        {
            if (currentTurn == value) return;
            currentTurn = value;
            OnTurnChange?.Invoke(currentTurn);
        }
    }

    public GameState CurrentState
    {
        get => currentState;
        private set
        {
            if (currentState == value) return;
            currentState = value;
            OnStateChange?.Invoke(currentState);
        }
    }

    public int TurnCount
    {
        get => turnCount;
        private set => turnCount = value;
    }

    public bool IsPlayerTurn
    {
        get => currentTurn == TurnState.PlayerTurn;
    }

    public bool IsEnemyTurn
    {
        get => currentTurn == TurnState.EnemyTurn;
    }

    public bool IsIdle
    {
        get => currentState == GameState.Idle;
    }

    public bool IsSpecialActionState
    {
        get => currentState == GameState.SpecialAction;
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

    public void SetSpecialActionState()
    {
        CurrentState = GameState.SpecialAction;
    }

    public void ResetState()
    {
        CurrentState = GameState.Idle;
    }
}