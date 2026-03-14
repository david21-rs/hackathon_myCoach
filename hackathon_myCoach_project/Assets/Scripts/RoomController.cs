using UnityEngine;

public class RoomController : MonoBehaviour
{
    [Header("What kind of room is this?")]
    public RoomType roomType = RoomType.Combat;

    [Header("Drag the entry door object here")]
    public Transform entryPoint;   // Where the player spawns when entering

    [Header("Drag the exit door object here")]
    public Transform exitPoint;    // Where the next room gets attached

    [Header("Enemies to spawn (Combat rooms only)")]
    public EnemySpawnSetup[] enemiesToSpawn;

    [Header("Animal spawn point (Animal rooms only)")]
    public Transform animalSpawnPoint;

    [Header("Item pedestal position (Treasure rooms only)")]
    public Transform itemPedestalPoint;

    // Tracked internally
    private bool roomCleared = false;
    private int enemiesAlive = 0;

    // RoomManager calls this when the room is activated
    public void ActivateRoom()
    {
        if (roomType == RoomType.Combat || roomType == RoomType.Boss)
        {
            SpawnEnemies();
        }
    }

    private void SpawnEnemies()
    {
        enemiesAlive = enemiesToSpawn.Length;

        foreach (var setup in enemiesToSpawn)
        {
            if (setup.enemyPrefab == null) continue;

            Vector3 spawnPos = setup.spawnPoint != null
                ? setup.spawnPoint.position
                : transform.position; // fallback if no spawn point set

            GameObject enemy = Instantiate(setup.enemyPrefab, spawnPos, Quaternion.identity);

            PoacherAI ai = enemy.GetComponent<PoacherAI>();
            if (ai != null)
            {
                ai.pointA = setup.patrolA;
                ai.pointB = setup.patrolB;
            }

            // Tell each enemy who to report to when they die
            EnemyHealth health = enemy.GetComponent<EnemyHealth>();
            if (health != null)
            {
                health.onDeath += OnEnemyDied;
            }
        }
    }

    // Called by EnemyHealth when an enemy dies
    public void OnEnemyDied()
    {
        enemiesAlive--;
        if (enemiesAlive <= 0 && !roomCleared)
        {
            roomCleared = true;
            RoomManager.Instance.OnRoomCleared();
        }
    }

    // Non-combat rooms clear immediately when entered
    public void OnPlayerEntered()
    {
        if (roomType == RoomType.Animal || roomType == RoomType.Treasure)
        {
            // Give player a second before auto-clearing
            Invoke(nameof(AutoClear), 0.5f);
        }
    }

    private void AutoClear()
    {
        if (!roomCleared)
        {
            roomCleared = true;
            RoomManager.Instance.OnRoomCleared();
        }
    }
}

// Keep your existing struct here too
[System.Serializable]
public struct EnemySpawnSetup
{
    public GameObject enemyPrefab;
    public Transform spawnPoint;
    public Transform patrolA;
    public Transform patrolB;
}