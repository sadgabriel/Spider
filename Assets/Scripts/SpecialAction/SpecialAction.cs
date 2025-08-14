using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public abstract class SpecialAction
{
    public Texture2D Icon { get; set; }
    public string Name { get; set; }
    public bool IsActive { get; private set; } = false;
    public void Activate()
    {
        if (IsActive) return;
        IsActive = true;
        InputManager.Instance.OnMouseButtonDown += HandleMouseButtonDown;
        InputManager.Instance.OnKeyDown += HandleKeyDown;
    }

    public void Deactivate()
    {
        if (!IsActive) return;
        IsActive = false;
        InputManager.Instance.OnMouseButtonDown -= HandleMouseButtonDown;
        InputManager.Instance.OnKeyDown -= HandleKeyDown;
    }

    protected void FinishSpecialActionWithTurnEnd()
    {
        GameStateManager.Instance.EndPlayerTurn();
        FinishSpecialAction();
    }

    protected void FinishSpecialActionWithoutTurnEnd()
    {
        FinishSpecialAction();
    }

    private void FinishSpecialAction()
    {
        GameStateManager.Instance.ResetGameState();
    }

    private void HandleMouseButtonDown(int button, Vector3 position, GameObject clickedGO)
    {
        var gameStateManager = GameStateManager.Instance;
        if (gameStateManager.IsPlayerTurn && gameStateManager.CurrentGameState == GameState.SpecialAction && gameStateManager.IsIdleUiState)
        {
            HandleMouseButtonDownWhileSpecialAction(button, position, clickedGO);
        }
    }
    
    private void HandleKeyDown(HashSet<KeyCode> pressedKeys)
    {
        var gameStateManager = GameStateManager.Instance;
        if (gameStateManager.IsPlayerTurn && gameStateManager.IsIdleGameState && gameStateManager.IsIdleUiState && pressedKeys.Contains(KeyCode.Space))
        {
            StartSpecialAction();
        }
    }

    protected virtual void HandleMouseButtonDownWhileSpecialAction(int button, Vector3 position, GameObject clickedGO)
    {

    }

    protected virtual void StartSpecialAction()
    {
        GameStateManager.Instance.SetGameState(GameState.SpecialAction);
    }
}

