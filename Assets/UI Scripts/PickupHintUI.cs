using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class PickupHintUI : MonoBehaviour
{
    public static PickupHintUI instance; // easy singleton

    [Tooltip("Assign the TextMeshProUGUI element that will display hints")]
    public TMP_Text hintText; // drag your TextMeshProUGUI object here, 
    // hint text style should be changed 

    public CanvasGroup canvasGroup;

    // Track which object asked to show the hint (prevents one object hiding another's hint)
    private GameObject currentSource;

    private void Awake()
    {
       if (instance != null && instance != this)
        {
            Destroy(gameObject);
            return;
        }
        instance = this;

        if (hintText != null) hintText.text = "";
        if (canvasGroup != null) canvasGroup.alpha = 0f;
    }


    // Show a hint. 'source' is the GameObject that requested it (e.g., the pickup)
    //key and object name will be given though itemProxomityPickup, placed on each object

    public void ShowHint(GameObject source, KeyCode key, string objectName)
    {
        if (hintText == null) return;
        currentSource = source;
        hintText.text = $"Press [{key}] to pick up {objectName}";
        if (canvasGroup != null)
        {
            canvasGroup.alpha = 1f; // lightweight show (replace with tweens for smoothness)
            canvasGroup.interactable = true;
            canvasGroup.blocksRaycasts = true;
        }
        else
        {
            hintText.enabled = true;
        }
    }

     public void HideHint(GameObject source)
    {
        if (source != currentSource) return;
        currentSource = null;
        if (canvasGroup != null)
        {
            canvasGroup.alpha = 0f;
            canvasGroup.interactable = false;
            canvasGroup.blocksRaycasts = false;
        }
        else if (hintText != null)
        {
            hintText.text = "";
            hintText.enabled = false;
        }
    }

    // Force hide no matter who requested it
    public void ForceHide()
    {
        currentSource = null;
        if (canvasGroup != null)
        {
            canvasGroup.alpha = 0f;
            canvasGroup.interactable = false;
            canvasGroup.blocksRaycasts = false;
        }
        else if (hintText != null)
        {
            hintText.text = "";
            hintText.enabled = false;
        }
    }
}
