using UnityEngine;

// Attach to your IceSpear prefab alongside a Rigidbody2D and a Collider2D (Is Trigger ON)
public class IceSpear : MonoBehaviour
{
    public float damage = 24f;
    [SerializeField] private float lifetime = 4f;

    void Start()
    {
        Destroy(gameObject, lifetime);
    }

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