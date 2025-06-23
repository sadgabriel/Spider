using UnityEngine;
using System.Collections.Generic;
using Unity.VisualScripting;

class Sprint : SpecialAction
{
    public static readonly Sprint Instance = new();
    private Sprint() { }

    public int Reach { get; set; } = 2;
    public int BaseStaminaConsume { get; set; } = 50;

    protected override void HandleMouseButtonDownWhileSpecialAction(int button, Vector3 position, GameObject clickedGO)
    {
        base.HandleMouseButtonDownWhileSpecialAction(button, position, clickedGO);
        if (button == 0)
        {
            Node targetNode = Utils.GetNodeFromGameObject(clickedGO);

            if (targetNode != null)
            {
                if (Player.Instance.TrySprintTo(targetNode, Reach, BaseStaminaConsume))
                {
                    FinishSpecialActionWithTurnEnd();
                    return;
                }
                FinishSpecialActionWithoutTurnEnd();
            }
        }
    }
}

