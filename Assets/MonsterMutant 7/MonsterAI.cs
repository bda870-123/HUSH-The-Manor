using UnityEngine;
using UnityEngine.AI;

public class EnemyAI : MonoBehaviour
{
    [Header("References")]
    public Transform player;
    public Animator animator;
    public PlayerHealth playerHealth;

    [Header("Attack System")]
    public AttackHitbox attackHitbox;     // <-- NEW
    public JumpscareManager jumpscare;    // <-- NEW

    [Header("Settings")]
    public float detectionRadius = 15f;
    public float attackRange = 1f;
    public float patrolRadius = 20f;
    public float attackCooldown = 2f;
    public float patrolIdleTime = 2f;
    public float rotationSpeed = 10f;
    public float attackDuration = 1.0f;

    private NavMeshAgent agent;
    private float cooldownTimer;
    private float attackTimer;
    private float idleTimer;
    private float stuckTimer;

    private Vector3 patrolPoint;
    private bool isAttacking;

    private enum State { Patrol, Chase, Attack }
    private State currentState;

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();

        agent.stoppingDistance = 1.2f;
        agent.autoRepath = true;

        if (animator == null) animator = GetComponent<Animator>();
        if (playerHealth == null && player != null)
            playerHealth = player.GetComponent<PlayerHealth>();

        SetNewPatrolPoint();
        currentState = State.Patrol;
    }

    void Update()
    {
        cooldownTimer -= Time.deltaTime;

        float distToPlayer = Vector3.Distance(transform.position, player.position);

        if (isAttacking)
        {
            attackTimer -= Time.deltaTime;
            if (attackTimer <= 0f)
                EndAttack();
        }

        if (!isAttacking)
        {
            if (distToPlayer <= attackRange && cooldownTimer <= 0f)
                currentState = State.Attack;
            else if (distToPlayer <= detectionRadius)
                currentState = State.Chase;
            else
                currentState = State.Patrol;
        }

        switch (currentState)
        {
            case State.Patrol: Patrol(); break;
            case State.Chase: ChasePlayer(); break;
            case State.Attack: Attack(); break;
        }

        animator.SetBool("IsWalking", agent.velocity.magnitude > 0.1f && !isAttacking);

        if (!isAttacking)
            RotateTowardsMovementDirection();

        HandleGlobalUnstuck();
    }

    /* ===========================================================
       PATROL
       =========================================================== */
    void Patrol()
    {
        if (!agent.hasPath || agent.pathStatus == NavMeshPathStatus.PathInvalid)
        {
            SetNewPatrolPoint();
            return;
        }

        if (!agent.pathPending && agent.remainingDistance <= 0.2f)
        {
            idleTimer += Time.deltaTime;

            if (idleTimer >= patrolIdleTime)
            {
                SetNewPatrolPoint();
                idleTimer = 0f;
            }
        }
    }

    void SetNewPatrolPoint()
    {
        float minDist = 5f;

        Vector3 randomDirection =
            Random.insideUnitSphere.normalized *
            Random.Range(minDist, patrolRadius) +
            transform.position;

        if (NavMesh.SamplePosition(randomDirection, out NavMeshHit hit, patrolRadius, NavMesh.AllAreas))
        {
            patrolPoint = hit.position;
            agent.SetDestination(patrolPoint);
        }
        else
        {
            SetNewPatrolPoint();
        }
    }

    /* ===========================================================
       CHASE
       =========================================================== */
    void ChasePlayer()
    {
        if (player == null || !agent.isOnNavMesh)
            return;

        float dist = Vector3.Distance(transform.position, player.position);

        if (dist > agent.stoppingDistance + 0.3f)
        {
            agent.SetDestination(player.position);
        }

        if (dist <= agent.stoppingDistance + 0.1f)
        {
            Vector3 fallback = transform.position - transform.forward * 0.6f;
            agent.SetDestination(fallback);
        }

        if (!agent.hasPath || agent.pathStatus == NavMeshPathStatus.PathInvalid)
        {
            SetNewPatrolPoint();
            currentState = State.Patrol;
        }
    }

    /* ===========================================================
       ATTACK
       =========================================================== */
    void Attack()
    {
        if (isAttacking) return;

        float dist = Vector3.Distance(transform.position, player.position);

        // ✔ MUST be close enough to attack
        if (dist > attackRange)
        {
            currentState = State.Chase;
            return;
        }

        // ✔ NEW: Must be FACING the player enough
        Vector3 dir = (player.position - transform.position).normalized;
        float dot = Vector3.Dot(transform.forward, dir);

        // If not facing the player, keep chasing (Prevents freeze)
        if (dot < 0.7f)
        {
            currentState = State.Chase;
            return;
        }

        // ✔ If we reach this point → begin the attack
        isAttacking = true;
        cooldownTimer = attackCooldown;
        attackTimer = attackDuration + 0.3f;

        // Stop moving during attack
        agent.SetDestination(transform.position);

        // Rotate smoothly toward the player
        Vector3 lookPos = new Vector3(player.position.x, transform.position.y, player.position.z);
        transform.rotation = Quaternion.Slerp(transform.rotation,
            Quaternion.LookRotation(lookPos - transform.position),
            Time.deltaTime * rotationSpeed);

        animator.ResetTrigger("Attack");
        animator.SetTrigger("Attack");
    }

    /* ===========================================================
       NEW: ANIMATION EVENT FUNCTION
       =========================================================== */
    public void AttackCheck()
    {
        // Hitbox decides the exact distance.
        if (attackHitbox != null && attackHitbox.playerInRange)
        {
            // Trigger jumpscare instead of normal damage
            jumpscare.TriggerJumpscare();
        }
    }

    public void EndAttack()
    {
        isAttacking = false;

        if (agent.isOnNavMesh)
            agent.SetDestination(transform.position - transform.forward * 1f);
    }

    /* ===========================================================
       UNSTUCK HANDLER
       =========================================================== */
    void HandleGlobalUnstuck()
    {
        if (agent.velocity.magnitude < 0.05f && !isAttacking)
        {
            stuckTimer += Time.deltaTime;

            if (stuckTimer >= 2f)
            {
                SetNewPatrolPoint();
                currentState = State.Patrol;
                stuckTimer = 0f;
            }
        }
        else
        {
            stuckTimer = 0f;
        }
    }

    void RotateTowardsMovementDirection()
    {
        if (agent.velocity.sqrMagnitude > 0.1f)
        {
            Quaternion targetRotation = Quaternion.LookRotation(agent.velocity.normalized);
            transform.rotation = Quaternion.Slerp(
                transform.rotation,
                targetRotation,
                Time.deltaTime * rotationSpeed
            );
        }
    }
}