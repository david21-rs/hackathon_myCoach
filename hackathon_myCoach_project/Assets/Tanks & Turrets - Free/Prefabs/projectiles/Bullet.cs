using UnityEngine;

public class Bullet : MonoBehaviour
{
    public float damage = 10f; // Changed to float to match your HeroKnight script

    void OnTriggerEnter2D(Collider2D hitInfo)
    {
        // 1. Did we hit the player?
        if (hitInfo.CompareTag("Player"))
        {
            // 2. Look for your specific player script
            HeroKnight playerScript = hitInfo.GetComponent<HeroKnight>();
            
            // 3. If the script exists, trigger the damage method
            if (playerScript != null)
            {
                playerScript.TakeDamage(damage, this.transform);
            }
        }

        // 4. Destroy the bullet no matter what it hits
        Destroy(gameObject);
    }
}