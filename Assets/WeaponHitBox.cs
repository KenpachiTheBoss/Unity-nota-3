using UnityEngine;

public class WeaponHitBox : MonoBehaviour
{
    public int damage = 1;
    public string targetTag = "Enemy";

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag(targetTag)) return;

        DummyHealth health = other.GetComponent<DummyHealth>();
        if (health != null)
        {
            health.TakeDamage(damage);
        }
    }
}
