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
    Sprint,
    Teleport,
}

public enum UiState
{
    Idle,
    FacilitySelection,
    FacilityBuilding,
}

public class GameStateManager : MonoBehaviour
{
    public static GameStateManager Instance { get; private set; }

    private TurnState currentTurn = TurnState.PlayerTurn;
    private GameState currentGameState = GameState.Idle;
    private UiState currentUiState = UiState.Idle;
    private object uiStateData;

    public event Action<TurnState> OnTurnChange;
    public event Action<GameState> OnGameStateChange;
    public event Action<UiState, object> OnUiStateChange;

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

    public GameState CurrentGameState
    {
        get => currentGameState;
        private set
        {
            if (currentGameState == value) return;
            currentGameState = value;
            OnGameStateChange?.Invoke(currentGameState);
        }
    }

    public UiState CurrentUiState
    {
        get => currentUiState;
        private set
        {
            if (currentUiState == value) return;
            currentUiState = value;
            OnUiStateChange?.Invoke(currentUiState, uiStateData);
        }
    }

    public int TurnCount { get; private set; } = 1;

    public bool IsPlayerTurn => CurrentTurn == TurnState.PlayerTurn;
    public bool IsEnemyTurn => CurrentTurn == TurnState.EnemyTurn;

    public bool IsIdleGameState => CurrentGameState == GameState.Idle;

    public bool IsIdleUiState => CurrentUiState == UiState.Idle;

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
    }

    public void EndPlayerTurn()
    {
        if (IsPlayerTurn)
            CurrentTurn = TurnState.EnemyTurn;
    }

    public void EndEnemyTurn()
    {
        if (IsEnemyTurn)
        {
            CurrentTurn = TurnState.PlayerTurn;
            TurnCount++;
        }
    }

    public void SetGameState(GameState state)
    {
        CurrentGameState = state;
    }
    
    public void ResetGameState() => CurrentGameState = GameState.Idle;

    public void SetUiState(UiState state, object data)
    {
        uiStateData = data;
        CurrentUiState = state;
    }

    public void ResetUiState() => SetUiState(UiState.Idle, null);
}