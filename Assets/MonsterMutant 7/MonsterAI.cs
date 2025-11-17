using UnityEngine;
using UnityEngine;
using UnityEngine.AI;

public class MonsterAI : MonoBehaviour
{
    [Header("References")]
    public NavMeshAgent agent;
    public Transform player;
    public Animator anim;

    [Header("Settings")]
    public float chaseDistance = 20f;
    public float wanderRadius = 25f;
    public float wanderSpeed = 3.5f;
    public float chaseSpeed = 6f;

    private Vector3 wanderPoint;

    void Awake()
    {
        if (!agent) agent = GetComponent<NavMeshAgent>();
        if (!anim) anim = GetComponentInChildren<Animator>();
    }

    void Start()
    {
        PickNewWanderPoint();
        agent.SetDestination(wanderPoint);
    }

    void Update()
    {
        float distance = Vector3.Distance(transform.position, player.position);

        // -----------------------------
        //           CHASE
        // -----------------------------
        if (distance <= chaseDistance)
        {
            agent.speed = chaseSpeed;
            agent.SetDestination(player.position);
        }
        else
        {
            // -----------------------------
            //          WANDER
            // -----------------------------
            agent.speed = wanderSpeed;

            // If we reached our wander point or have no path -> get a new one
            if (!agent.hasPath || agent.remainingDistance < 1f)
            {
                PickNewWanderPoint();
                agent.SetDestination(wanderPoint);
            }
        }

        // Update animation blend tree
        anim.SetFloat("Speed", agent.velocity.magnitude);
    }

    // ---------------------------------------------------
    //    Picks a valid random point on the NavMesh
    // ---------------------------------------------------
    void PickNewWanderPoint()
    {
        Vector3 randomDirection = Random.insideUnitSphere * wanderRadius;
        randomDirection += transform.position;

        NavMeshHit hit;
        NavMesh.SamplePosition(randomDirection, out hit, wanderRadius, NavMesh.AllAreas);

        wanderPoint = hit.position;
    }
}
