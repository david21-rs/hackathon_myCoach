using UnityEngine;
using System.Collections.Generic;

public class RoomManager : MonoBehaviour
{
    public static RoomManager Instance;

    [Header("Room prefab pools — drag your prefabs in here")]
    public GameObject[] combatRoomPrefabs;
    public GameObject[] animalRoomPrefabs;
    public GameObject[] treasureRoomPrefabs;
    public GameObject[] bossRoomPrefabs;

    [Header("How many rooms before the boss?")]
    public int roomsBeforeBoss = 5;

    // Internal state
    private List<RoomType> roomSequence = new List<RoomType>();
    private int currentRoomIndex = 0;
    private RoomController currentRoom;
    private bool animalRoomGuaranteed = false;

    void Awake()
    {
        Instance = this;
    }

    void Start()
    {
        BuildRoomSequence();
        LoadRoom(0);
    }

    // -------------------------------------------------------
    // Build a random sequence that always has at least 1 Animal
    // -------------------------------------------------------
    void BuildRoomSequence()
    {
        roomSequence.Clear();
        animalRoomGuaranteed = false;

        for (int i = 0; i < roomsBeforeBoss; i++)
        {
            // Force an animal room around the middle if none yet
            bool forcedAnimal = !animalRoomGuaranteed && i == roomsBeforeBoss / 2;

            if (forcedAnimal)
            {
                roomSequence.Add(RoomType.Animal);
                animalRoomGuaranteed = true;
            }
            else
            {
                // Random: 50% combat, 20% animal, 30% treasure
                float roll = Random.value;
                if (roll < 0.50f)
                    roomSequence.Add(RoomType.Combat);
                else if (roll < 0.70f)
                {
                    roomSequence.Add(RoomType.Animal);
                    animalRoomGuaranteed = true;
                }
                else
                    roomSequence.Add(RoomType.Treasure);
            }
        }

        // Always end with a boss
        roomSequence.Add(RoomType.Boss);

        Debug.Log("Room sequence: " + string.Join(" → ", roomSequence));
    }

    // -------------------------------------------------------
    // Load a room by index
    // -------------------------------------------------------
    void LoadRoom(int index)
    {
        if (index >= roomSequence.Count)
        {
            Debug.Log("All rooms complete — run over!");
            return;
        }

        // Destroy the old room to keep the scene clean
        if (currentRoom != null)
            Destroy(currentRoom.gameObject);

        RoomType nextType = roomSequence[index];
        GameObject prefab = PickRandomPrefab(nextType);

        if (prefab == null)
        {
            Debug.LogError("No prefab found for room type: " + nextType);
            return;
        }

        // Spawn at world origin (camera is already there)
        GameObject roomObj = Instantiate(prefab, Vector3.zero, Quaternion.identity);
        currentRoom = roomObj.GetComponent<RoomController>();

        if (currentRoom == null)
        {
            Debug.LogError("Room prefab is missing a RoomController script!");
            return;
        }

        // Move the player to the room's entry point
        MovePlayerToEntry(currentRoom.entryPoint);

        // Activate the room (spawns enemies if combat)
        currentRoom.ActivateRoom();
        currentRoom.OnPlayerEntered();

        Debug.Log($"Loaded room {index + 1}/{roomSequence.Count}: {nextType}");
    }

    // -------------------------------------------------------
    // Called by RoomController when room is finished
    // -------------------------------------------------------
    public void OnRoomCleared()
    {
        Debug.Log("Room cleared! Advancing...");

        // Small delay so the player can see the room cleared
        Invoke(nameof(LoadNextRoom), 1.5f);
    }

    void LoadNextRoom()
    {
        currentRoomIndex++;
        LoadRoom(currentRoomIndex);
    }

    // -------------------------------------------------------
    // Helpers
    // -------------------------------------------------------
    GameObject PickRandomPrefab(RoomType type)
    {
        GameObject[] pool = type switch
        {
            RoomType.Combat => combatRoomPrefabs,
            RoomType.Animal => animalRoomPrefabs,
            RoomType.Treasure => treasureRoomPrefabs,
            RoomType.Boss => bossRoomPrefabs,
            _ => combatRoomPrefabs
        };

        if (pool == null || pool.Length == 0)
        {
            Debug.LogWarning("Pool is empty for type: " + type + ". Falling back to combat.");
            pool = combatRoomPrefabs;
        }

        return pool[Random.Range(0, pool.Length)];
    }

    void MovePlayerToEntry(Transform entry)
    {
        if (entry == null) return;

        // Find the player by tag
        GameObject player = GameObject.FindWithTag("Player");
        if (player != null)
            player.transform.position = entry.position;
    }
}