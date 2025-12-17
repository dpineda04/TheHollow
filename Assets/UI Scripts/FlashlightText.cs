using UnityEngine;
using TMPro;

public class FlashlightText : MonoBehaviour
{
    [Header("UI Reference")]
    public TextMeshProUGUI promptText;
    public GameObject hintBackground;

    [Header("Text To Display")]
    [TextArea]
    public string message = "Press C to pick up the flashlight";

    private void Start()
    {
        if (promptText != null)
            promptText.gameObject.SetActive(false); // Hide initially
        if (hintBackground != null) hintBackground.SetActive(false);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            if (promptText != null)
            {
                promptText.text = message;   // Set exact styled text
                promptText.gameObject.SetActive(true);
                if (hintBackground != null) hintBackground.SetActive(true);
                
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            if (promptText != null)
                promptText.gameObject.SetActive(false);
                hintBackground.SetActive(false);
        }
    }
}
