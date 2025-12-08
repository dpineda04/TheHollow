using System.Collections;
using UnityEngine;
using UnityEngine.AI;

public class MonsterSpawner : MonoBehaviour
{
    public GameObject monsterPrefab;
    public Transform spawnPoint;            // where monster appears
    public float spawnDelay = 1.0f;         // seconds after inventory full

    public bool setPlayerAsTarget = true;

    // how far to search for a nearby NavMesh if the spawn position isn't on one
    public float sampleDistance = 3f;

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

    public IEnumerator SpawnAfterDelay()
    {
        yield return new WaitForSecondsRealtime(spawnDelay);
        if (monsterPrefab == null || spawnPoint == null) yield break;

        var m = Instantiate(monsterPrefab, spawnPoint.position, spawnPoint.rotation);
        // give it a frame to initialize (components awake/start)
        yield return null;

        // try to ensure the agent is placed onto the NavMesh (so SetDestination won't error)
        var agent = m.GetComponent<NavMeshAgent>();
        if (agent != null)
        {
            // if the agent isn't on the navmesh at spawn, try to sample the nearest navmesh point and warp to it
            if (!agent.isOnNavMesh)
            {
                NavMeshHit hit;
                if (NavMesh.SamplePosition(m.transform.position, out hit, sampleDistance, NavMesh.AllAreas))
                {
                    bool warped = agent.Warp(hit.position);
                    if (!warped)
                    {
                        Debug.LogWarning($"[MonsterSpawner] Failed to Warp agent of {m.name} to NavMesh at {hit.position}.");
                    }
                    else
                    {
                        // optionally align rotation to the nav hit normal or leave as-is
                        m.transform.rotation = spawnPoint.rotation;
                    }
                }
                else
                {
                    Debug.LogWarning($"[MonsterSpawner] No NavMesh found within {sampleDistance}m of spawn point for {m.name}. Consider baking the NavMesh or increasing sampleDistance.");
                }

                // allow a frame for the agent to settle if needed
                yield return null;
            }
        }

        var ai = m.GetComponent<MonsterAI>();
        if (ai != null && setPlayerAsTarget)
        {
            var player = GameObject.FindGameObjectWithTag("Player");
            if (player != null)
            {
                // call the public SetTarget; MonsterAI should handle additional safety checks
                ai.SetTarget(player.transform);
            }
            else
            {
                Debug.LogWarning("[MonsterSpawner] Player with tag 'Player' not found. Monster won't have a target.");
            }
        }

        // ---- UNLOCK THE EXIT ----
        // If you implemented ExitTrigger.Instance, this will unlock the house exit.
        if (ExitTrigger.Instance != null)
        {
            ExitTrigger.Instance.UnlockExit();
            Debug.Log("[MonsterSpawner] Exit unlocked because monster spawned.");
        }
        else
        {
            Debug.LogWarning("[MonsterSpawner] ExitTrigger.Instance is null. Ensure ExitTrigger exists.");
        }
    }
}
