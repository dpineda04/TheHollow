using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
public class MonsterAI : MonoBehaviour
{
    [Header("Targeting")]
    public Transform target;                 // assign at spawn (usually player.transform)
    public float detectionRange = 50f;       // max distance before monster stops pursuing
    public float loseSightTimeout = 5f;      // how long to keep chasing after losing target

    [Header("Movement (tweak in Inspector)")]
    public float walkSpeed = 3.5f;
    public float runSpeed = 6.5f;
    public float acceleration = 8f;
    public float angularSpeed = 120f;
    public float stoppingDistance = 1.5f;    // distance to consider "attack range"

    [Header("Behavior")]
    public float updateRate = 0.25f;         // how often to update nav target
    public bool alwaysChase = false;         // ignore detectionRange (debugging)

    // runtime
    private NavMeshAgent agent;
    private float lastUpdate = 0f;
    private float lostTargetTimer = 0f;

    // optional animator
    public Animator animator;
    private static readonly int HashSpeed = Animator.StringToHash("speed");
    private static readonly int HashAttack = Animator.StringToHash("attack");

    void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
        agent.acceleration = acceleration;
        agent.angularSpeed = angularSpeed;
        agent.stoppingDistance = stoppingDistance;
    }

    void Start()
    {
        // set initial speeds
        agent.speed = runSpeed;
    }

    void Update()
    {
        if (target == null) return;

        float dist = Vector3.Distance(transform.position, target.position);

        // detection logic
        if (!alwaysChase && dist > detectionRange)
        {
            lostTargetTimer += Time.deltaTime;
            if (lostTargetTimer > loseSightTimeout)
            {
                // stop moving
                agent.ResetPath();
                if (animator) animator.SetFloat(HashSpeed, 0f);
                return;
            }
        }
        else
        {
            lostTargetTimer = 0f;
        }

        lastUpdate -= Time.deltaTime;
        if (lastUpdate <= 0f)
        {
            // set destination
            if (agent.isOnNavMesh)
                agent.SetDestination(target.position);
            lastUpdate = updateRate;
        }

        // simple speed/animation sync
        float sp = agent.velocity.magnitude;
        if (animator) animator.SetFloat(HashSpeed, sp);

        // attack proximity check (optional)
        if (dist <= agent.stoppingDistance)
        {
            if (animator) animator.SetTrigger(HashAttack);
            // you can call attack logic here (damage player etc.)
        }
    }

    // callable from spawn code
    public void SetTarget(Transform t)
    {
        target = t;
        lostTargetTimer = 0f;
    }

    // helper to change run settings at runtime
    public void SetRunSettings(float speed, float acc)
    {
        runSpeed = speed;
        acceleration = acc;
        agent.speed = runSpeed;
        agent.acceleration = acceleration;
    }
}
