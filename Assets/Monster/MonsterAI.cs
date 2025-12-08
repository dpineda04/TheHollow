using System.Collections;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
[RequireComponent(typeof(Animator))]
public class MonsterAI : MonoBehaviour
{
    [Header("Targeting / Movement")]
    public Transform player;                 // assigned via SetTarget or inspector
    public float chaseSpeed = 3.5f;
    public float wanderSpeed = 1.2f;
    public bool stopOnJumpscare = true;

    [Header("NavMesh Safety")]
    public float sampleDistance = 3f;        // how far to search for a nearby NavMesh
    bool hasLoggedNotOnNavMesh = false;

    [Header("Camera / Effects")]
    public CameraFocusOverride cameraFocus;  // optional, assign main camera's focus script

    [Header("Debug")]
    public bool debugTest = false;           // set true to run the small start test
    public bool animDebug = true;


    NavMeshAgent agent;
    Animator animator;
    bool isJumpscaring = false;

    void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
        animator = GetComponent<Animator>();

        if (agent == null)
        {
            Debug.LogError($"{name}: No NavMeshAgent found. Disabling MonsterAI.");
            enabled = false;
            return;
        }

        if (animator == null)
        {
            Debug.LogError($"{name}: No Animator found. Disabling MonsterAI.");
            enabled = false;
            return;
        }

        // Avoid animation culling when offscreen (helps debugging/spawned off-camera)
        animator.cullingMode = AnimatorCullingMode.AlwaysAnimate;

