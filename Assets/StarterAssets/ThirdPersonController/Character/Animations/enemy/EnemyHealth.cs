using UnityEngine;

public class EnemyHealth : MonoBehaviour
{
    public float maxHealth = 60f;
    public float currentHealth;
    public Animator animator;

    void Awake()
    {
        currentHealth = maxHealth;
        if (!animator) animator = GetComponentInChildren<Animator>();
    }

    public void TakeDamage(float dmg)
    {
        currentHealth -= dmg;
        Debug.Log("Enemy HP: " + currentHealth);

        if (currentHealth <= 0f)
            Die();
    }

    void Die()
    {
        Debug.Log("ENEMIGO MUERTO");
        if (animator) animator.SetBool("IsDead", true);

        // Desactivar IA / agente para que no siga moviéndose
        var ai = GetComponent<EnemyAI>();
        if (ai) ai.enabled = false;

        var agent = GetComponent<UnityEngine.AI.NavMeshAgent>();
        if (agent) agent.enabled = false;

        // Opcional: quitar collider
        var col = GetComponent<Collider>();
        if (col) col.enabled = false;

        // Opcional: destruir después de 3 segundos
        Destroy(gameObject, 3f);
    }
}
