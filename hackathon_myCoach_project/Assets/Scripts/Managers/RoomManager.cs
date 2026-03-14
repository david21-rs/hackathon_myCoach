using UnityEngine;

public class RoomManager : MonoBehaviour
{
    public static RoomManager Instance;

    [Header("All Room Prefabs")]
    public GameObject[] roomPrefabs;

    private GameObject currentRoomObj;
    private GameObject currentExitDoor;
    private bool roomCleared = false;

    void Awake()
    {
        Instance = this;
    }

    void Start()
    {
        LoadNextRoom();
    }

    public void LoadNextRoom()
    {
        if (currentRoomObj != null) Destroy(currentRoomObj);

        // 1. Get a random room from the list and load it
        GameObject randomPrefab = roomPrefabs[Random.Range(0, roomPrefabs.Length)];
        currentRoomObj = Instantiate(randomPrefab, Vector3.zero, Quaternion.identity);
        roomCleared = false;

        // 2. Move player to the entry point
        Transform entry = currentRoomObj.transform.Find("EntryPoint");
        if (entry != null)
        {
            GameObject player = GameObject.FindWithTag("Player");
            if (player != null) player.transform.position = entry.position;
        }

        // 3. Find the exit door and hide it
        Transform exit = currentRoomObj.transform.Find("ExitPoint");
        if (exit != null)
        {
            currentExitDoor = exit.gameObject;
            currentExitDoor.SetActive(false);
        }

        // 4. Start continuously checking for enemies
        CancelInvoke(nameof(CheckEnemies));
        InvokeRepeating(nameof(CheckEnemies), 0.5f, 0.5f);
    }

    private void CheckEnemies()
    {
        if (roomCleared) return;

        // 5. When enemies hit 0, set door as active
        GameObject[] enemies = GameObject.FindGameObjectsWithTag("Enemy");
        if (enemies.Length == 0)
        {
            roomCleared = true;
            Debug.Log("all killed");
            if (currentExitDoor != null) currentExitDoor.SetActive(true);
            CancelInvoke(nameof(CheckEnemies));
        }
    }
}