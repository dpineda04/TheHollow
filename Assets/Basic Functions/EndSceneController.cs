using UnityEngine;

public class EndSceneController : MonoBehaviour
{
    [Tooltip("Name of the main menu scene to load")]
    public string mainMenu = "MainMenu";

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.KeypadEnter))
        {
            // Use fade controller to transition
            if (FadeController.Instance != null)
            {
                FadeController.Instance.FadeToScene(mainMenu);
            }
            else
            {
                UnityEngine.SceneManagement.SceneManager.LoadScene(mainMenu);
            }
        }
    }
}
