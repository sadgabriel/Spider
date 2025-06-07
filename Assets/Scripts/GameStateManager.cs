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
    SpecialAction,
}

public enum UIState
{
    Idle,
    FacilitySelection,
}

class GameStateManager : MonoBehaviour
{
    public static GameStateManager Instance { get; private set; }
    private TurnState currentTurn = TurnState.PlayerTurn;
    private GameState currentGameState = GameState.Idle;
    private UIState currentUIState = UIState.Idle;
    private int turnCount = 1;

    public event Action<TurnState> OnTurnChange;
    public event Action<GameState> OnGameStateChange;
    public event Action<UIState> OnUIStateChange;

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

    public UIState CurrentUIState
    {
        get => currentUIState;
        private set
        {
            if (currentUIState == value) return;
            currentUIState = value;
            OnUIStateChange?.Invoke(currentUIState);
        }
    }

    public int TurnCount
    {
        get => turnCount;
        private set => turnCount = value;
    }

    public bool IsPlayerTurn
    {
        get => CurrentTurn == TurnState.PlayerTurn;
    }

    public bool IsEnemyTurn
    {
        get => CurrentTurn == TurnState.EnemyTurn;
    }

    public bool IsIdleGameState
    {
        get => CurrentGameState == GameState.Idle;
    }

    public bool IsSpecialActionState
    {
        get => CurrentGameState == GameState.SpecialAction;
    }

    public bool IsIdleUIState
    {
        get => currentUIState == UIState.Idle;
    }

    public bool IsFacilitySelectionState
    {
        get => currentUIState == UIState.FacilitySelection;
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

    public void SetSpecialActionGameState()
    {
        CurrentGameState = GameState.SpecialAction;
    }

    public void ResetGameState()
    {
        CurrentGameState = GameState.Idle;
    }

    public void SetFacilitySelectionUIState()
    {
        CurrentUIState = UIState.FacilitySelection;
    }

    public void ResetUIState()
    {
        CurrentUIState = UIState.Idle;
    }

}