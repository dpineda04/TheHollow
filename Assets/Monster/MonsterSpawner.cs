using System.Collections;
using UnityEngine;

public class MonsterSpawner : MonoBehaviour
{
    public GameObject monsterPrefab;
    public Transform spawnPoint;            // where monster appears
    public float spawnDelay = 1.0f;         // seconds after inventory full
    
    public bool setPlayerAsTarget = true;

    void OnEnable()
    {
        if (Inventory.instance != null)
            Inventory.instance.OnInventoryFull += SpawnNow;
    }

    void OnDisable()
    {
        if (Inventory.instance != null)
            Inventory.instance.OnInventoryFull -= SpawnNow;
    }
    public void SpawnNow()
    {
        StartCoroutine(SpawnAfterDelay());
    }



    private IEnumerator SpawnAfterDelay()
    {
        yield return new WaitForSecondsRealtime(spawnDelay);
        if (monsterPrefab == null || spawnPoint == null) yield break;

        var m = Instantiate(monsterPrefab, spawnPoint.position, spawnPoint.rotation);
        // give it time to initialize
        yield return null;

        var ai = m.GetComponent<MonsterAI>();
        if (ai != null && setPlayerAsTarget)
        {
            var player = GameObject.FindGameObjectWithTag("Player");
            if (player != null) ai.SetTarget(player.transform);
        }
    }
}
