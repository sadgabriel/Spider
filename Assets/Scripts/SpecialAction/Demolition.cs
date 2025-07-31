using UnityEngine;

public class Demolition : SpecialAction
{
    public static readonly Demolition Instance = new();
    private Demolition() { }

    public int StaminaConsume { get; set; } = 50;
    public int HpGain { get; set; } = 0;
    public bool NeedTurnEnd { get; set; } = true;

    protected override void HandleMouseButtonDownWhileSpecialAction(int button, Vector3 position, GameObject clickedGO)
    {
        base.HandleMouseButtonDownWhileSpecialAction(button, position, clickedGO);

        if (button == 0)
        {
            Node targetNode = Utils.GetNodeFromGameObject(clickedGO);

            if (targetNode != null && targetNode is Bridge bridge)
            {
                if (StaminaConsume > Player.Instance.Stamina)
                {
                    Debug.Log("Not enough stamina.");
                    FinishSpecialActionWithoutTurnEnd();
                    return;
                }

                Player.Instance.UseStamina(StaminaConsume);

                if (bridge.IsOccupied && bridge.OccupyingUnit is Enemy)
                {
                    Player.Instance.RegenerateHp(HpGain);
                }

                Map.Instance.DemolishBridge(bridge);
                AudioManager.Instance.PlayDemolitionSfx();

                if (NeedTurnEnd)
                {
                    FinishSpecialActionWithTurnEnd();
                }
                else
                {
                    FinishSpecialActionWithoutTurnEnd();
                }
                return;
            }
            FinishSpecialActionWithoutTurnEnd();
        }
    }
}