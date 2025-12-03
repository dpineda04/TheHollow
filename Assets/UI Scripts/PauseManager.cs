using System;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;

public class PauseManager : MonoBehaviour
{
    public static bool IsPaused { get; private set; } = false;

    [Tooltip("Name of the main menu scene used by Quit")]
    public string mainMenuSceneName = "MainMenu";

    [Header("UXML element names (case-sensitive)")]
    public string pausePanelName = "PausePanel";       // set this to the Name you used in UI Builder
    public string pauseOverlayName = "pause-overlay";
    public string pauseContainerName = "pause-container";
    public string optionsPanelName = "options-panel";

    public string resumeButtonName = "ResumeButton";
    public string optionsButtonName = "OptionsButton";
    public string mainMenuButtonName = "MainMenuButton";
    public string quitButtonName = "QuitButton";

    private UIDocument uiDoc;
    private VisualElement root;
    private VisualElement pausePanel;
    private VisualElement pauseOverlay;
    private VisualElement pauseContainer;
    private VisualElement optionsPanel;
    private Button resumeButton, optionsButton, mainMenuButton, quitButton;

    void Awake()
    {
        // Try to get UIDocument on same GameObject, but allow inspector assignment later
        if (uiDoc == null)
            uiDoc = GetComponent<UIDocument>();
    }

    void Start()
    {
        // Try again if not set
        if (uiDoc == null)
        {
            uiDoc = GetComponent<UIDocument>();
            if (uiDoc == null)
            {
                // As a last resort, find any UIDocument in the scene
                uiDoc = FindObjectOfType<UIDocument>();
                if (uiDoc == null)
                {
                    Debug.LogError("[PauseManager] No UIDocument found. Attach a UIDocument with your UXML to the scene.");
                    enabled = false;
                    return;
                }
                else
                {
                    Debug.Log("[PauseManager] Auto-found UIDocument on: " + uiDoc.gameObject.name);
                }
            }
        }

        // Get the root visual element
        root = uiDoc.rootVisualElement;
        if (root == null)
        {
            Debug.LogError("[PauseManager] uiDocument.rootVisualElement is null. Is the UXML assigned to the UIDocument?");
            enabled = false;
            return;
        }

        // Query elements (case-sensitive names)
        pausePanel = root.Q<VisualElement>(pausePanelName);
        pauseOverlay = root.Q<VisualElement>(pauseOverlayName);
        pauseContainer = root.Q<VisualElement>(pauseContainerName);
        optionsPanel = root.Q<VisualElement>(optionsPanelName);

        resumeButton = root.Q<Button>(resumeButtonName);
        optionsButton = root.Q<Button>(optionsButtonName);
        mainMenuButton = root.Q<Button>(mainMenuButtonName);
        quitButton = root.Q<Button>(quitButtonName);

        Debug.Log($"[PauseManager] Query results -> pausePanel={(pausePanel!=null)}, optionsPanel={(optionsPanel!=null)}, resume={(resumeButton!=null)}, optionsBtn={(optionsButton!=null)}, mainMenuBtn={(mainMenuButton!=null)}, quitBtn={(quitButton!=null)}");

        // subscribe safely
        if (resumeButton != null) resumeButton.clicked -= OnResumeClicked;
        if (optionsButton != null) optionsButton.clicked -= OnOptionsClicked;
        if (mainMenuButton != null) mainMenuButton.clicked -= OnMainMenuClicked;
        if (quitButton != null) quitButton.clicked -= OnQuitClicked;

        if (resumeButton != null) resumeButton.clicked += OnResumeClicked;
        if (optionsButton != null) optionsButton.clicked += OnOptionsClicked;
        if (mainMenuButton != null) mainMenuButton.clicked += OnMainMenuClicked;
        if (quitButton != null) quitButton.clicked += OnQuitClicked;

        // hide panels initially (defensive)
        if (pausePanel != null)
        {
            pausePanel.style.display = DisplayStyle.None;
            pausePanel.pickingMode = PickingMode.Ignore;
        }
        if (optionsPanel != null)
        {
            optionsPanel.style.display = DisplayStyle.None;
            optionsPanel.pickingMode = PickingMode.Ignore;
        }

        // Ensure gameplay is running (in case Play started paused)
        Time.timeScale = 1f;
        IsPaused = false;
        UnityEngine.Cursor.visible = false;
        UnityEngine.Cursor.lockState = CursorLockMode.Locked;
    }

