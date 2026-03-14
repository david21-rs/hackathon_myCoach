using System.Collections.Generic;

[System.Serializable]
public class SaveData
{
    // Animals met
    public List<string> unlockedAnimalIDs = new List<string>();

    // Enemies killed (tracks unique enemy TYPES, not every single kill)
    public List<string> unlockedEnemyIDs = new List<string>();

    // Items collected
    public List<string> unlockedItemIDs = new List<string>();

    // Raw kill counts per enemy type for display in journal
    public List<string> enemyKillCountKeys = new List<string>();
    public List<int> enemyKillCountValues = new List<int>();

    // Stats
    public int totalRunsCompleted = 0;
    public int totalPoachersDefeated = 0;
    public int totalAnimalsDocumented = 0;

    // Helper to get kill count (Dictionary not serializable so we use two lists)
    public int GetKillCount(string enemyID)
    {
        int index = enemyKillCountKeys.IndexOf(enemyID);
        if (index == -1) return 0;
        return enemyKillCountValues[index];
    }

    public void AddKill(string enemyID)
    {
        int index = enemyKillCountKeys.IndexOf(enemyID);
        if (index == -1)
        {
            enemyKillCountKeys.Add(enemyID);
            enemyKillCountValues.Add(1);
        }
        else
        {
            enemyKillCountValues[index]++;
        }
    }
}