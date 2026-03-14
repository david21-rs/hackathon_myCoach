using UnityEngine;

public class Bullet : MonoBehaviour
{
    public int damage = 1;

    // This built-in Unity function automatically fires the exact frame 
    // this object's collider touches another collider marked as a Trigger.
    void OnTriggerEnter2D(Collider2D hitInfo)
    {
        // 1. Check if the object we hit has the "Player" tag
        if (hitInfo.CompareTag("Player"))
        {
            // 2. Look for the Health script on the object we just hit
            PlayerHealth playerHealth = hitInfo.GetComponent<PlayerHealth>();
            
            // 3. If it actually has a health script, force it to take damage
            if (playerHealth != null)
            {
                playerHealth.TakeDamage(damage);
            }
        }

        // 4. Destroy the bullet immediately. 
        // Because this is outside the "if player" check, it will destroy itself 
        // whether it hits the player, the floor, a wall, or a ceiling.
        Destroy(gameObject);
    }
}