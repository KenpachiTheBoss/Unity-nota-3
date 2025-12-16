using UnityEngine;
using UnityEngine.AI;

public class EnemyAI : MonoBehaviour
{
    [Header("Referencias")]
    public Transform player;
    public Animator animator;
    public NavMeshAgent agent;

    [Header("Detección (Raycast)")]
    public Transform eyes;                 // punto desde donde “mira”
    public float viewDistance = 15f;
    public float viewAngle = 60f;          // cono de visión
    public LayerMask playerMask;           // layer del player
    public LayerMask obstacleMask;         // paredes/obstáculos

    [Header("Movimiento")]
    public float chaseSpeed = 3.5f;
    public float runSpeed = 5.5f;
    public float chaseStopDistance = 1.8f;

    [Header("Ataque")]
    public float attackRange = 2.0f;
    public float attackDamage = 15f;
    public float attackCooldown = 1.2f;

    private float nextAttackTime;
    private bool playerDetected;

    private void Reset()
    {
        agent = GetComponent<NavMeshAgent>();
        animator = GetComponentInChildren<Animator>();
    }

    private void Awake()
    {
        if (!agent) agent = GetComponent<NavMeshAgent>();
        if (!animator) animator = GetComponentInChildren<Animator>();

        agent.stoppingDistance = chaseStopDistance;
    }

    private void Update()
    {
        if (!player) return;

        playerDetected = CanSeePlayerRaycast();

        if (playerDetected)
        {
            ChaseAndAttack();
        }
        else
        {
            Idle();
        }

        UpdateAnimatorSpeed();
    }

    bool CanSeePlayerRaycast()
    {
        Vector3 origin = eyes ? eyes.position : (transform.position + Vector3.up * 1.6f);

        Vector3 dirToPlayer = (player.position - origin);
        float dist = dirToPlayer.magnitude;
        if (dist > viewDistance) return false;

        Vector3 dirNormalized = dirToPlayer.normalized;

        // ✅ DEBUG: ver el rayo en la Scene mientras juegas
        Debug.DrawRay(origin, dirNormalized * viewDistance, Color.green);

        // ✅ Ángulo usando forward "real" del agente (más fiable que transform.forward)
        Vector3 forward = agent ? agent.transform.forward : transform.forward;
        float angle = Vector3.Angle(forward, dirNormalized);
        if (angle > viewAngle * 0.5f) return false;

        // ✅ 1) Si hay obstáculo entre medio, no lo ve (pero ignorando el Player)
        if (Physics.Raycast(origin, dirNormalized, out RaycastHit blockHit, dist, obstacleMask))
        {
            // Si el obstáculo detectado es parte del player, no cuenta como bloqueo
            if (blockHit.transform != player && blockHit.transform.root != player)
                return false;
        }

        // ✅ 2) Confirmar que lo que ve es el player (por layer)
        if (Physics.Raycast(origin, dirNormalized, out RaycastHit hit, dist, playerMask))
        {
            bool ok = hit.transform == player || hit.transform.root == player;

            // (Opcional) Debug en consola:
            // if (ok) Debug.Log("✅ Player detectado por Raycast");

            return ok;
        }

        return false;
    }

    void ChaseAndAttack()
    {
        float dist = Vector3.Distance(transform.position, player.position);

        agent.isStopped = false;
        agent.speed = (dist > 6f) ? runSpeed : chaseSpeed;
        agent.SetDestination(player.position);

        FaceTarget(agent.steeringTarget);

        if (dist <= attackRange)
        {
            TryAttack();
        }
    }

    void TryAttack()
    {
        if (Time.time < nextAttackTime) return;

        nextAttackTime = Time.time + attackCooldown;

        agent.isStopped = true; // se frena al atacar
        animator.SetTrigger("Attack");
    }

    // Llamar desde Animation Event en el clip de ataque
    public void DealDamage()
    {
        if (!player) return;

        float dist = Vector3.Distance(transform.position, player.position);
        if (dist > attackRange + 0.5f) return;

        var ph = player.GetComponent<PlayerHealth>();
        if (ph) ph.TakeDamage(attackDamage);
    }

    void Idle()
    {
        agent.isStopped = true;
        // aquí puedes patrullar si quieres
    }

    void FaceTarget(Vector3 targetPos)
    {
        Vector3 dir = (targetPos - transform.position);
        dir.y = 0f;
        if (dir.sqrMagnitude < 0.001f) return;

        Quaternion rot = Quaternion.LookRotation(dir);
        transform.rotation = Quaternion.Slerp(transform.rotation, rot, Time.deltaTime * 10f);
    }

    void UpdateAnimatorSpeed()
    {
        float normalized = agent.velocity.magnitude / runSpeed;
        animator.SetFloat("Speed", normalized);
    }

    private void OnDrawGizmosSelected()
    {
        Vector3 origin = eyes ? eyes.position : (transform.position + Vector3.up * 1.6f);

        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, viewDistance);

        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRange);

        Vector3 left = Quaternion.Euler(0, -viewAngle * 0.5f, 0) * transform.forward;
        Vector3 right = Quaternion.Euler(0, viewAngle * 0.5f, 0) * transform.forward;
        Gizmos.color = Color.cyan;
        Gizmos.DrawRay(origin, left * viewDistance);
        Gizmos.DrawRay(origin, right * viewDistance);
    }
}
