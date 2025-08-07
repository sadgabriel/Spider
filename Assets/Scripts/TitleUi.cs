using UnityEngine;
using UnityEngine.SceneManagement;

public class TitleUi : MonoBehaviour
{

    public void OnClickStartGame()
    {
        SceneManager.LoadScene("GameScene");
    }

    public void OnClickExit()
    {
        Application.Quit();
        Debug.Log("Game Exited");
    }
}
