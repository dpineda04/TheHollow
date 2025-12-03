using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections;

public class DeathScreenManager : MonoBehaviour
{
    public static DeathScreenManager Instance { get; private set; }

    [Header("UI")]
    public GameObject deathPanel;
    public Button respawnButton;
    public Button mainMenuButton;
    public Text autoRespawnText; // optional

    [Header("Options")]
    public bool autoRespawnAfterTimeout = false;
    public float autoRespawnSeconds = 5f;

    // internal
    Action onRespawnRequested;

    void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
        if (deathPanel != null) deathPanel.SetActive(false);
    }

    void Start()
    {
        if (respawnButton != null) respawnButton.onClick.AddListener(OnRespawnClicked);
        if (mainMenuButton != null) mainMenuButton.onClick.AddListener(OnMainMenuClicked);
    }

    public void ShowDeathScreen(Action onRespawn)
    {
        onRespawnRequested = onRespawn;
        if (deathPanel != null) deathPanel.SetActive(true);

        // unlock cursor so player can click UI
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;

        if (autoRespawnAfterTimeout)
            StartCoroutine(AutoRespawnCoroutine());
    }

    public void HideDeathScreen()
    {
        if (deathPanel != null) deathPanel.SetActive(false);
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
        StopAllCoroutines();
        if (autoRespawnText != null) autoRespawnText.text = "";
    }

    void OnRespawnClicked()
    {
        onRespawnRequested?.Invoke();
        HideDeathScreen();
    }

    IEnumerator AutoRespawnCoroutine()
    {
        float t = autoRespawnSeconds;
        while (t > 0f)
        {
            if (autoRespawnText != null) autoRespawnText.text = $"Auto respawn in {Mathf.CeilToInt(t)}";
            yield return null;
            t -= Time.deltaTime;
        }
        // trigger respawn
        onRespawnRequested?.Invoke();
        HideDeathScreen();
    }

    void OnMainMenuClicked()
    {
        // load main menu (change name as needed)
        UnityEngine.SceneManagement.SceneManager.LoadScene("MainMenu");
    }
}
