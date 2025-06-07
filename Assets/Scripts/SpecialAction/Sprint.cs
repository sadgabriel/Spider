using UnityEngine;
using System.Collections.Generic;

class Sprint : SpecialAction
{
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
            
            if (targetNode != null && Player.Instance.TrySprintTo(targetNode, 2))
            {
                GameStateManager.Instance.ResetGameState();
                GameStateManager.Instance.EndPlayerTurn();
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

