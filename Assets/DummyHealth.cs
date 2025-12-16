using UnityEngine;

public class DummyHealth : MonoBehaviour
{
    public int maxHealth = 5;
    private int currentHealth;

    void Start()
    {
        currentHealth = maxHealth;
    }

    public void TakeDamage(int amount)
    {
        currentHealth -= amount;
        Debug.Log($"Dummy recibió {amount} de daño. Vida actual: {currentHealth}");

        if (currentHealth <= 0)
        {
            Debug.Log("💀 Dummy murió");
            Destroy(gameObject);
        }
    }
}
