using UnityEngine;

public class RoomTrigger : MonoBehaviour
{
    [Header("Room to Trigger")]
    public AnimalRoom animalRoom;

    [Header("Settings")]
    public bool triggerOnlyOnce = false;

    private bool hasBeenTriggered = false;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (triggerOnlyOnce && hasBeenTriggered) return;

        if (collision.CompareTag("Player"))
        {
            hasBeenTriggered = true;

            if (animalRoom != null)
            {
                animalRoom.PlayerEnteredRoom();
            }
        }
    }
}