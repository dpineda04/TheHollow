using UnityEngine;
using System.Collections;
using UnityEngine.Playables;     // Timeline/PlayableDirector
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    [Header("Scene Names")]
    public string endScene = "EndScene";    // your end screen scene
    public string mainMenu = "MainMenu";

    private bool waitingForInput = false;

    private void Awake()
    {
        // Singleton setup
        if (Instance == null) Instance = this;
        else Destroy(gameObject);

        DontDestroyOnLoad(gameObject);
    }

    public void FinishGame()
    {
        Debug.Log("Game Finished! Fading to end scene...");

       // if (FadeController.Instance != null)
       // {
         FadeController.Instance.FadeToScene(endScene);
        //}
        //else
       // {
            // fallback if FadeController missing
        //    SceneManager.LoadScene(endScene);
        //}
        
        //SceneManager.LoadScene(endScene);   // load your end scene
        
        waitingForInput = true;
    }

    private void Update()
    {
        if (waitingForInput && Input.GetKeyDown(KeyCode.Return))
        {
            Debug.Log("Returning to main menu...");
            SceneManager.LoadScene(mainMenu); 
            waitingForInput = false;
        }
    }
}
