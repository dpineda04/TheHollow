using UnityEngine;

[RequireComponent(typeof(Collider))]
public class EndPointTrigger : MonoBehaviour
{
 
    public string requiredTag = "Player";

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag(requiredTag)) return;

       

        // call the manager
        if (GameManager.Instance != null)
        {
            GameManager.Instance.FinishGame();
        }
        else
        {
            Debug.LogWarning("[EndPointTrigger] No GameManager found in scene.");
        }
    }
}
