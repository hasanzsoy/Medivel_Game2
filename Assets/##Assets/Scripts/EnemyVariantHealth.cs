using UnityEngine;
using UnityEngine.UI;

public class EnemyVariantHealth : MonoBehaviour
{
    [Header("HP Sistemi")]
    [SerializeField] private float maxHealth = 100;
    [SerializeField] private Image healthBarFill;
    [SerializeField] private bool canRegenerate = false;
    [SerializeField] private float regenRate = 2f;
    [SerializeField] private float regenDelay = 3f;

    private float currentHealth;
    private float lastDamageTime = -999f;
    private bool isDead = false;

    public System.Action OnDeath;

    public float CurrentHealth => currentHealth;
    public float MaxHealth => maxHealth;
    public bool IsDead => isDead;

    void Awake()
    {
        currentHealth = maxHealth;
        UpdateHealthBar();
        Debug.Log($"[EnemyVariantHealth] Awake: currentHealth = {currentHealth}, maxHealth = {maxHealth}");
    }

    void Update()
    {
        if (isDead) return;

        if (canRegenerate && currentHealth > 0 && currentHealth < maxHealth)
        {
            if (Time.time - lastDamageTime > regenDelay)
            {
                currentHealth += regenRate * Time.deltaTime;
                currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth);
                UpdateHealthBar();
            }
        }
    }

    public void TakeDamage(int damage)
    {
        Debug.Log($"[EnemyVariantHealth] TakeDamage çaðrýldý. damage: {damage}, isDead: {isDead}");
        if (isDead) return;

        float oldHealth = currentHealth;
        currentHealth -= damage;
        currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth);

        Debug.Log($"[EnemyVariantHealth] {damage} damage alýndý. Önceki: {oldHealth}, Þimdi: {currentHealth}");
        UpdateHealthBar();

        lastDamageTime = Time.time;

        if (currentHealth <= 0)
        {
            Debug.Log("[EnemyVariantHealth] currentHealth <= 0, Die() çaðrýlýyor.");
            Die();
        }
    }

    public void Heal(int amount)
    {
        if (isDead) return;

        float oldHealth = currentHealth;
        currentHealth += amount;
        currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth);

        Debug.Log($"[EnemyVariantHealth] {amount} iyileþti. Önceki: {oldHealth}, Þimdi: {currentHealth}");
        UpdateHealthBar();
    }

    public void SetMaxHealth(float newMax)
    {
        maxHealth = newMax;
        currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth);
        UpdateHealthBar();
    }

    private void Die()
    {
        if (isDead) return;
        isDead = true;
        Debug.Log("[EnemyVariantHealth] Enemy öldü. OnDeath invoke ediliyor.");
        OnDeath?.Invoke();
    }

    private void UpdateHealthBar()
    {
        if (healthBarFill != null)
            healthBarFill.fillAmount = (float)currentHealth / maxHealth;
        Debug.Log($"[EnemyVariantHealth] UpdateHealthBar: fillAmount = {healthBarFill?.fillAmount}");
    }
}