using UnityEngine;
using System.Collections;

public class PlayerDeathSimple : MonoBehaviour
{
    [Header("Respawn")]
    public float respawnInvulnerability = 1.5f;

    [Header("Controller")]
    public Behaviour movementController; // your movement script
    public Collider playerCollider;      // optional (auto-assign)
    public Rigidbody playerRigidbody;    // optional (auto-assign)

    bool isDead = false;

    void Awake()
    {
        if (playerCollider == null) playerCollider = GetComponent<Collider>();
        if (playerRigidbody == null) playerRigidbody = GetComponent<Rigidbody>();
    }

    public void Die()
{
    if (isDead) return;
    isDead = true;

    // disable movement so player can't move
    if (movementController != null) movementController.enabled = false;

    if (playerRigidbody != null)
    {
        playerRigidbody.velocity = Vector3.zero;
        playerRigidbody.angularVelocity = Vector3.zero;
        playerRigidbody.isKinematic = true;
    }

    // Show the UI Toolkit death screen if available, otherwise immediately respawn via SimpleRespawnManager
    if (DeathScreenUI.Instance != null)
    {
        // pass an action that triggers the respawn when the player clicks the respawn button
        DeathScreenUI.Instance.Show(() =>
        {
            if (SimpleRespawnManager.Instance != null)
                SimpleRespawnManager.Instance.RespawnPlayer(this);
            else
                Debug.LogWarning("[PlayerDeathSimple] SimpleRespawnManager instance not found when respawn requested.");
        });
    }
    else
    {
        // fallback immediate respawn (no UI available)
        if (SimpleRespawnManager.Instance != null)
        {
            SimpleRespawnManager.Instance.RespawnPlayer(this);
        }
        else
        {
            Debug.LogWarning("[PlayerDeathSimple] DeathScreenUI and SimpleRespawnManager not found. Cannot respawn.");
        }
    }
}

    public IEnumerator ReviveAt(Vector3 spawnPosition)
    {
        // teleport player
        transform.position = spawnPosition;

        // wait a frame + physics step
        yield return null;
        yield return new WaitForFixedUpdate();

        // restore rigidbody if there was one
        if (playerRigidbody != null)
        {
            playerRigidbody.isKinematic = false;
            playerRigidbody.velocity = Vector3.zero;
            playerRigidbody.angularVelocity = Vector3.zero;
        }

        // re-enable movement & collider
        if (playerCollider != null) playerCollider.enabled = true;
        if (movementController != null) movementController.enabled = true;

        // invulnerability window
        float t = 0f;
        while (t < respawnInvulnerability)
        {
            t += Time.deltaTime;
            yield return null;
        }

        isDead = false;
    }
}
