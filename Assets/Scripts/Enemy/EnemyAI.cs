using UnityEngine;
using UnityEngine.AI;

/// <summary>
/// Düşman AI — oyuncuyu görünce önce 2 sn yürür, sonra koşar.
/// Animator parametreleri: Speed (Float), Attack (Trigger), Dead (Trigger)
/// Bu scripti Basic_Bandit objesine ekle.
/// </summary>
[RequireComponent(typeof(NavMeshAgent))]
public class EnemyAI : MonoBehaviour
{
    [Header("Algılama")]
    [SerializeField] private float detectionRange = 12f;
    [SerializeField] private float attackRange = 1.8f;

    [Header("Hareket")]
    [SerializeField] private float walkSpeed = 2f;
    [SerializeField] private float runSpeed = 5f;
    [SerializeField] private float walkDuration = 2f;

    [Header("Saldırı")]
    [SerializeField] private float attackDamage = 15f;
    [SerializeField] private float attackCooldown = 1.5f;
    [SerializeField] private float attackAnimDuration = 1.0f;

    private enum State { Idle, Walk, Run, Attack, Dead }
    private State currentState = State.Idle;

    private NavMeshAgent agent;
    private Animator anim;
    private Transform player;
    private HealthSystem healthSystem;

    private float walkTimer = 0f;
    private float nextAttackTime = 0f;
    private float attackAnimTimer = 0f;
    private bool isAttacking = false;
    private bool isDead = false;

    private static readonly int SpeedHash = Animator.StringToHash("Speed");
    private static readonly int AttackHash = Animator.StringToHash("Attack");
    private static readonly int DeadHash = Animator.StringToHash("Dead");

    private void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
        anim = GetComponentInChildren<Animator>();
        healthSystem = GetComponent<HealthSystem>();
    }

    private void Start()
    {
        GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
        if (playerObj != null)
            player = playerObj.transform;

        agent.speed = walkSpeed;
        SetState(State.Idle);

        if (healthSystem != null)
            healthSystem.OnDeath.AddListener(Die);
    }

    private void Update()
    {
        if (isDead || player == null) return;

        float distance = Vector3.Distance(transform.position, player.position);

        if (isAttacking)
        {
            attackAnimTimer -= Time.deltaTime;
            if (attackAnimTimer <= 0f)
                isAttacking = false;
        }

        switch (currentState)
        {
            case State.Idle: UpdateIdle(distance); break;
            case State.Walk: UpdateWalk(distance); break;
            case State.Run: UpdateRun(distance); break;
            case State.Attack: UpdateAttack(distance); break;
        }

        anim?.SetFloat(SpeedHash, agent.velocity.magnitude);
    }

    private void UpdateIdle(float distance)
    {
        if (distance <= detectionRange)
        {
            walkTimer = walkDuration;
            SetState(State.Walk);
        }
    }

    private void UpdateWalk(float distance)
    {
        if (distance <= attackRange)
        {
            SetState(State.Attack);
            return;
        }

        if (distance > detectionRange * 1.5f)
        {
            SetState(State.Idle);
            return;
        }

        walkTimer -= Time.deltaTime;
        if (walkTimer <= 0f)
        {
            SetState(State.Run);
            return;
        }

        agent.isStopped = false;
        agent.speed = walkSpeed;
        agent.SetDestination(player.position);
    }

    private void UpdateRun(float distance)
    {
        if (distance <= attackRange)
        {
            SetState(State.Attack);
            return;
        }

        if (distance > detectionRange * 1.5f)
        {
            SetState(State.Idle);
            return;
        }

        agent.isStopped = false;
        agent.speed = runSpeed;
        agent.SetDestination(player.position);
    }

    private void UpdateAttack(float distance)
    {
        // Oyuncuya bak
        Vector3 dir = (player.position - transform.position).normalized;
        dir.y = 0;
        if (dir != Vector3.zero)
            transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(dir), Time.deltaTime * 10f);

        // Animasyon devam ediyorsa bekle
        if (isAttacking) return;

        // Oyuncu uzaklaştıysa kovala
        if (distance > attackRange * 1.5f)
        {
            SetState(State.Run);
            return;
        }

        // Saldır
        if (Time.time >= nextAttackTime)
        {
            nextAttackTime = Time.time + attackCooldown;
            isAttacking = true;
            attackAnimTimer = attackAnimDuration;
            anim?.SetTrigger(AttackHash);

            HealthSystem playerHealth = player.GetComponent<HealthSystem>();
            if (playerHealth != null)
                playerHealth.TakeDamage(attackDamage);
        }
    }

    private void Die()
    {
        if (isDead) return;
        isDead = true;

        SetState(State.Dead);
        agent.isStopped = true;
        agent.enabled = false;

        anim?.SetTrigger(DeadHash);
        Invoke(nameof(Deactivate), 3f);
    }

    private void Deactivate()
    {
        gameObject.SetActive(false);
    }

    private void SetState(State newState)
    {
        currentState = newState;

        if (newState == State.Idle || newState == State.Attack || newState == State.Dead)
        {
            agent.isStopped = true;
            agent.velocity = Vector3.zero;
        }
        else
        {
            agent.isStopped = false;
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, detectionRange);
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRange);
    }
}