    void OnDisable()
    {
        if (resumeButton != null) resumeButton.clicked -= OnResumeClicked;
        if (optionsButton != null) optionsButton.clicked -= OnOptionsClicked;
        if (mainMenuButton != null) mainMenuButton.clicked -= OnMainMenuClicked;
        if (quitButton != null) quitButton.clicked -= OnQuitClicked;
    }

    void Update()
    {
        // Toggle with Escape (commonly used) or Q if you prefer
        if (Input.GetKeyDown(KeyCode.Escape) || Input.GetKeyDown(KeyCode.Q))
        {
            TogglePause();
        }
    }

    public void TogglePause()
    {
        if (IsPaused) ExitPause();
        else EnterPause();
    }

    private void EnterPause()
    {
        Debug.Log("[PauseManager] EnterPause()");
        if (pausePanel == null)
        {
            Debug.LogWarning("[PauseManager] pausePanel not found. Check pausePanelName matches the UXML element name.");
            return;
        }

        // show panel and capture input
        pausePanel.style.display = DisplayStyle.Flex;
        pausePanel.pickingMode = PickingMode.Position;

        if (pauseOverlay != null) pauseOverlay.pickingMode = PickingMode.Position;

        if (pauseContainer != null)
        {
            pauseContainer.pickingMode = PickingMode.Position;
            pauseContainer.BringToFront();
        }
        else
        {
            pausePanel.BringToFront();
        }

        // hide options panel when entering pause
        if (optionsPanel != null)
        {
            optionsPanel.style.display = DisplayStyle.None;
            optionsPanel.pickingMode = PickingMode.Ignore;
        }

        // pause time
        Time.timeScale = 0f;

        // show cursor
        UnityEngine.Cursor.visible = true;
        UnityEngine.Cursor.lockState = CursorLockMode.None;

        // focus resume button if exists
        resumeButton?.Focus();

        IsPaused = true;
    }

    private void ExitPause()
    {
        Debug.Log("[PauseManager] ExitPause()");
        if (pausePanel != null)
        {
            pausePanel.style.display = DisplayStyle.None;
            pausePanel.pickingMode = PickingMode.Ignore;
        }
        if (optionsPanel != null)
        {
            optionsPanel.style.display = DisplayStyle.None;
            optionsPanel.pickingMode = PickingMode.Ignore;
        }

        Time.timeScale = 1f;

        // hide & lock cursor for gameplay
        UnityEngine.Cursor.visible = false;
        UnityEngine.Cursor.lockState = CursorLockMode.Locked;

        IsPaused = false;
    }

    // ---------- UI Button handlers ----------
    private void OnResumeClicked()
    {
        ExitPause();
    }

    private void OnOptionsClicked()
    {
        if (optionsPanel == null)
        {
            Debug.LogWarning("[PauseManager] Options button clicked but optionsPanel not found.");
            return;
        }

        bool visible = optionsPanel.style.display == DisplayStyle.Flex;
        optionsPanel.style.display = visible ? DisplayStyle.None : DisplayStyle.Flex;
        optionsPanel.pickingMode = visible ? PickingMode.Ignore : PickingMode.Position;
    }

    private void OnMainMenuClicked()
    {
        Debug.Log("[PauseManager] MainMenu clicked. Cleaning up and loading MainMenu.");
        // Ensure game resumed and input reset before leaving
        IsPaused = false;
        Time.timeScale = 1f;
        UnityEngine.Cursor.visible = true;
        UnityEngine.Cursor.lockState = CursorLockMode.None;

        if (pausePanel != null) { pausePanel.style.display = DisplayStyle.None; pausePanel.pickingMode = PickingMode.Ignore; }
        if (optionsPanel != null) { optionsPanel.style.display = DisplayStyle.None; optionsPanel.pickingMode = PickingMode.Ignore; }

        SceneManager.LoadScene(mainMenuSceneName, LoadSceneMode.Single);
    }

    private void OnQuitClicked()
    {
        Debug.Log("[PauseManager] MainMenu clicked. Cleaning up and loading MainMenu.");
        // Ensure game resumed and input reset before leaving
        IsPaused = false;
        Time.timeScale = 1f;
        UnityEngine.Cursor.visible = true;
        UnityEngine.Cursor.lockState = CursorLockMode.None;

        if (pausePanel != null) { pausePanel.style.display = DisplayStyle.None; pausePanel.pickingMode = PickingMode.Ignore; }
        if (optionsPanel != null) { optionsPanel.style.display = DisplayStyle.None; optionsPanel.pickingMode = PickingMode.Ignore; }

        SceneManager.LoadScene(mainMenuSceneName, LoadSceneMode.Single);
    }
}
