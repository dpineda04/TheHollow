using System;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;

public class DeathScreenUI : MonoBehaviour
{
    public static DeathScreenUI Instance { get; private set; }
    public UIDocument uiDocument;

    // element names (match these to your uxml names)
    [SerializeField] string deathPanelName = "death-panel";
    [SerializeField] string respawnButtonName = "death-respawn-btn";
    [SerializeField] string mainMenuButtonName = "death-mainmenu-btn";
    [SerializeField] string optionsButtonName = "death-options-btn";
    [SerializeField] string optionsPanelName = "options-panel"; // panel to open/hide

    VisualElement root;
    VisualElement deathPanel;
    VisualElement optionsPanel;
    Button respawnBtn;
    Button mainMenuBtn;
    Button optionsBtn;

    Action onRespawnRequested;

    void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);

        if (uiDocument == null) uiDocument = GetComponent<UIDocument>();
    }

    void Start()
    {
        root = uiDocument.rootVisualElement;

        deathPanel = root.Q(deathPanelName);
        optionsPanel = root.Q(optionsPanelName);

        respawnBtn = root.Q<Button>(respawnButtonName);
        mainMenuBtn = root.Q<Button>(mainMenuButtonName);
        optionsBtn = root.Q<Button>(optionsButtonName);

        // attach handlers
        if (respawnBtn != null) respawnBtn.clicked += OnRespawnClicked;
        if (mainMenuBtn != null) mainMenuBtn.clicked += OnMainMenuClicked;
        if (optionsBtn != null) optionsBtn.clicked += OnOptionsClicked;

        // ensure panels are hidden initially
        if (deathPanel != null) deathPanel.style.display = DisplayStyle.None;
        if (optionsPanel != null) optionsPanel.style.display = DisplayStyle.None;
    }

    // Show death UI and give the script a callback action
    public void Show(Action onRespawn)
    {
        onRespawnRequested = onRespawn;
        if (deathPanel != null) deathPanel.style.display = DisplayStyle.Flex;

        // unlock cursor for interacting with UI
        UnityEngine.Cursor.lockState = CursorLockMode.None;
        UnityEngine.Cursor.visible = true;

        // focus first button
        respawnBtn?.Focus();
    }

    public void Hide()
    {
        if (deathPanel != null) deathPanel.style.display = DisplayStyle.None;
        if (optionsPanel != null) optionsPanel.style.display = DisplayStyle.None;

        onRespawnRequested = null;

        // re-lock cursor (if your gameplay needs it)
        UnityEngine.Cursor.lockState = CursorLockMode.Locked;
        UnityEngine.Cursor.visible = false;
    }

    void OnRespawnClicked()
    {
        onRespawnRequested?.Invoke();
        Hide();
    }

    void OnMainMenuClicked()
    {
          // 1) Unpause the game
        Time.timeScale = 1f;

        // 2) Ensure cursor is visible & free
        UnityEngine.Cursor.visible = true;
        UnityEngine.Cursor.lockState = CursorLockMode.None;

        // 3) Hide any UI here (optional)
        Hide();

        // 4) Load the main menu scene
        SceneManager.LoadScene("MainMenu", LoadSceneMode.Single);
    }
    

    void OnOptionsClicked()
    {
        // toggle the options panel visibility
        if (optionsPanel == null) return;

        bool isVisible = optionsPanel.style.display == DisplayStyle.Flex;
        optionsPanel.style.display = isVisible ? DisplayStyle.None : DisplayStyle.Flex;

        // if you want to populate options controls, do that here
    }
}
