using System.Collections.Generic;

[System.Serializable]
public class SaveData
{
    // Which animals have been fully documented (journal unlocked)
    public List<string> unlockedAnimalIDs = new List<string>();

    // Which quests have been completed across all runs
    public List<string> completedQuestIDs = new List<string>();

    // Achievements
    public List<string> unlockedAchievements = new List<string>();

    // Stats shown on end screen
    public int totalRunsCompleted = 0;
    public int totalPoachersDefeated = 0;
    public int totalAnimalsDocumented = 0;
}