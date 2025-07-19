using System.Collections;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;

public class Enemy : MonoBehaviour
{
    [Header("Health")]
    [SerializeField] private float maxHealth = 100;
    private float currentHealth;
    public Image healthBarFill; 

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

    private Animator animator;
    private NavMeshAgent agent;
    private float lastAttackTime = -999f;
    private bool isDead = false;


    private void Awake()
    {
        animator = GetComponent<Animator>();
        agent = GetComponent<NavMeshAgent>();
        if (attackCollider != null)
            attackCollider.enabled = false;
    }

    private void Start()
    {
        currentHealth = maxHealth;
        if (playerTransform == null && GameObject.FindGameObjectWithTag("Player") != null)
            playerTransform = GameObject.FindGameObjectWithTag("Player").transform;
        UpdateHealthBar();

        // NavMeshAgent ve NavMesh durumu i�in debug
        if (agent == null)
            Debug.LogError("[Enemy] NavMeshAgent bulunamad�!");
        else if (!agent.isOnNavMesh)
            Debug.LogError("[Enemy] NavMeshAgent NavMesh �zerinde de�il!");
        else
            Debug.Log("[Enemy] NavMeshAgent NavMesh �zerinde ve haz�r.");
    }

    private void Update()
    {
        if (isDead || playerTransform == null) return;

        float distance = Vector3.Distance(transform.position, playerTransform.position);

        if (!agent.isOnNavMesh)
        {
            Debug.LogError("[Enemy] Update: NavMeshAgent NavMesh �zerinde de�il!");
            return;
        }

        if (distance <= detectionRange && distance > attackRange)
        {
            // H�z oran�n� mesafeye g�re hesapla (yak�nken min, uzakken max)
            float t = Mathf.InverseLerp(attackRange, detectionRange, distance);
            float targetSpeed = Mathf.Lerp(minMoveSpeed, maxMoveSpeed, t);

            agent.isStopped = false;
            agent.speed = targetSpeed;
            agent.SetDestination(playerTransform.position);

            Debug.Log($"[Enemy] Takip: Hedefe gidiliyor. Mesafe: {distance:F2}, H�z: {targetSpeed:F2}");


            // Animator Speed parametresi i�in normalize et (walk: 0.3, run: 1)
            float normalizedSpeed = Mathf.InverseLerp(minMoveSpeed, maxMoveSpeed, agent.velocity.magnitude);
            // Walk threshold: 0.3, Run: 0.3-1
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
                Debug.Log("[Enemy] Sald�r� tetiklendi.");
            }
        }
        else
        {
            agent.isStopped = true;
            animator.SetFloat("Speed", 0f);
            Debug.Log("[Enemy] Oyuncu menzilde de�il, idle.");
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
        if (isDead) return;

        float oldHealth = currentHealth;
        currentHealth -= damage;
        currentHealth = Mathf.Max(currentHealth, 0);

        Debug.Log($"[Enemy] {damage} damage al�nd�. �nceki: {oldHealth}, �imdi: {currentHealth}");
        UpdateHealthBar();
        if (currentHealth <= 0)
        {
            Die();
        }
    }

    public void Die()
    {
        if (isDead) return;
        isDead = true;

        animator.SetBool("isDead", true);
        agent.isStopped = true;
        if (attackCollider != null)
            attackCollider.enabled = false;

        StartCoroutine(WaitForDeathAnimation());

        Debug.Log("[Enemy] Enemy �ld�, obje yok ediliyor.");
    }
    private IEnumerator WaitForDeathAnimation()
    {
        // �l�m animasyonunun s�resi kadar bekle, sonra objeyi yok et
        yield return new WaitForSeconds(2f); // Animasyon s�resine g�re ayarlay�n
        Destroy(gameObject);
    }

    void UpdateHealthBar()
    {
        if (healthBarFill != null)
            healthBarFill.fillAmount = (float)currentHealth / maxHealth;
    }
}
