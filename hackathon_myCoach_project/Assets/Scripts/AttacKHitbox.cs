using UnityEngine;

public class AttackHitbox : MonoBehaviour
{
    [Header("Base damage dealt per swing")]
    public float baseDamage = 20f;

    void OnTriggerEnter2D(Collider2D other)
    {
        // Only fire when hitbox is active
        if (!gameObject.activeSelf) return;

        // Try to hit via the universal receiver
        EnemyHitReceiver receiver = other.GetComponent<EnemyHitReceiver>();
        if (receiver != null)
        {
            // Get multiplier from PlayerStatModifier if it exists
            float multiplier = 1f;
            PlayerStatModifier stats = GetComponentInParent<PlayerStatModifier>();
            if (stats != null) multiplier = stats.GetDamageMultiplier();

            receiver.TakeDamage(baseDamage * multiplier);
        }
    }
}