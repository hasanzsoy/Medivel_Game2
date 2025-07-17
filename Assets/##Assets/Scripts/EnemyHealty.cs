using UnityEngine;
using UnityEngine.UI;
public class EnemyHealty : MonoBehaviour
{
    [SerializeField] private int maxHealth = 100;
    private int currentHealth;

    public Image healthBarFill;

    private void Start()
    {
        currentHealth = maxHealth;
        Debug.Log($"[EnemyHealty] Ba�lang�� can�: {currentHealth}");
    }

    public void TakeDamage(int damage)
    {
        int oldHealth = currentHealth;
        currentHealth -= damage;
        currentHealth = Mathf.Max(currentHealth, 0);

        Debug.Log($"[EnemyHealty] {damage} damage al�nd�. �nceki: {oldHealth}, �imdi: {currentHealth}");
        UpdateHealthBar();

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    public void Die()
    {
        Debug.Log("[EnemyHealty] Enemy �ld�, obje yok ediliyor.");
        Destroy(gameObject);
    }
    void UpdateHealthBar()
    {
        float fillAmount = currentHealth / maxHealth;
        healthBarFill.fillAmount = fillAmount;
    }

}
