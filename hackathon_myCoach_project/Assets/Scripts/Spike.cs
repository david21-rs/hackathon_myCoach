using UnityEngine;

public class SpikeHazard : MonoBehaviour
{
    [Header("Hazard Settings")]
    [SerializeField] private float damageAmount = 20f;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        // 1. Verify we actually hit the player
        if (collision.CompareTag("Player"))
        {
            HeroKnight player = collision.GetComponent<HeroKnight>();
            if (player != null)
            {
                // 2. Pass this transform so the player's block logic doesn't crash from a null reference
                player.TakeDamage(damageAmount, transform);
            }
        }
    }
}