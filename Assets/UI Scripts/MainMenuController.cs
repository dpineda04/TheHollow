using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEngine.SceneManagement;

public class MainMenuController : MonoBehaviour
{
    [Tooltip("Load Main Game")]
    public string gameSceneName = "TheHollow";

    private UIDocument uiDoc; //create this doc


    void OnEnable()
    {
        uiDoc = GetComponent<UIDocument>();
        if (uiDoc == null)
        {
            Debug.LogError("Missing UIDocument component!");
            return;
        }

        var root = uiDoc.rootVisualElement;

        var playButton = root.Q<Button>("PlayButton");
        var optionsButton = root.Q<Button>("OptionsButton");
        var quitButton = root.Q<Button>("QuitButton");

        playButton.clicked += () => SceneManager.LoadScene("TheHollow");
        optionsButton.clicked += () => Debug.Log("Options pressed");
        quitButton.clicked += Application.Quit;
    }

    void OnDisable()
    {
        if (uiDoc == null) return;
        var root = uiDoc.rootVisualElement;

        var playButton = root.Q<Button>("PlayButton");
        var optionsButton = root.Q<Button>("OptionsButton");
        var quitButton = root.Q<Button>("QuitButton");

        if (playButton != null) playButton.clicked -= OnPlayClicked;
        if (optionsButton != null) optionsButton.clicked -= OnOptionsClicked;
        if (quitButton != null) quitButton.clicked -= OnQuitClicked;

     void OnPlayClicked()
    {
        // Simple: synchronous load
        SceneManager.LoadScene("TheHollow");

        // Or use async with a loading screen:
        // StartCoroutine(LoadAsync(gameSceneName));
    }

    void OnOptionsClicked()
    {
        Debug.Log("Open options UI");
        // If you have an options panel inside the same UXML:
        // optionsPanel = uiDoc.rootVisualElement.Q<VisualElement>("OptionsPanel");
        // optionsPanel.style.display = DisplayStyle.Flex;

        // Or load a separate scene / overlay
    }

    void OnQuitClicked()
    {
        Application.Quit();
    }

    // Example coroutine for async load (if you want a loading screen)
    // private IEnumerator LoadAsync(string sceneName)
    // {
    //     var op = SceneManager.LoadSceneAsync(sceneName);
    //     op.allowSceneActivation = true;
    //     while (!op.isDone)
    //     {
    //         // update loading UI here using op.progress
    //         yield return null;
    //     }
    // }



   
}

}
