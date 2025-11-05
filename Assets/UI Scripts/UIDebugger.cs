using UnityEngine;
using UnityEngine.UIElements;
using UnityEngine.SceneManagement;

public class UIDebugger : MonoBehaviour
{
    void OnEnable()
    {
        Debug.Log("[UIDebugger] OnEnable");
        var doc = GetComponent<UnityEngine.UIElements.UIDocument>();
        Debug.Log("[UIDebugger] UIDocument present? " + (doc != null));

        if (doc == null) return;

        var root = doc.rootVisualElement;
        Debug.Log("[UIDebugger] rootVisualElement present? " + (root != null));

        var play = root.Q<Button>("PlayButton");
        var options = root.Q<Button>("OptionsButton");
        var quit = root.Q<Button>("QuitButton");

        Debug.Log($"[UIDebugger] Play found? { (play != null) }, Options? { (options != null) }, Quit? { (quit != null) }");

        if (play != null)
        {
            // remove any previous anonymous handlers to avoid doubling in edit-mode
            play.clicked -= TestPlay;
            play.clicked += TestPlay;
        }
        if (options != null)
        {
            options.clicked -= TestOptions;
            options.clicked += TestOptions;
        }
        if (quit != null)
        {
            quit.clicked -= TestQuit;
            quit.clicked += TestQuit;
        }
    }

    void TestPlay()
    {
        Debug.Log("[UIDebugger] Play clicked handler ran");
        // Don't actually change scenes — uncomment to test scene load:
        // SceneManager.LoadScene("TheHollow");
    }

    void TestOptions()
    {
        Debug.Log("[UIDebugger] Options clicked handler ran");
    }

    void TestQuit()
    {
        Debug.Log("[UIDebugger] Quit clicked handler ran");
    }
}
