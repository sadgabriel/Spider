using UnityEngine;
using UnityEngine.SceneManagement;

public class GameClearPanel : Panel<NoData>
{
    public override void Show(NoData data = default)
    {
        gameObject.SetActive(true);
    }

    public void RestartGame()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}