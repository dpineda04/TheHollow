using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class FadeController : MonoBehaviour
{
      public static FadeController Instance;

    [Header("References")]
    [Tooltip("Assign the full-screen black Image here (child of this GameObject).")]
    public Image fadeImage;

    [Header("Settings")]
    public float fadeDuration = 1f;

    private void Awake()
    {
        // Singleton
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);

            // If the image exists, ensure transparent start
            if (fadeImage != null)
                SetAlpha(0f);

            SceneManager.sceneLoaded += OnSceneLoaded;
        }
        else
        {
            // If a duplicate exists (e.g., in another scene), destroy it
            Destroy(gameObject);
        }
    }

    private void OnDestroy()
    {
        // cleanup
        if (Instance == this)
            SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    // If a scene is loaded and for some reason the fadeImage reference is null
    // (e.g., you accidentally didn't parent it), try to find it by name.
    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (fadeImage == null)
        {
            // Try to find a GameObject named "FadeImage" in the loaded scene (optional)
            var go = GameObject.Find("FadeImage");
            if (go != null)
            {
                var img = go.GetComponent<Image>();
                if (img != null)
                {
                    fadeImage = img;
                    SetAlpha(0f); // make sure it's transparent on start
                }
            }
        }
    }

    // Public helper to start fade+scene load
    public void FadeToScene(string sceneName)
    {
        if (fadeImage == null)
        {
            Debug.LogWarning("[FadeController] fadeImage is not assigned.");
            SceneManager.LoadScene(sceneName); // fallback
            return;
        }

        StartCoroutine(FadeOutAndLoadScene(sceneName));
    }

    private IEnumerator FadeOutAndLoadScene(string sceneName)
    {
        // Fade fully to black
        yield return StartCoroutine(Fade(0f, 1f));

        // OPTIONAL: Pause the game (stops movement, enemies, etc.)
        Time.timeScale = 0f;

    // WAIT here (even when paused, WaitForSecondsRealtime still works)
        yield return new WaitForSecondsRealtime(2f);

        // Load the new scene (screen is fully black here)
        SceneManager.LoadScene(sceneName);
            
        Time.timeScale = 1f;
        // Wait one frame so new scene UI/lighting initializes
        yield return null;

        // Fade from black into the new scene
        yield return StartCoroutine(Fade(1f, 0f));
    }

    private IEnumerator Fade(float startAlpha, float endAlpha)
    {
        if (fadeImage == null)
            yield break;

        float t = 0f;
        Color c = fadeImage.color;
        while (t < fadeDuration)
        {
            float a = Mathf.Lerp(startAlpha, endAlpha, t / fadeDuration);
            c.a = a;
            fadeImage.color = c;
            t += Time.deltaTime;
            yield return null;
        }
        c.a = endAlpha;
        fadeImage.color = c;
    }

    // Immediate alpha set
    private void SetAlpha(float a)
    {
        if (fadeImage == null) return;
        Color c = fadeImage.color;
        c.a = a;
        fadeImage.color = c;
    }
}
