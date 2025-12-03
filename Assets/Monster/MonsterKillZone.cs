using UnityEngine;

public class MonsterKillZone : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player")) return;

        var pd = other.GetComponent<PlayerDeathSimple>();
        if (pd != null)
        {
            pd.Die();
        }
        else
        {
            Debug.LogWarning("PlayerDeathSimple not found on Player object.");
        }
    }
}

