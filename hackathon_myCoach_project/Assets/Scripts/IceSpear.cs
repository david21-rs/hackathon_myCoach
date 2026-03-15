using UnityEngine;

// Attach to your IceSpear prefab alongside a Rigidbody2D and a Collider2D (Is Trigger ON)
public class IceSpear : MonoBehaviour
{
    public float damage = 24f;
    [SerializeField] private float lifetime = 4f;
    [SerializeField] private GameObject hitVFXPrefab; // Optional impact particle

    void Start()
    {
        Destroy(gameObject, lifetime);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            other.GetComponent<HeroKnight>()?.TakeDamage(damage, this.transform);
            SpawnHitVFX();
            Destroy(gameObject);
        }
        else if (!other.CompareTag("Enemy") && !other.isTrigger)
        {
            // Hits walls or ground
            SpawnHitVFX();
            Destroy(gameObject);
        }
    }

    private void SpawnHitVFX()
    {
        if (hitVFXPrefab != null)
            Instantiate(hitVFXPrefab, transform.position, Quaternion.identity);
    }
}