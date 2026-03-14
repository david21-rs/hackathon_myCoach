using System.Collections.Generic;

public class RunState
{
    // Progress
    public int currentBiomeIndex = 0;
    public int roomsCompletedInBiome = 0;

    // Animals met THIS run (so you don't show same animal twice in one run)
    public List<string> animalsMetThisRun = new List<string>();

    // Quests active or done THIS run
    public Dictionary<string, QuestStatus> questStates
        = new Dictionary<string, QuestStatus>();

    // Quiz scores THIS run
    public Dictionary<string, int> quizScores
        = new Dictionary<string, int>();

    // Items equipped THIS run
    public List<string> equippedItemIDs = new List<string>();

    // Player stats THIS run
    public int maxHealth = 100;
    public int currentHealth = 100;
    public int poachersDefeatedThisRun = 0;
    public int animalsDocumentedThisRun = 0;

    // Helper
    public bool HasMetAnimal(string animalID)
    {
        return animalsMetThisRun.Contains(animalID);
    }

    public void RecordAnimalMet(string animalID)
    {
        if (!animalsMetThisRun.Contains(animalID))
            animalsMetThisRun.Add(animalID);
    }
}

public enum QuestStatus { NotStarted, InProgress, Complete, Failed }