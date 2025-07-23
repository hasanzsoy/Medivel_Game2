using UnityEngine;

public class Flames : MonoBehaviour
{
    public float damage = 10f;
    public float damageCooldown = 1f;

    private bool canDamage = true;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && canDamage)
        {
            AnaKarakterCanSistemi playerHP = other.GetComponent<AnaKarakterCanSistemi>();
            if (playerHP != null)
            {
                playerHP.CanAzalt(damage);
                Debug.Log("Player has entered the flames! Hasar verildi.");
                StartCoroutine(DamageCooldown());
            }
        }
    }

    private System.Collections.IEnumerator DamageCooldown()
    {
        canDamage = false;
        yield return new WaitForSeconds(damageCooldown);
        canDamage = true;
    }
}