using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEngine.SceneManagement;

public class MainMenuController : MonoBehaviour
{
   private UIDocument uiDoc;

    // cache these so OnDisable can safely unsubscribe
    private Button playButton;
    private Button optionsButton;
    private Button quitButton;

    void OnEnable()
    {
        uiDoc = GetComponent<UIDocument>();
        if (uiDoc == null)
        {
            Debug.LogWarning("MainMenuController: missing UIDocument on GameObject");
            return;
        }

        var root = uiDoc.rootVisualElement;
        if (root == null)
        {
            Debug.LogWarning("MainMenuController: rootVisualElement is null");
            return;
        }

        // Cache references once
        playButton = root.Q<Button>("PlayButton");
        optionsButton = root.Q<Button>("OptionsButton");
        quitButton = root.Q<Button>("QuitButton");

        // Subscribe if they exist
        if (playButton != null) playButton.clicked += OnPlayClicked;
        if (optionsButton != null) optionsButton.clicked += OnOptionsClicked;
        if (quitButton != null) quitButton.clicked += OnQuitClicked;
    }

    void OnDisable()
    {
        // Unsubscribe from cached references only (safe even if root has been destroyed)
        if (playButton != null) playButton.clicked -= OnPlayClicked;
        if (optionsButton != null) optionsButton.clicked -= OnOptionsClicked;
        if (quitButton != null) quitButton.clicked -= OnQuitClicked;

        // Null out caches (optional but neat)
        playButton = null;
        optionsButton = null;
        quitButton = null;
        uiDoc = null;
    }

    private void OnPlayClicked()
    {
        SceneManager.LoadScene("TheHollow");
    }

    private void OnOptionsClicked()
    {
        Debug.Log("Options clicked");
    }

    private void OnQuitClicked()
    {
        Application.Quit();
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#endif
    }


   
}

