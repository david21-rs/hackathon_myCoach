using UnityEngine;

[System.Serializable]
public struct EnemySpawnSetup
{
    public GameObject enemyPrefab;
    public Transform spawnPoint;
    public Transform patrolA;
    public Transform patrolB;
}

public class RoomController : MonoBehaviour
{
    [SerializeField] private EnemySpawnSetup[] enemiesToSpawn;
    
    private bool hasSpawned = false;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        // Only trigger once, and only if it's the Player
        if (!hasSpawned && collision.CompareTag("Player"))
        {
            hasSpawned = true;
            ActivateRoom();
        }
    }

    private void ActivateRoom()
    {
        foreach (var setup in enemiesToSpawn)
        {
            GameObject newEnemy = Instantiate(setup.enemyPrefab, setup.spawnPoint.position, Quaternion.identity);
            
            PoacherAI poacher = newEnemy.GetComponent<PoacherAI>();
            if (poacher != null)
            {
                poacher.pointA = setup.patrolA;
                poacher.pointB = setup.patrolB;
            }
        }
    }
}