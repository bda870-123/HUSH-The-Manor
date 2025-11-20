using UnityEngine;
using UnityEngine.AI;

public class MonsterAI : MonoBehaviour
{
    [Header("References")]
    public NavMeshAgent agent;
    public Transform player;
    public AudioSource proximitySound;

    [Header("Behavior Settings")]
    public float chaseDistance = 20f;   // Start chasing when this close
    public float wanderRadius = 25f;   // Random roam radius
    public float wanderInterval = 5f;   // Seconds between picking new roam points

    [Header("Movement Speeds")]
    public float wanderSpeed = 3.5f;
    public float chaseSpeed = 6f;

    [Header("Audio Fade Settings")]
    public float fadeSpeed = 2f;        // Speed of fade in/out

    // Internal variables
    float timer;
    float targetVolume = 0f;
    Animator anim;

    void Awake()
    {
        if (!agent) agent = GetComponent<NavMeshAgent>();
        // Finds Animator even if it's on a child object
        anim = GetComponentInChildren<Animator>();
    }

    void Start()
    {
        timer = wanderInterval;

        // Prep audio
        if (proximitySound)
        {
            proximitySound.loop = true;
            if (!proximitySound.isPlaying) proximitySound.Play();
            proximitySound.volume = 0f; // start silent
        }

        // Ensure NavMeshAgent is active
        if (agent) agent.isStopped = false;
    }

    void Update()
    {
        if (!agent || !player) return;

        float distance = Vector3.Distance(player.position, transform.position);

        if (distance <= chaseDistance)
        {
            // CHASE
            agent.speed = chaseSpeed;
            agent.SetDestination(player.position);
            targetVolume = 1f; // full volume when close
        }
        else
        {
            // WANDER
            agent.speed = wanderSpeed;
            timer += Time.deltaTime;

            if (timer >= wanderInterval || agent.remainingDistance <= agent.stoppingDistance)
            {
                Vector3 newPos = RandomNavPoint(transform.position, wanderRadius);
                agent.SetDestination(newPos);
                timer = 0f;
            }

            targetVolume = 0f; // fade out when far
        }

        // Smooth audio fade
        if (proximitySound)
        {
            proximitySound.volume = Mathf.MoveTowards(
                proximitySound.volume,
                targetVolume,
                fadeSpeed * Time.deltaTime
            );
        }

        // Update animator if found
        if (anim)
        {
            float speedValue = agent.velocity.magnitude;
            anim.SetFloat("Speed", speedValue);
            anim.speed = speedValue > 0.1f ? 1f : 0.9f;
        }
    }

    // Picks a random reachable point on NavMesh near origin
    static Vector3 RandomNavPoint(Vector3 origin, float radius)
    {
        Vector3 rand = Random.insideUnitSphere * radius + origin;
        NavMeshHit hit;
        if (NavMesh.SamplePosition(rand, out hit, radius, NavMesh.AllAreas))
            return hit.position;
        return origin;
    }

#if UNITY_EDITOR
    // Draw chase and wander ranges in Scene view
    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, chaseDistance);
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(transform.position, wanderRadius);
    }
#endif
}
