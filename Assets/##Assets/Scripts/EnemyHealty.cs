using UnityEngine;

public class EnemyHealty : MonoBehaviour
{
    public int maxHealth = 100;
    [SerializeField] private int currentHealth;

    public void Start()
    {
        currentHealth = maxHealth;
    }

    public void TakeDamage(int damage)
    {
        currentHealth -= damage;
        Debug.Log($"Enemy took {damage} damage. Current Health: {currentHealth}");

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    public void Die()
    {
        Debug.Log("Enemy died");
        
        Destroy(gameObject);
    }

}
