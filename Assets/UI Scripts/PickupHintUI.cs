using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI; // For LayoutRebuilder
using TMPro;

public class PickupHintUI : MonoBehaviour
{
    public static PickupHintUI instance; // easy singleton

    [Tooltip("Assign the TextMeshProUGUI element that will display hints")]
    public TMP_Text hintText; // drag your TextMeshProUGUI object here

    [Tooltip("Assign the background GameObject (Image) that surrounds the text)")]
    public GameObject hintBackground; // drag HintBackground here

    public CanvasGroup canvasGroup;

    // Track which object asked to show the hint (prevents one object hiding another's hint)
    private GameObject currentSource;

    
    public Item item;
  
    private PlayerMovement playerMovement;

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

        // Ensure background starts hidden if assigned
        if (hintBackground != null) hintBackground.SetActive(false);
    }

    // Show a hint. 'source' is the GameObject that requested it (e.g., the pickup)
    public void ShowHint(GameObject source, KeyCode key, string objectName)
    {
        Debug.Log($"playerMovement = {playerMovement}, item = {objectName}, UI instance = {PickupHintUI.instance}, gameObject = {source}");
        if (hintText == null) return;
        currentSource = source;
        hintText.text = $"Press [{key}] to pick up {objectName}";

        // If you're using a CanvasGroup for fade/visibility, enable it
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

        
        // Enable the background image/panel if assigned
        if (hintBackground != null)
        {
            hintText.enabled = true;
            hintBackground.SetActive(true);
            hintText.gameObject.SetActive(true);

            // If the background uses a layout group / ContentSizeFitter, force immediate rebuild so it resizes to the new text
            RectTransform bgRect = hintBackground.GetComponent<RectTransform>();
            if (bgRect != null)
            {
                LayoutRebuilder.ForceRebuildLayoutImmediate(bgRect);
            }

            // If the background has its own CanvasGroup and you want it to follow the main canvasGroup's alpha:
            if (canvasGroup != null)
            {
                CanvasGroup bgCg = hintBackground.GetComponent<CanvasGroup>();
                if (bgCg != null)
                {
                    bgCg.alpha = canvasGroup.alpha;
                    bgCg.interactable = canvasGroup.interactable;
                    bgCg.blocksRaycasts = canvasGroup.blocksRaycasts;
                }
            }
            Debug.Log($"[PickupHintUI] ShowHint: source={source.name}, key={key}, text='{hintText.text}'");
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

        if (hintBackground != null)
        {
            hintBackground.SetActive(false);
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

        if (hintBackground != null)
        {
            hintBackground.SetActive(false);
        }
    }
}

