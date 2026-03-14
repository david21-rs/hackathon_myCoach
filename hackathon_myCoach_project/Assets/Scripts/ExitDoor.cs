using UnityEngine;

public class ExitDoor : MonoBehaviour
{
    void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Player")) return;
        
        // Tell the manager to load the next prefab
        RoomManager.Instance.LoadNextRoom();
    }
}