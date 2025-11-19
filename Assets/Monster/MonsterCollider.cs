using UnityEngine;

public class MonsterCollider : MonoBehaviour
{
    [Tooltip("Tag that identifies the player")]
    public string playerTag = "Player";

    [Tooltip("Optional: how close to cause death (works if using distance checks)")]
    public float contactDelay = 0f; // can be used for attack windup

    private bool triggered = false;

    private void OnTriggerEnter(Collider other)
    {
        if (triggered) return;
        if (!other.CompareTag(playerTag)) return;

        triggered = true;
        // call game over / death UI
        GameOverManager.Instance?.ShowGameOver();
        // optionally stop monster or play attack anim
        var ai = GetComponentInParent<MonsterAI>();
        if (ai != null)
        {
            // stop chasing
            GetComponentInParent<UnityEngine.AI.NavMeshAgent>()?.ResetPath();
        }
    }
}

