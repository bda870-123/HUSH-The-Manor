using UnityEngine;
using UnityEngine.AI;

public class EnemyAI : MonoBehaviour
{
    [Header("References")]
    public Transform player;
    public Animator animator;
    public PlayerHealth playerHealth;

    [Header("Settings")]
    public float detectionRadius = 15f;
    public float attackRange = 2.5f; // Increased for consistency
    public float patrolRadius = 20f;
    public float attackCooldown = 2f;
    public float patrolIdleTime = 3f;
    public float rotationSpeed = 7f;
    public float attackDuration = 1.0f;

    private NavMeshAgent agent;
    private float cooldownTimer;
    private float idleTimer;
    private float attackTimer;
    private float stuckTimer;

    private Vector3 patrolPoint;
    private bool isPatrolling;
    private bool isIdle;
    private bool isAttacking;

    private enum State { Patrol, Chase, Attack }
    private State currentState;

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        if (animator == null) animator = GetComponent<Animator>();
        if (playerHealth == null && player != null)
            playerHealth = player.GetComponent<PlayerHealth>();

        agent.updateRotation = false; // We rotate manually

        SetNewPatrolPoint();
        currentState = State.Patrol;
    }

    void Update()
    {
        cooldownTimer -= Time.deltaTime;

        // Horizontal-only distance
        Vector3 flatEnemy = new Vector3(transform.position.x, 0, transform.position.z);
        Vector3 flatPlayer = new Vector3(player.position.x, 0, player.position.z);
        float distanceToPlayer = Vector3.Distance(flatEnemy, flatPlayer);

        // Cancel attack if player leaves range
        if (isAttacking && distanceToPlayer > attackRange)
        {
            CancelAttack();
            currentState = State.Chase;
        }

        // Attack timer countdown
        if (isAttacking)
        {
            attackTimer -= Time.deltaTime;
            if (attackTimer <= 0f)
                EndAttack();
        }

        // Decide state
        if (!isAttacking)
        {
            if (distanceToPlayer <= attackRange && cooldownTimer <= 0f)
                currentState = State.Attack;
            else if (distanceToPlayer <= detectionRadius)
                currentState = State.Chase;
            else
                currentState = State.Patrol;
        }

        // Execute behavior
        switch (currentState)
        {
            case State.Patrol: Patrol(); break;
            case State.Chase: ChasePlayer(); break;
            case State.Attack: Attack(); break;
        }

        // Animation
        animator.SetBool("IsWalking", agent.velocity.magnitude > 0.1f && !isAttacking);

        // Smooth rotation
        if (!isAttacking)
            RotateTowardsMovementDirection();

        // Prevent wall sticking
        HandleGlobalUnstuck();
    }

    // ---------------- PATROL ----------------
    public void AttackJumpscare()
    {
        if (Vector3.Distance(transform.position, player.position) <= attackRange)
        {
            FindObjectOfType<JumpscareManager>().TriggerJumpscare();
        }
    }
    void Patrol()
    {
        if (isIdle)
        {
            idleTimer += Time.deltaTime;
            if (idleTimer >= patrolIdleTime)
            {
                SetNewPatrolPoint();
                idleTimer = 0f;
            }
            return;
        }

        if (!isPatrolling || Vector3.Distance(transform.position, patrolPoint) < 1.5f)
        {
            isIdle = true;
            isPatrolling = false;
            agent.ResetPath();
        }
    }

    void SetNewPatrolPoint()
    {
        Vector3 randomDirection = Random.insideUnitSphere * patrolRadius + transform.position;

        if (NavMesh.SamplePosition(randomDirection, out NavMeshHit hit, patrolRadius, NavMesh.AllAreas))
        {
            patrolPoint = hit.position;
            agent.SetDestination(patrolPoint);
            isPatrolling = true;
            isIdle = false;
        }
    }

    // ---------------- CHASE ----------------
    void ChasePlayer()
    {
        isIdle = false;
        isPatrolling = false;

        if (agent.isOnNavMesh && player != null)
        {
            agent.updatePosition = true;
            agent.SetDestination(player.position);
        }
    }

    // ---------------- ATTACK ----------------
    void Attack()
    {
        if (isAttacking) return;

        // Horizontal-only check
        Vector3 flatEnemy = new Vector3(transform.position.x, 0, transform.position.z);
        Vector3 flatPlayer = new Vector3(player.position.x, 0, player.position.z);
        float distance = Vector3.Distance(flatEnemy, flatPlayer);

        if (distance > attackRange)
        {
            currentState = State.Chase;
            return;
        }

        isAttacking = true;
        cooldownTimer = attackCooldown;
        attackTimer = attackDuration;

        agent.ResetPath();

        // Rotate toward player
        Vector3 lookPos = new Vector3(player.position.x, transform.position.y, player.position.z);
        transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(lookPos - transform.position), Time.deltaTime * rotationSpeed);

        // 🔥 REQUIRED or attack will never play
        animator.SetTrigger("Attack");
    }

    public void DealDamage()
    {
        if (Vector3.Distance(transform.position, player.position) <= attackRange)
            playerHealth.TakeDamage(10);
    }

    public void EndAttack()
    {
        isAttacking = false;
        attackTimer = 0f;
    }

    public void CancelAttack()
    {
        if (!isAttacking) return;

        isAttacking = false;
        attackTimer = 0f;
        cooldownTimer = attackCooldown;

        animator.ResetTrigger("Attack");

        animator.CrossFade("Walk", 0.1f);

        if (agent.isOnNavMesh && player != null)
            agent.SetDestination(player.position);
    }

    // ---------------- ROTATION ----------------
    void RotateTowardsMovementDirection()
    {
        if (agent.velocity.sqrMagnitude > 0.1f)
        {
            Quaternion targetRotation = Quaternion.LookRotation(agent.velocity.normalized);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * rotationSpeed);
        }
    }

    // ---------------- UNSTUCK SYSTEM ----------------
    void HandleGlobalUnstuck()
    {
        if (agent.velocity.magnitude < 0.05f && !isAttacking)
        {
            stuckTimer += Time.deltaTime;

            if (stuckTimer >= 0.6f)
            {
                Vector3 backPos = transform.position - transform.forward * 0.7f;

                if (agent.isOnNavMesh)
                    agent.SetDestination(backPos);

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
    public void AttackCheck()
    {
        // If player is close enough
        float flatDistance = Vector3.Distance(
            new Vector3(transform.position.x, 0, transform.position.z),
            new Vector3(player.position.x, 0, player.position.z)
        );

        if (flatDistance <= attackRange)
        {
            // Trigger jumpscare
            FindObjectOfType<JumpscareManager>().TriggerJumpscare();
        }
    }
}