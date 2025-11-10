using UnityEngine;
using UnityEngine.AI;

public class MonsterAI : MonoBehaviour
{
    public NavMeshAgent agent;
    public Transform player;
    public float chaseDistance = 10f;
    public float wanderRadius = 15f;
    public float wanderTimer = 5f;
    public AudioSource proximitySound;

    private float timer;
    private bool isChasing = false;
    private float targetVolume = 0f;

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        timer = wanderTimer;

        // Start sound silent and looped
        if (proximitySound != null)
        {
            proximitySound.volume = 0f;
            proximitySound.loop = true;
            proximitySound.Play();
        }
    }

    void Update()
    {
        float distance = Vector3.Distance(player.position, transform.position);

        // If player is within chase range
        if (distance < chaseDistance)
        {
            isChasing = true;
            agent.SetDestination(player.position);
            targetVolume = 1f; // full volume when close
        }
        else
        {
            // Wander randomly
            isChasing = false;
            timer += Time.deltaTime;

            if (timer >= wanderTimer)
            {
                Vector3 newPos = RandomNavSphere(transform.position, wanderRadius, -1);
                agent.SetDestination(newPos);
                timer = 0;
            }

            targetVolume = 0f; // fade out when far
        }

        GetComponent<Animator>().SetFloat("Speed", agent.velocity.magnitude);
        GetComponent<Animator>().speed = agent.velocity.magnitude > 0.1f ? 1f : 0.8f;



        // Smooth sound fade (both in/out)
        if (proximitySound != null)
        {
            proximitySound.volume = Mathf.Lerp(proximitySound.volume, targetVolume, Time.deltaTime * 2f);
        }
    }

    // Generates random points on the NavMesh for wandering
    public static Vector3 RandomNavSphere(Vector3 origin, float dist, int layermask)
    {
        Vector3 randDirection = Random.insideUnitSphere * dist;
        randDirection += origin;
        NavMeshHit navHit;
        NavMesh.SamplePosition(randDirection, out navHit, dist, layermask);
        return navHit.position;
    }
    void OnCollisionStay(Collision collision)
    {
        if (collision.gameObject.CompareTag("Wall"))
        {
            Vector3 pushDir = transform.position - collision.contacts[0].point;
            agent.Move(pushDir.normalized * 0.5f * Time.deltaTime);
        }
    }

}
