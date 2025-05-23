using UnityEngine;
using System.Collections.Generic;

namespace SpecialAction
{
    class Sprint : SpecialAction
    {
        protected override void HandleMouseButtonDown(int button, Vector3 position, GameObject clickedGO)
        {
            if (button == 0 && GameStateManager.Instance.IsPlayerTurn && GameStateManager.Instance.IsSpecialActionState)
            {
                Node targetNode = clickedGO.GetComponent<Node>();
                
                if (targetNode != null &&  Player.Instance.TrySprintTo(targetNode, 2))
                {
                    GameStateManager.Instance.ResetState();
                    GameStateManager.Instance.EndPlayerTurn();
                }
            }
        }

        protected override void HandleKeyDown(HashSet<KeyCode> pressedKeys)
        {
            if (pressedKeys.Contains(KeyCode.Space) && GameStateManager.Instance.IsPlayerTurn && GameStateManager.Instance.IsIdle)
            {
                GameStateManager.Instance.SetSpecialActionState();
            }
        }
    }
}
