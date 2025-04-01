using UnityEngine;

public class GameManager : MonoBehaviour
{
    [SerializeField] private MapManager mapManager;
    [SerializeField] private PlayerController playerPrefab;

    private PlayerController player;
    void Start()
    {
        InitializeGame();
    }

    private void InitializeGame()
    {
        mapManager.GenerateMap();

        InitializePlayer();
    }

    private void InitializePlayer()
    {
        Node startNode = mapManager.GetStartNode();
        player = Instantiate(playerPrefab);
        player.SetStartNode(startNode, mapManager.GetPlayerYOffset());
    }
}
