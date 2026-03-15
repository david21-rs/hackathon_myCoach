using UnityEngine;

public class AnimalRoom : MonoBehaviour
{
    [Header("Wire in Inspector")]
    public AnimalData animal;
    public GameObject exitDoor;
    public GameObject itemPickupPrefab;
    public Transform itemSpawnPoint;
    public int killsRequired;

    private bool hasTriggeredIntro = false;
    private bool questTurnedIn = false;

    public void PlayerEnteredRoom()
    {
        if (!hasTriggeredIntro)
        {
            hasTriggeredIntro = true;
            LockDoor();
            QuestTracker.Instance.RegisterQuest(animal.animalID, killsRequired);

            DialogueUI.Instance.ShowDialogue(
                animal.portrait,
                animal.animalName,
                animal.introDialogue,
                onClose: OnIntroComplete
            );
            return;
        }

        if (!questTurnedIn && QuestTracker.Instance.IsQuestComplete(animal.animalID))
        {
            questTurnedIn = true;
            DialogueUI.Instance.ShowDialogue(
                animal.portrait,
                animal.animalName,
                animal.completionDialogue,
                onClose: GiveItem
            );
            Debug.Log("turning in quest");
        }
    }

    void OnIntroComplete()
    {
        // Unlock animal in journal as soon as you meet them
        SaveSystem.UnlockAnimal(
            GameManager.Instance.saveData,
            animal.animalID
        );

        // Show unlock popup
        JournalUnlockPopup.Instance.Show(
            animal.portrait,
            animal.animalName,
            "Animal added to journal!"
        );
        Debug.Log("opening door");
        UnlockDoor();
    }

    void LockDoor() { if (exitDoor != null) exitDoor.SetActive(true); }
    void UnlockDoor() { if (exitDoor != null) exitDoor.SetActive(false); }

    void GiveItem()
    {
        if (itemPickupPrefab != null && itemSpawnPoint != null)
            Instantiate(itemPickupPrefab, itemSpawnPoint.position, Quaternion.identity);
    }
}