using UnityEngine;
using UnityEngine.AI;

public class EnemyAI : MonoBehaviour
{
    [Header("References")]
    public Transform player;
    
    [Header("Detection Settings")]
    public float chaseRange = 10f;

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

        if (distanceToPlayer <= chaseRange)
        {
            agent.SetDestination(player.position);
            
            bool isMoving = agent.velocity.sqrMagnitude > 0.1f;
            animator.SetBool("isMoving", isMoving);
        }
        else
        {
            agent.ResetPath();
            animator.SetBool("isMoving", false);
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, chaseRange);
    }
}