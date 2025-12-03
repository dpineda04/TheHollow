using UnityEngine;

[RequireComponent(typeof(Collider))]
public class ExitTrigger : MonoBehaviour
{
    public static ExitTrigger Instance; // optional for easy access

    public bool isUnlocked = false;     // EXIT IS LOCKED AT START
    public string requiredTag = "Player";

    private void Awake()
    {
        Instance = this;
        // Make sure the collider is a trigger
        GetComponent<Collider>().isTrigger = true;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag(requiredTag)) return;

        if (!isUnlocked)
        {
            Debug.Log("Exit locked! Monster not spawned or items not collected yet.");
            return;
        }

        Debug.Log("Exit UNLOCKED — finishing game!");
        GameManager.Instance.FinishGame();  // call your ending
    }

    // Call this when monster spawns / items completed
    public void UnlockExit()
    {
        Debug.Log("Exit unlocked!");
        isUnlocked = true;
    }
}
