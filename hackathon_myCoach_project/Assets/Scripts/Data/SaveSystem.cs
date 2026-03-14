using UnityEngine;
using System.IO;

public static class SaveSystem
{
    private static string SavePath =>
        Application.persistentDataPath + "/thawfall_save.json";

    public static void Save(SaveData data)
    {
        string json = JsonUtility.ToJson(data, prettyPrint: true);
        File.WriteAllText(SavePath, json);
        Debug.Log("Game saved to: " + SavePath);
    }

    public static SaveData Load()
    {
        if (!File.Exists(SavePath))
        {
            Debug.Log("No save found. Starting fresh.");
            return new SaveData();
        }

        string json = File.ReadAllText(SavePath);
        return JsonUtility.FromJson<SaveData>(json);
    }

    public static void UnlockAnimal(SaveData save, string animalID)
    {
        if (!save.unlockedAnimalIDs.Contains(animalID))
        {
            save.unlockedAnimalIDs.Add(animalID);
            Save(save);
            Debug.Log("Unlocked animal: " + animalID);
        }
    }

    public static void CompleteQuest(SaveData save, string questID)
    {
        if (!save.completedQuestIDs.Contains(questID))
        {
            save.completedQuestIDs.Add(questID);
            Save(save);
        }
    }

    public static bool IsAnimalUnlocked(SaveData save, string animalID)
    {
        return save.unlockedAnimalIDs.Contains(animalID);
    }
}