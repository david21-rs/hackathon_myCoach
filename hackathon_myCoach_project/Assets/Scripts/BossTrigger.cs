using UnityEngine;

public class BossTrigger : MonoBehaviour
{
    [SerializeField] private GameObject bossPrefab;
    [SerializeField] private Transform spawnPoint;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            if (bossPrefab != null && spawnPoint != null)
            {
                Instantiate(bossPrefab, spawnPoint.position, Quaternion.identity);
                
                // Immediately destroy this trigger so it only fires once
                Destroy(gameObject); 
            }
        }
    }
}