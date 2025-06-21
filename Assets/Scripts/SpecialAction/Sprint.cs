using UnityEngine;
using System.Collections.Generic;
using Unity.VisualScripting;

class Sprint : SpecialAction
{
    public static readonly Sprint Instance = new();
    private Sprint() { }

    public int Reach { get; set; } = 2;
    public int BaseStaminaConsume { get; set; } = 50;

    protected override void HandleMouseButtonDown(int button, Vector3 position, GameObject clickedGO)
    {
        var gameStateManager = GameStateManager.Instance;
        if (button == 0 && gameStateManager.IsPlayerTurn && gameStateManager.CurrentGameState == GameState.Sprint && gameStateManager.IsIdleUiState)
        {
            Node targetNode = Utils.GetNodeFromGameObject(clickedGO);

            if (targetNode != null)
            {
                if (Player.Instance.TrySprintTo(targetNode, Reach, BaseStaminaConsume))
                {
                    gameStateManager.EndPlayerTurn();
                }
                gameStateManager.ResetGameState();
            }
        }
    }

    protected override void HandleKeyDown(HashSet<KeyCode> pressedKeys)
    {
        var gameStateManager = GameStateManager.Instance;
        if (pressedKeys.Contains(KeyCode.Space) && gameStateManager.IsPlayerTurn && gameStateManager.IsIdleGameState && gameStateManager.IsIdleUiState)
        {
            gameStateManager.SetGameState(GameState.Sprint);
        }
    }
}

