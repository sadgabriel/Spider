using UnityEngine;
using System.Collections.Generic;

class Sprint : SpecialAction
{
    public int Reach = 2;
    public int BaseStaminaConsume = 50;

    protected override void HandleMouseButtonDown(int button, Vector3 position, GameObject clickedGO)
    {
        if (button == 0 && GameStateManager.Instance.IsPlayerTurn && GameStateManager.Instance.IsSpecialActionState)
        {
            Node targetNode = clickedGO.GetComponent<Node>();

            if (targetNode == null)
            {
                Facility facility = clickedGO.GetComponentInParent<Facility>();
                targetNode = facility?.CurrentPillar;
            }

            if (targetNode != null)
            {
                if (Player.Instance.TrySprintTo(targetNode, Reach, BaseStaminaConsume))
                {
                    GameStateManager.Instance.EndPlayerTurn();
                }
                GameStateManager.Instance.ResetGameState();
            }
        }
    }

    protected override void HandleKeyDown(HashSet<KeyCode> pressedKeys)
    {
        var gameStateManager = GameStateManager.Instance;
        if (pressedKeys.Contains(KeyCode.Space) && gameStateManager.IsPlayerTurn && gameStateManager.IsIdleGameState && gameStateManager.IsIdleUIState)
        {
            GameStateManager.Instance.SetSpecialActionGameState();
        }
    }
}

