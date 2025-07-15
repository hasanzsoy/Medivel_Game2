using UnityEngine;

public class PlayerAttack : MonoBehaviour
{
    public int attackDamage = 20;
    public Collider attackCollider;

    private void Start()
    {
        if (attackCollider == null)
        {
            Debug.LogWarning("[PlayerAttack] attackCollider atanmam��!");
        }
        else
        {
            attackCollider.enabled = false;
        }
        Debug.Log("[PlayerAttack] Start �a�r�ld�.");
    }

    public void EnableAttackCollider()
    {
        if (attackCollider != null)
        {
            attackCollider.enabled = true;
            Debug.Log("[PlayerAttack] Attack collider ENABLED.");
        }
        else
        {
            Debug.LogWarning("[PlayerAttack] EnableAttackCollider �a�r�ld� ama attackCollider yok!");
        }
    }

    public void DisableAttackCollider()
    {
        if (attackCollider != null)
        {
            attackCollider.enabled = false;
            Debug.Log("[PlayerAttack] Attack collider DISABLED.");
        }
        else
        {
            Debug.LogWarning("[PlayerAttack] DisableAttackCollider �a�r�ld� ama attackCollider yok!");
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        Debug.Log($"[PlayerAttack] OnTriggerEnter: {other.name} tag: {other.tag}");

        if (other.CompareTag("Enemy"))
        {
            Enemy enemyHealth = other.GetComponent<Enemy>();
            if (enemyHealth != null)
            {
                Debug.Log($"[PlayerAttack] EnemyHealty bulundu, {attackDamage} damage veriliyor.");
                enemyHealth.TakeDamage(attackDamage);
            }
            else
            {
                Debug.LogWarning("[PlayerAttack] Enemy tagli objede EnemyHealty componenti yok!");
            }
        }
    }
}

