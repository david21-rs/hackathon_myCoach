using UnityEngine;

public class AnimalEncounterManager : MonoBehaviour
{
    public static AnimalEncounterManager Instance;

    private AnimalData currentAnimal;

    void Awake()
    {
        Instance = this;
    }

    // Called by AnimalEncounterTrigger
    public void StartEncounter(AnimalData animal)
    {
        currentAnimal = animal;

        // Show intro dialogue first
        DialogueUI.Instance.ShowDialogue(
            animal.portrait,
            animal.introDialogue,
            onClose: OnIntroClosed   // called when player presses Space
        );
    }

    // Called automatically when intro dialogue closes
    private void OnIntroClosed()
    {
        if (currentAnimal.questType == QuestType.Quiz
            && currentAnimal.linkedQuiz != null)
        {
            // Start the quiz
            QuizUI.Instance.StartQuiz(
                currentAnimal.linkedQuiz,
                onPass: OnQuestComplete,
                onFail: OnQuizFailed
            );
        }
        else
        {
            // Show quest briefing, then let the player go fight
            DialogueUI.Instance.ShowDialogue(
                currentAnimal.portrait,
                currentAnimal.questDialogue,
                onClose: null
            );
            // Quest completion is triggered separately when combat clears
        }
    }

    // Call this from your combat/quest system when the quest is done
    public void OnQuestComplete()
    {
        // Show completion dialogue
        DialogueUI.Instance.ShowDialogue(
            currentAnimal.portrait,
            currentAnimal.completionDialogue,
            onClose: OnCompletionClosed
        );
    }

    private void OnCompletionClosed()
    {
        // Permanently unlock the journal entry
        SaveSystem.UnlockAnimal(
            GameManager.Instance.saveData,
            currentAnimal.animalID
        );

        GameManager.Instance.currentRun.animalsDocumentedThisRun++;

        // Show a little notification
        Debug.Log("Journal entry unlocked: " + currentAnimal.animalName);
    }

    private void OnQuizFailed()
    {
        // Let them try again
        DialogueUI.Instance.ShowDialogue(
            currentAnimal.portrait,
            "That's not right — but knowledge is fixable. Try again!",
            onClose: () => QuizUI.Instance.StartQuiz(
                currentAnimal.linkedQuiz,
                onPass: OnQuestComplete,
                onFail: OnQuizFailed
            )
        );
    }
}