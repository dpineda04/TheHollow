using UnityEngine;
using System.Collections;
using UnityEngine.Playables;     // Timeline/PlayableDirector
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    [Header("General")]
    public bool gameFinished = false;
    public float postCutsceneDelay = 1.0f; // wait before showing end UI / loading next scene

    [Header("Timeline cutscene (optional)")]
    public PlayableDirector endingTimeline; // assign a Timeline asset + GameObject with PlayableDirector

    [Header("Image sequence cutscene (optional)")]
    public GameObject imageCutsceneCanvas;  // root Canvas with an Image component
    public Image imageCutsceneImage;        // UI image that will show sprites
    public Sprite[] endingSprites;
    public float imageDisplayTime = 2.0f;
    public float imageFadeTime = 0.5f;

    [Header("End screen / Next scene")]
    public GameObject endScreenUI;          // optional UI to show after cutscene (e.g., "You finished!" panel)
    public string nextSceneName;            // optional: load this scene after everything

    void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    // Called by EndPointTrigger
    public void FinishGame()
    {
        if (gameFinished) return;
        gameFinished = true;

        // stop player input / disable controllers
        DisablePlayerControl();

        // prefer Timeline if assigned, else image sequence, else directly finish
        if (endingTimeline != null)
        {
            StartCoroutine(PlayTimelineThenFinish());
        }
        else if (imageCutsceneCanvas != null && endingSprites != null && endingSprites.Length > 0)
        {
            StartCoroutine(PlayImageSequenceThenFinish());
        }
        else
        {
            StartCoroutine(FinishAfterDelay(postCutsceneDelay));
        }
    }

    void DisablePlayerControl()
    {
        // Try to find player and disable their movement script
        var player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
        {
            var controllers = player.GetComponents<Behaviour>();
            // Commonly you want to disable your specific movement script; if you know the name, disable it directly.
            // This is a safe fallback: attempt to disable a few likely components (replace with your controller type).
            foreach (var c in controllers)
            {
                if (c != null && (c.GetType().Name.Contains("Player") || c.GetType().Name.Contains("Controller") || c.GetType().Name.Contains("Movement")))
                {
                    c.enabled = false;
                }
            }
        }
    }

    IEnumerator PlayTimelineThenFinish()
    {
        endingTimeline.Play();
        // wait until timeline finishes
        while (endingTimeline.state == PlayState.Playing)
            yield return null;

        yield return new WaitForSeconds(postCutsceneDelay);
        DoEndActions();
    }

    IEnumerator PlayImageSequenceThenFinish()
    {
        imageCutsceneCanvas.SetActive(true);
        // ensure the image starts transparent
        yield return StartCoroutine(FadeImageAlpha(0f, 0f));

        for (int i = 0; i < endingSprites.Length; i++)
        {
            imageCutsceneImage.sprite = endingSprites[i];
            // fade in
            yield return StartCoroutine(FadeImageAlpha(1f, imageFadeTime));
            // hold
            float t = 0f;
            while (t < imageDisplayTime)
            {
                t += Time.deltaTime;
                yield return null;
            }
            // fade out
            yield return StartCoroutine(FadeImageAlpha(0f, imageFadeTime));
        }

        yield return new WaitForSeconds(postCutsceneDelay);
        imageCutsceneCanvas.SetActive(false);
        DoEndActions();
    }

    IEnumerator FadeImageAlpha(float target, float duration)
    {
        if (imageCutsceneImage == null) yield break;
        float start = imageCutsceneImage.color.a;
        float t = 0f;
        while (t < duration)
        {
            t += Time.deltaTime;
            float a = Mathf.Lerp(start, target, duration > 0f ? t / duration : 1f);
            var c = imageCutsceneImage.color;
            c.a = a;
            imageCutsceneImage.color = c;
            yield return null;
        }
        var c2 = imageCutsceneImage.color;
        c2.a = target;
        imageCutsceneImage.color = c2;
    }

    IEnumerator FinishAfterDelay(float seconds)
    {
        yield return new WaitForSeconds(seconds);
        DoEndActions();
    }

    void DoEndActions()
    {
        // show final UI
        if (endScreenUI != null) endScreenUI.SetActive(true);

        // unlock cursor if needed
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        // optionally load next scene
        if (!string.IsNullOrEmpty(nextSceneName))
        {
            SceneManager.LoadScene(nextSceneName);
        }
    }
}
