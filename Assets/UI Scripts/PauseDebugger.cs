using UnityEngine;
using UnityEngine.UIElements;

public class PauseDebugger : MonoBehaviour
{
    void Start()
    {
        // Print basic info immediately on start
        var doc = GetComponent<UIDocument>();
        Debug.Log("[PauseDebugger] UIDocument present? " + (doc != null));
        if (doc == null) return;

        var root = doc.rootVisualElement;
        Debug.Log("[PauseDebugger] root != null? " + (root != null));

        // Print top-level children
        string names = "";
        foreach (var c in root.Children())
            names += $"({c.name}:{c.GetType().Name}) ";
        Debug.Log("[PauseDebugger] Top-level children: " + names);

        // Find PausePanel
        var pause = root.Q<VisualElement>("PausePanel");
        Debug.Log("[PauseDebugger] PausePanel found? " + (pause != null));
        if (pause != null)
        {
            Debug.Log($"[PauseDebugger] PausePanel.style.display = {pause.style.display} ; pickingMode = {pause.pickingMode}");
            // Print children of pause panel
            string pnames = "";
            foreach (var c in pause.Children()) pnames += $"({c.name}:{c.GetType().Name}) ";
            Debug.Log("[PauseDebugger] PausePanel children: " + pnames);
        }
    }

    // Helper you can call from the PauseManager or manually to force-show a visible test overlay
    [ContextMenu("ForceShowPauseTest")]
    public void ForceShowPauseTest()
    {
        var doc = GetComponent<UIDocument>();
        if (doc == null) { Debug.Log("[PauseDebugger] No UIDocument"); return; }
        var root = doc.rootVisualElement;
        var pause = root.Q<VisualElement>("PausePanel");
        if (pause == null)
        {
            // If missing, create a simple panel so you can see something
            var newPanel = new VisualElement { name = "PausePanel" };
            newPanel.style.position = Position.Absolute;
            newPanel.style.left = 0; newPanel.style.right = 0; newPanel.style.top = 0; newPanel.style.bottom = 0;
            newPanel.style.backgroundColor = new Color(1f, 0f, 0f, 0.45f); // translucent red
            newPanel.pickingMode = PickingMode.Position;
            root.Add(newPanel);
            Debug.Log("[PauseDebugger] Created temporary PausePanel (red overlay).");
            return;
        }

        // otherwise make it visible and obvious
        pause.style.display = DisplayStyle.Flex;
        pause.style.backgroundColor = new Color(1f, 0f, 0f, 0.45f);
        pause.pickingMode = PickingMode.Position;
        pause.BringToFront();
        Debug.Log("[PauseDebugger] Forced PausePanel visible (red overlay).");
    }
}
