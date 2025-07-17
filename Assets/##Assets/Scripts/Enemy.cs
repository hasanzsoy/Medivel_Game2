using System.Collections;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;

public class Enemy : MonoBehaviour
{
    [Header("Health")]
    [SerializeField] private int maxHealth = 100;
    private int currentHealth;
    public Image healthBarFill; // UI'deki sa�l�k �ubu�u

    [Header("AI")]
    public float detectionRange = 10f;
    public float attackRange = 2f;
    public float attackCooldown = 1.5f;
    public int attackDamage = 20;

    [Header("References")]
    public Transform playerTransform;
    public Collider attackCollider; // Sald�r� s�ras�nda a��l�p kapanacak collider

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
        Debug.Log($"[Enemy] Ba�lang�� sa�l���: {currentHealth}");
        if (playerTransform == null && GameObject.FindGameObjectWithTag("Player") != null)
            playerTransform = GameObject.FindGameObjectWithTag("Player").transform;
    }

    private void Update()
    {
        if (isDead || playerTransform == null) return;

        float distance = Vector3.Distance(transform.position, playerTransform.position);

        if (distance <= detectionRange && distance > attackRange)
        {
            // Takip et
            agent.isStopped = false;
            agent.SetDestination(playerTransform.position);

            animator.SetBool("isWalking", true);
            animator.SetBool("isRunning", false);
            animator.SetFloat("Speed", agent.velocity.magnitude);
        }
        else if (distance <= attackRange)
        {
            // Sald�r�ya haz�rlan
            agent.isStopped = true;
            animator.SetBool("isWalking", false);
            animator.SetBool("isRunning", false);
            animator.SetFloat("Speed", 0f);

            // Y�ze d�n
            Vector3 lookDir = (playerTransform.position - transform.position);
            lookDir.y = 0;
            if (lookDir != Vector3.zero)
                transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(lookDir), Time.deltaTime * 10f);

            // Sald�r�
            if (Time.time - lastAttackTime > attackCooldown)
            {
                animator.SetTrigger("Attack");
                lastAttackTime = Time.time;
            }
        }
        else
        {
            // Oyuncu menzilde de�il
            agent.isStopped = true;
            animator.SetBool("isWalking", false);
            animator.SetBool("isRunning", false);
            animator.SetFloat("Speed", 0f);
        }
    }

    // Animator'da Attack animasyonunun uygun frame'ine event ekleyin:
    // EnemyAttackEnable ve EnemyAttackDisable fonksiyonlar�n� �a��r�n.
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
    // Sald�r� collider'� ba�ka bir objeye �arpt���nda hasar vermek i�in:
    private void OnTriggerEnter(Collider other)
    {
        if (attackCollider != null && attackCollider.enabled && other.CompareTag("Player"))
        {
            // Burada oyuncunun sa�l�k scriptine eri�ip hasar verebilirsiniz
            // �rnek: other.GetComponent<PlayerHealth>()?.TakeDamage(attackDamage);
            other.GetComponent<AnaKarakterCanSistemi>()?.CanAzalt(attackDamage);
        }
    }


    public void TakeDamage(int damage)
    {
        if (isDead) return;

        int oldHealth = currentHealth;
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

        if (animator != null)
            animator.SetTrigger("Die");

        agent.isStopped = true;
        if (attackCollider != null)
            attackCollider.enabled = false;

        StartCoroutine(WaitForAnimation(animator));
        Debug.Log("[Enemy] Enemy �ld�, obje yok ediliyor.");
    }
    void UpdateHealthBar()
    {
        float fillAmount = currentHealth / maxHealth;
        healthBarFill.fillAmount = fillAmount;
    }
    private IEnumerator WaitForAnimation(Animator animator)
    {
        if (animator != null)
        {
            AnimatorStateInfo stateInfo = animator.GetCurrentAnimatorStateInfo(0);
            yield return new WaitForSeconds(stateInfo.length);
        }
        Destroy(gameObject);
    }
}
