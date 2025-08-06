using UnityEngine;
using UnityEngine.SceneManagement;

public class TitleUi : MonoBehaviour
{
    [SerializeField] private GameObject creditPanel;

    public void OnClickStartGame()
    {
        SceneManager.LoadScene("GameScene");
    }

    public void OnClickCredit()
    {
        creditPanel.SetActive(true);
    }

    public void OnClickBackFromCredit()
    {
        creditPanel.SetActive(false);
    }

    public void OnClickExit()
    {
        Application.Quit();
        Debug.Log("Game Exited");
    }
}
