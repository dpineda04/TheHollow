using UnityEngine;
using UnityEngine.SceneManagement;

public class GameOverManager : MonoBehaviour
{
    public static GameOverManager Instance { get; private set; }
    public string mainMenuSceneName = "MainMenu";

    // hook this to a UI VisualElement or CanvasGroup in your UI code
    public GameObject gameOverUI; // assign a GameObject (UIDocument wrapper or Canvas) to show

    void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    public void ShowGameOver()
    {
        Debug.Log("[GameOverManager] ShowGameOver");
        if (gameOverUI != null) gameOverUI.SetActive(true);
        // pause game (optional)
        Time.timeScale = 0f;
    }

    public void Restart()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void QuitToMenu()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(mainMenuSceneName);
    }
}
