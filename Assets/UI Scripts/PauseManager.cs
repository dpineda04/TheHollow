using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;

public class PauseManager : MonoBehaviour
{
    public static bool IsPaused { get; private set; } = false;
    [Tooltip("Name of the main menu scene used by Quit")]
    public string mainMenuSceneName = "MainMenu";

    private UIDocument uiDoc;
    private VisualElement pausePanel;
    private VisualElement pauseOverlay;
    private VisualElement pauseContainer;
    private Button resumeButton, optionsButton, quitButton;

    void OnEnable()
    {
        uiDoc = GetComponent<UIDocument>();
        if (uiDoc == null) { Debug.LogError("PauseManager requires a UIDocument on the same GameObject."); enabled = false; return; }

        var root = uiDoc.rootVisualElement;

        // Cache elements
        pausePanel = root.Q<VisualElement>("PausePanel");
        if (pausePanel == null) Debug.LogWarning("PausePanel not found in UXML (name must match).");

        // Assuming you've named these classes/nodes in your UXML:
        pauseOverlay   = pausePanel?.Q<VisualElement>("pause-overlay");
        pauseContainer = pausePanel?.Q<VisualElement>("pause-container"); // the container which holds buttons

        resumeButton  = root.Q<Button>("ResumeButton");
        optionsButton = root.Q<Button>("OptionsButton");
        quitButton    = root.Q<Button>("QuitButton");

        // subscribe safely
        if (resumeButton != null) { resumeButton.clicked -= OnResumeClicked; resumeButton.clicked += OnResumeClicked; }
        if (optionsButton != null) { optionsButton.clicked -= OnOptionsClicked; optionsButton.clicked += OnOptionsClicked; }
        if (quitButton != null) { quitButton.clicked -= OnQuitClicked; quitButton.clicked += OnQuitClicked; }

        // Ensure hidden at start
        if (pausePanel != null)
        {
            pausePanel.style.display = DisplayStyle.None;
            // Make sure it doesn't accidentally capture input before shown
            pausePanel.pickingMode = PickingMode.Ignore;
        }

        // Ensure gameplay is running
        Time.timeScale = 1f;
        IsPaused = false;
        UnityEngine.Cursor.visible = false;
        UnityEngine.Cursor.lockState = CursorLockMode.Locked;
    }

    void OnDisable()
    {
        if (resumeButton != null) resumeButton.clicked -= OnResumeClicked;
        if (optionsButton != null) optionsButton.clicked -= OnOptionsClicked;
        if (quitButton != null) quitButton.clicked -= OnQuitClicked;
    }

    void Update()
    {
        // Toggle only on key press Q (or add Escape)
        if (Input.GetKeyDown(KeyCode.Q))
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
            Debug.LogWarning("PausePanel missing; cannot open pause menu.");
            return;
        }

        // show panel
        pausePanel.style.display = DisplayStyle.Flex;

        // Ensure it captures pointer events to block game clicks
        pausePanel.pickingMode = PickingMode.Position;

        // overlay should capture clicks but be behind container; container must be on top
        if (pauseOverlay != null) pauseOverlay.pickingMode = PickingMode.Position;

        // Bring container to front (so buttons are clickable and visible above overlay)
        if (pauseContainer != null)
        {
            pauseContainer.pickingMode = PickingMode.Position;
            pauseContainer.BringToFront();
        }
        else
        {
            // If no container, bring the whole panel to front
            pausePanel.BringToFront();
        }

        // pause gameplay
        Time.timeScale = 0f;

        // show cursor
        UnityEngine.Cursor.visible = true;
        UnityEngine.Cursor.lockState = CursorLockMode.None;

        // focus resume button for keyboard/gamepad
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

        // resume gameplay
        Time.timeScale = 1f;

        // hide and lock cursor again (adjust for your game)
        UnityEngine.Cursor.visible = false;
        UnityEngine.Cursor.lockState = CursorLockMode.Locked;

        IsPaused = false;
    }

    private void OnResumeClicked() => TogglePause();
    private void OnOptionsClicked() => Debug.Log("[PauseManager] Options clicked");
    private void OnQuitClicked()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(mainMenuSceneName);
    }
}