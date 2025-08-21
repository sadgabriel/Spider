using UnityEngine;

public class TeleportFacility : Facility
{
    private static TeleportFacility selectedTeleportFacility = null;
    private static bool handlerIsRegistered = false;

    private const int MaxUpgrade = 3;
    private const int MinUpgrade = 0;
    public override int MaxUpgradeLevel => MaxUpgrade;
    public override int MinUpgradeLevel => MinUpgrade;

    private int coolDown = 0;
    private int maxCoolDown = 20;

    public bool IsAvailable => ClampedUpgradeLevel > 0 && coolDown <= 0;

    public void StartCoolDown()
    {
        if (coolDown <= 0)
        {
            coolDown = maxCoolDown;
        }
        LightOff();
    }

    public void ResetCoolDown()
    {
        coolDown = 0;
    }

    public override void Demolish()
    {
        base.Demolish();
        if (handlerIsRegistered)
        {
            InputManager.Instance.OnMouseButtonDown -= HandleMouseButtonDown;
            handlerIsRegistered = false;
        }
        GameStateManager.Instance.OnTurnChange -= HandleTurnChange;
    }

    protected override void HandleUpgradeLevelIncrease(int level)
    {
        base.HandleUpgradeLevelIncrease(level);
        switch (level)
        {
            case 2:
            case 3:
                maxCoolDown -= 5;
                if (coolDown > maxCoolDown)
                {
                    coolDown = maxCoolDown;
                }
                break;
        }
    }

    protected override void HandleUpgradeLevelDecrease(int level)
    {
        base.HandleUpgradeLevelDecrease(level);
        switch (level)
        {
            case 2:
            case 3:
                maxCoolDown += 5;
                break;
        }
    }

    protected override void Initialize()
    {
        if (!handlerIsRegistered)
        {
            InputManager.Instance.OnMouseButtonDown += HandleMouseButtonDown;
            handlerIsRegistered = true;
        }

        GameStateManager.Instance.OnTurnChange += HandleTurnChange;
    }

    private static void HandleMouseButtonDown(int button, Vector3 position, GameObject clickedGO)
    {
        Node clickedNode = Utils.GetNodeFromGameObject(clickedGO);
        if (clickedNode != null && clickedNode is Pillar clickedPillar)
        {
            var gameStateManager = GameStateManager.Instance;
            if (!gameStateManager.IsPlayerTurn || !gameStateManager.IsIdleUiState) return;

            if (clickedPillar.BuiltFacility is TeleportFacility clickedTeleportFacility && clickedTeleportFacility.IsAvailable)
            {
                if (gameStateManager.IsIdleGameState && button == 1 && Map.Instance.CalcPathPillarDistance(clickedPillar, Player.Instance.CurrentNode) <= 1)
                {
                    gameStateManager.SetGameState(GameState.Teleport);
                    selectedTeleportFacility = clickedTeleportFacility;
                    selectedTeleportFacility.TurnOnBlink();
                    return;
                }
                else if (gameStateManager.CurrentGameState == GameState.Teleport && button == 0)
                {
                    if (clickedTeleportFacility != selectedTeleportFacility && !clickedPillar.IsOccupied && clickedTeleportFacility.ClampedUpgradeLevel > 0)
                    {
                        Player.Instance.PutOn(clickedPillar);
                        selectedTeleportFacility.StartCoolDown();
                        clickedTeleportFacility.StartCoolDown();

                        gameStateManager.ResetGameState();
                        selectedTeleportFacility.TurnOffBlink();
                        selectedTeleportFacility = null;
                        gameStateManager.EndPlayerTurn();
                        return;
                    }
                }
            }

            gameStateManager.ResetGameState();
            if (selectedTeleportFacility != null)
            {
                selectedTeleportFacility.TurnOffBlink();
                selectedTeleportFacility = null;
            }
        }
    }

    private void HandleTurnChange(TurnState turnState, int turnCount)
    {
        if (turnState == TurnState.PlayerTurn)
        {
            if (coolDown > 0)
            {
                coolDown--;
                if (coolDown == 0)
                {
                    LightOn();
                }
            }
        }
    }
}