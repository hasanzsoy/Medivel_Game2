using System.Collections;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    [SerializeField] private int maxHealth = 100;
    private int currentHealth;

    private void Start()
    {
        currentHealth = maxHealth;
        Debug.Log($"[Enemy] Baþlangýç caný: {currentHealth}");
    }

    public void TakeDamage(int damage)
    {
        int oldHealth = currentHealth;
        currentHealth -= damage;
        currentHealth = Mathf.Max(currentHealth, 0);

        Debug.Log($"[Enemy] {damage} damage alýndý. Önceki: {oldHealth}, Þimdi: {currentHealth}");

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    public void Die()
    {
       // call the animator to play death animation
        Animator animator = GetComponent<Animator>();
        if (animator != null)
        {
            animator.SetTrigger("Die");
        }
       // Wait for the animation to finish before destroying the object
        StartCoroutine(WaitForAnimation(animator));


        Debug.Log("[Enemy] Enemy öldü, obje yok ediliyor.");
        //Destroy(gameObject);
    }

    // Add this method to the Enemy class
    private IEnumerator WaitForAnimation(Animator animator)
    {
        if (animator != null)
        {
            AnimatorStateInfo stateInfo = animator.GetCurrentAnimatorStateInfo(0);
            yield return new WaitForSeconds(stateInfo.length);
        }
    }
}
