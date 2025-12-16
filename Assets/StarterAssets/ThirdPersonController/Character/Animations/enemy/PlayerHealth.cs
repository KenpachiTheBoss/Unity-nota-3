using UnityEngine;

public class PlayerHealth : MonoBehaviour
{
    public float maxHealth = 100f;
    public float currentHealth;

    void Awake()
    {
        currentHealth = maxHealth;
    }

    public void TakeDamage(float dmg)
    {
        currentHealth -= dmg;
        Debug.Log("Player HP: " + currentHealth);

        if (currentHealth <= 0f)
            Die();
    }

    void Die()
    {
        Debug.Log("PLAYER MUERTO");
        // Aquí puedes desactivar control, animación muerte, reiniciar escena, etc.
        // Ejemplo simple:
        gameObject.SetActive(false);
    }
}


