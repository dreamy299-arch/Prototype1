using UnityEngine;
using UnityEngine.AI;

public class EnemyAI : MonoBehaviour
{
    [Header("References")]
    public Transform player;
    
    [Header("Detection Settings")]
    public float chaseRange = 10f;
    public float attackRange = 1.5f; // Distance at which the enemy touches/kills the player

    private NavMeshAgent agent;
    private Animator animator;

    private void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
        animator = GetComponent<Animator>();
    }

    private void Start()
    {
        if (player == null)
        {
            FPController playerScript = Object.FindFirstObjectByType<FPController>();
            if (playerScript != null)
            {
                player = playerScript.transform;
            }
        }
    }

    private void Update()
    {
        if (player == null || PauseManuScript.isPaused) return;

        float distanceToPlayer = Vector3.Distance(transform.position, player.position);

        // 1. Check if close enough to attack/touch the player
        if (distanceToPlayer <= attackRange)
        {
            agent.SetDestination(transform.position); // Stop moving
            animator.SetBool("isMoving", false);

            // Trigger player damage/death
            PlayerHP playerHP = player.GetComponent<PlayerHP>();
            if (playerHP != null)
            {
                playerHP.TakeDamage(100); // Instant kill
            }
        }
        // 2. Otherwise, chase if within chase range
        else if (distanceToPlayer <= chaseRange)
        {
            agent.SetDestination(player.position);
            
            bool isMoving = agent.velocity.sqrMagnitude > 0.1f;
            animator.SetBool("isMoving", isMoving);
        }
        // 3. Out of range, idle
        else
        {
            agent.ResetPath();
            animator.SetBool("isMoving", false);
        }
    }

    private void OnDrawGizmosSelected()
    {
        // Yellow sphere for Chase Range
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, chaseRange);

        // Red sphere for Attack/Touch Range
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRange);
    }
}