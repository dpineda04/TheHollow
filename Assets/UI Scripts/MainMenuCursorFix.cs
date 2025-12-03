using UnityEngine;
using System.Collections;

public class MainMenuCursorFix : MonoBehaviour
{
    IEnumerator Start()
    {
        // Force visible immediately (cover race conditions)
        UnityEngine.Cursor.visible = true;
        UnityEngine.Cursor.lockState = CursorLockMode.None;

        // Also force again for the next few frames in case something else toggles it
        for (int i = 0; i < 3; i++)
        {
            yield return null;
            UnityEngine.Cursor.visible = true;
            UnityEngine.Cursor.lockState = CursorLockMode.None;
        }
    }
}