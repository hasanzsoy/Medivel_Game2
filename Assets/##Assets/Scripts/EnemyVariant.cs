using System.Collections;
using System.Linq;
using UnityEditor.ShaderGraph;
using UnityEngine;
using UnityEngine.AI;

public class EnemyVariant : MonoBehaviour
{
    [Header("AI")]
    public float detectionRange = 10f;
    public float attackRange = 2f;
    public float attackCooldown = 1.5f;
    public float attackDamage = 20;
    public float minMoveSpeed = 1.5f;
    public float maxMoveSpeed = 5.0f;

    [Header("References")]
    public Transform playerTransform;
    public Collider attackCollider;
    public string enemyType = ""; // Enemy türü, örneðin "Kurt", "Troll" vs.

    private EnemyVariantHealth healthSystem;
    private Animator animator;
    private NavMeshAgent agent;
    private float lastAttackTime = -999f;
    private bool isDead = false;
    private bool wasInCombat = false;

    private void Awake()
    {
        animator = GetComponent<Animator>();
        agent = GetComponent<NavMeshAgent>();
        healthSystem = GetComponent<EnemyVariantHealth>();
        if (attackCollider != null)
            attackCollider.enabled = false;
    }

    private void Start()
    {
        if (healthSystem != null)
            healthSystem.OnDeath += Die;

        if (playerTransform == null && GameObject.FindGameObjectWithTag("Player") != null)
            playerTransform = GameObject.FindGameObjectWithTag("Player").transform;

        // NavMeshAgent ve NavMesh durumu için debug
        if (agent == null)
            Debug.LogError("[Enemy] NavMeshAgent bulunamadý!");
        else if (!agent.isOnNavMesh)
            Debug.LogError("[Enemy] NavMeshAgent NavMesh üzerinde deðil!");
        else
            Debug.Log("[Enemy] NavMeshAgent NavMesh üzerinde ve hazýr.");
    }

    private void Update()
    {
        if (isDead || playerTransform == null) return;

        float distance = Vector3.Distance(transform.position, playerTransform.position);
        bool inCombat = (distance <= detectionRange);
        if (inCombat != wasInCombat)
        {
            wasInCombat = inCombat;
            if (AudioManager.Instance != null)
                AudioManager.Instance.SetCombatState(inCombat);
        }

        if (!agent.isOnNavMesh)
        {
            Debug.LogError("[Enemy] Update: NavMeshAgent NavMesh üzerinde deðil!");
            return;
        }

        if (distance <= detectionRange && distance > attackRange)
        {
            float t = Mathf.InverseLerp(attackRange, detectionRange, distance);
            float targetSpeed = Mathf.Lerp(minMoveSpeed, maxMoveSpeed, t);

            agent.isStopped = false;
            agent.speed = targetSpeed;
            agent.SetDestination(playerTransform.position);

            Debug.Log($"[Enemy] Takip: Hedefe gidiliyor. Mesafe: {distance:F2}, Hýz: {targetSpeed:F2}");

            float normalizedSpeed = Mathf.InverseLerp(minMoveSpeed, maxMoveSpeed, agent.velocity.magnitude);
            animator.SetFloat("Speed", normalizedSpeed);

            Debug.Log($"[Enemy] Agent velocity: {agent.velocity.magnitude:F2}, Animator Speed: {normalizedSpeed:F2}");
        }
        else if (distance <= attackRange)
        {
            agent.isStopped = true;
            animator.SetFloat("Speed", 0f);

            Vector3 lookDir = (playerTransform.position - transform.position);
            lookDir.y = 0;
            if (lookDir != Vector3.zero)
                transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(lookDir), Time.deltaTime * 10f);

            if (Time.time - lastAttackTime > attackCooldown)
            {
                animator.SetTrigger("Attack");
                lastAttackTime = Time.time;
                Debug.Log("[Enemy] Saldýrý tetiklendi.");
            }
        }
        else
        {
            agent.isStopped = true;
            animator.SetFloat("Speed", 0f);
            Debug.Log("[Enemy] Oyuncu menzilde deðil, idle.");
        }
    }

    // EnemyAttackEnable ve EnemyAttackDisable animasyon event
    public void EnemyAttackEnable()
    {
        if (attackCollider != null)
            attackCollider.enabled = true;
    }
    public void EnemyAttackDisable()
    {
        if (attackCollider != null)
            attackCollider.enabled = false;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (attackCollider != null && attackCollider.enabled && other.CompareTag("Player"))
        {
            Debug.Log("[Enemy] Oyuncuya hasar verildi.");
            other.GetComponent<AnaKarakterCanSistemi>()?.CanAzalt(attackDamage);
        }
    }

    public void TakeDamage(int damage)
    {
        Debug.Log($"[EnemyVariant] TakeDamage çaðrýldý. damage: {damage}, healthSystem null mu: {healthSystem == null}");
        if (healthSystem != null)
            healthSystem.TakeDamage(damage);
        else
            Debug.LogWarning("[EnemyVariant] healthSystem referansý yok!");
    }

    public void Die()
    {
        Debug.Log("[EnemyVariant] Die() çaðrýldý.");

        isDead = true;

        if (animator != null)
        {
            animator.SetBool("isDead", true);
            Debug.Log($"[EnemyVariant] animator.SetBool('isDead', true) çaðrýldý. Animator objesi: {animator.gameObject.name}");
        }

        agent.isStopped = true;
        if (attackCollider != null)
            attackCollider.enabled = false;

        StartCoroutine(WaitForDeathAnimation());
        Debug.Log("[EnemyVariant] Enemy öldü, obje yok ediliyor.");

        // Quest tracker iþlemini en sona ve try-catch ile al
        try
        {
            if (ActiveQuestTracker.instance != null)
                ActiveQuestTracker.instance.RegisterKill(enemyType);
        }
        catch (System.Exception ex)
        {
            Debug.LogError($"[EnemyVariant] ActiveQuestTracker RegisterKill hatasý: {ex.Message}");
        }
    }
    private IEnumerator WaitForDeathAnimation()
    {
        yield return new WaitForSeconds(2f); // Animasyon süresine göre ayarlayýn
        Destroy(gameObject);
    }
}
