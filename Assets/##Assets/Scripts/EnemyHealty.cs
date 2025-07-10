using UnityEngine;

public class EnemyHealty : MonoBehaviour
{
    [SerializeField] private int maxHealth = 100;
    private int currentHealth;

    private void Start()
    {
        currentHealth = maxHealth;
        Debug.Log($"[EnemyHealty] Baþlangýç caný: {currentHealth}");
    }

    public void TakeDamage(int damage)
    {
        int oldHealth = currentHealth;
        currentHealth -= damage;
        currentHealth = Mathf.Max(currentHealth, 0);

        Debug.Log($"[EnemyHealty] {damage} damage alýndý. Önceki: {oldHealth}, Þimdi: {currentHealth}");

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    public void Die()
    {
        Debug.Log("[EnemyHealty] Enemy öldü, obje yok ediliyor.");
        Destroy(gameObject);
    }
}