        // Make sure agent isn't accidentally stopped at start
        agent.isStopped = false;
    }

    // Small debug routine that runs one frame after Awake (can be disabled)
    IEnumerator Start()
    {
        // allow one frame for spawner to finish setup
    yield return null;

    if (!animDebug) yield break;

    Debug.Log($"[AnimDebug] ---- Monster '{name}' Debug Start ----");
    Debug.Log($"[AnimDebug] animator NULL? {animator == null}");
    if (animator != null)
    {
        Debug.Log($"[AnimDebug] runtimeAnimatorController = {(animator.runtimeAnimatorController == null ? "NULL" : animator.runtimeAnimatorController.name)}");
        Debug.Log($"[AnimDebug] cullingMode = {animator.cullingMode}");
    }

    if (agent != null)
    {
        Debug.Log($"[AnimDebug] NavMeshAgent isOnNavMesh = {agent.isOnNavMesh}");
        Debug.Log($"[AnimDebug] agent.speed = {agent.speed}");
        Debug.Log($"[AnimDebug] agent.velocity.magnitude = {agent.velocity.magnitude}");
    }

    // choose an initial speed: prefer velocity, fallback to agent.speed (so blend tree can pick walk/run)
    float initialSpeed = 0f;
    if (agent != null)
    {
        initialSpeed = agent.velocity.magnitude;
        if (initialSpeed <= 0.001f)
        {
            // fallback — use a portion of agent.speed so we don't always stay at 0 on spawn
            initialSpeed = agent.speed * 0.75f;
            Debug.Log($"[AnimDebug] Using fallback initialSpeed={initialSpeed} from agent.speed={agent.speed}");
        }
    }
    else
    {
        initialSpeed = 2.0f; // arbitrary test value if no agent
    }

    if (animator != null && animator.runtimeAnimatorController != null)
    {
        animator.SetFloat("Speed", initialSpeed);
        // Play the Move state directly. If your state has a different name, change "Move" to that exact name.
        animator.Play("Move", 0, 0f);

        // Force an immediate animator update so the visual state is applied this frame (useful in Start)
        animator.Update(0f);

        Debug.Log("[AnimDebug] Forced Play('Move') and set Speed = " + initialSpeed);

        // wait half a second in real time to visually confirm
        yield return new WaitForSecondsRealtime(0.6f);

        Debug.Log("[AnimDebug] Triggering Jumpscare now");
        animator.SetTrigger("Jumpscare");
    }
    else
    {
        Debug.LogError("[AnimDebug] Animator or Controller is missing on monster prefab!");
    }

    Debug.Log($"[AnimDebug] ---- Debug End ----");
    }

    void Update()
    {
        if (isJumpscaring) return;

    if (player != null)
    {
        if (!agent.isOnNavMesh)
        {
            if (!EnsureAgentOnNavMesh()) return;
        }

        agent.isStopped = false;
        agent.speed = chaseSpeed;

        // safe set destination
        agent.SetDestination(player.position);
    }

    // Fallback speed: prefer actual velocity (when moving), otherwise use a portion of agent.speed so animator can enter Move
    float speed = 0f;
    if (agent != null)
    {
        speed = agent.velocity.magnitude;
        if (speed <= 0.001f)
        {
            speed = agent.speed * 0.75f; // tweak multiplier to match thresholds in your Blend Tree
        }
    }

    animator.SetFloat("Speed", speed);
    }

    void OnTriggerEnter(Collider other)
    {
         if (isJumpscaring) return;
        if (other == null) return;

         Debug.Log($"[MonsterAI] OnTriggerEnter with '{other.gameObject.name}' (tag='{other.gameObject.tag}')");

        // Accept either tag or Player-specific component: try tag first
         if (other.CompareTag("Player"))
        {
        // Immediately show death screen (if desired) and play jumpscare
        TryShowDeathScreenImmediate();
        StartCoroutine(HandleJumpscare(other.gameObject));
        }
    }

    // Public API: set the current target for this monster (spawner calls this)
    public void SetTarget(Transform target)
    {
        player = target;

        // ensure agent is on navmesh immediately if possible
        EnsureAgentOnNavMesh();

        if (agent.isOnNavMesh && player != null)
        {
            agent.isStopped = false;
            agent.SetDestination(player.position);
        }
    }

    // Overload accepting GameObject (convenience)
    public void SetTarget(GameObject targetObject)
    {
        SetTarget(targetObject != null ? targetObject.transform : null);
    }

    /// <summary>
    /// Ensures the agent is placed on the NavMesh. Returns true if on NavMesh (after possible warp).
    /// </summary>
    bool EnsureAgentOnNavMesh()
    {
        if (agent.isOnNavMesh) return true;

        NavMeshHit hit;
        if (NavMesh.SamplePosition(transform.position, out hit, sampleDistance, NavMesh.AllAreas))
        {
            bool warped = agent.Warp(hit.position);
            if (!warped)
            {
                if (!hasLoggedNotOnNavMesh)
                {
                    Debug.LogWarning($"{name}: Agent failed to Warp to NavMesh at {hit.position}.");
                    hasLoggedNotOnNavMesh = true;
                }
                return false;
            }

            hasLoggedNotOnNavMesh = false;
            return true;
        }
        else
        {
            if (!hasLoggedNotOnNavMesh)
            {
                Debug.LogWarning($"{name}: No NavMesh found within {sampleDistance}m of position {transform.position}. Consider baking NavMesh or increasing sampleDistance.");
                hasLoggedNotOnNavMesh = true;
            }
            return false;
        }
    }

    IEnumerator HandleJumpscare(GameObject target)
    {
       if (isJumpscaring) yield break;
    isJumpscaring = true;

    // stop agent movement
    if (agent != null) agent.isStopped = true;

    // trigger jumpscare animation
    if (animator != null)
    {
        if (animator.runtimeAnimatorController == null)
        {
            Debug.LogError($"{name}: Animator has no runtime controller assigned!");
        }
        else
        {
            Debug.Log($"{name}: Triggering Jumpscare animation.");
        }

        animator.SetTrigger("Jumpscare");
    }

    // Optional: also ensure death screen is shown at end if you prefer (uncomment)
    // TryShowDeathScreenImmediate();

    // Wait a frame for animation transition, then attempt to read clip length.
    yield return null;

    float waitTime = 1.5f; // fallback
    if (animator != null)
    {
        var clips = animator.GetCurrentAnimatorClipInfo(0);
        if (clips != null && clips.Length > 0)
        {
            waitTime = clips[0].clip.length;
            Debug.Log($"{name}: Jumpscare clip length estimate = {waitTime}s");
        }
    }

    yield return new WaitForSeconds(waitTime + 0.05f);

    // If you want the death screen only after the roar is done, you could call here:
    // if (DeathScreenManager.Instance != null) DeathScreenManager.Instance.ShowDeathScreen();

    // end jumpscare state (do not resume movement if you intended to stop on death)
    isJumpscaring = false;
    }

    // Optional animation event callback to end jumpscare precisely at clip end
     void OnJumpscareEnd()
    {
        isJumpscaring = false;
        agent.isStopped = false;
    }

    // Optional manual trigger from other scripts
    void DoJumpscare()
    {
        if (isJumpscaring) return;
        StartCoroutine(HandleJumpscare(null));
    }

    void TryShowDeathScreenImmediate()
{
    // show death UI immediately (before waiting for jumpscare animation)
    if (DeathScreenManager.Instance != null)
    {
        Debug.Log("[MonsterAI] Requesting DeathScreenManager.ShowDeathScreen()");
        DeathScreenManager.Instance.ShowDeathScreen(() => {
    // This runs when the player presses Respawn
    Debug.Log("Respawn callback triggered!");

    // Example:
    // SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
});
    }
    else
    {
        Debug.LogWarning("[MonsterAI] DeathScreenManager.Instance is null - cannot show death screen.");
    }
}
}


