using UnityEngine;

public class AnimalEncounterTrigger : MonoBehaviour
{
    [Header("Drag the .asset file here in the Inspector")]
    public AnimalData animal;

    private bool hasTriggered = false;

    void OnTriggerEnter2D(Collider2D other)
    {
        // Only fire once, only for the player
        if (hasTriggered) return;
        if (!other.CompareTag("Player")) return;

        // Don't show same animal twice in one run
        if (GameManager.Instance.currentRun.HasMetAnimal(animal.animalID))
            return;

        hasTriggered = true;
        GameManager.Instance.currentRun.RecordAnimalMet(animal.animalID);

        // Kick off the encounter
        AnimalEncounterManager.Instance.StartEncounter(animal);
    }
}