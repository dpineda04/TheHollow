using UnityEngine;
using System.Collections;

public class SimpleRespawnManager : MonoBehaviour
{
    public static SimpleRespawnManager Instance { get; private set; }

    [Tooltip("Fallback spawn point where the player will respawn.")]
    public Transform defaultSpawn;

    [Tooltip("Delay before respawning (seconds)")]
    public float respawnDelay = 0.8f;

    void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    // Public API: call this to respawn the player (player must have PlayerDeathSimple)
    public void RespawnPlayer(PlayerDeathSimple playerDeath)
    {
        if (playerDeath == null) return;
        Vector3 spawnPos = defaultSpawn != null ? defaultSpawn.position : playerDeath.transform.position;
        StartCoroutine(RespawnCoroutine(playerDeath, spawnPos));
    }

    IEnumerator RespawnCoroutine(PlayerDeathSimple pd, Vector3 spawnPos)
    {
        // optional: wait for death animation / UI
        yield return new WaitForSeconds(respawnDelay);

        // Ensure pd still exists
        if (pd == null) yield break;

        // Teleport & revive
        yield return pd.ReviveAt(spawnPos);
    }
}
