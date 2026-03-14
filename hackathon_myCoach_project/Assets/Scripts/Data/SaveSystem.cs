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
    }

    public static SaveData Load()
    {
        if (!File.Exists(SavePath))
            return new SaveData();
        string json = File.ReadAllText(SavePath);
        return JsonUtility.FromJson<SaveData>(json);
    }

    // ANIMALS
    public static void UnlockAnimal(SaveData save, string id)
    {
        if (!save.unlockedAnimalIDs.Contains(id))
        {
            save.unlockedAnimalIDs.Add(id);
            Save(save);
        }
    }

    public static bool IsAnimalUnlocked(SaveData save, string id) =>
        save.unlockedAnimalIDs.Contains(id);

    // ENEMIES
    public static void RegisterEnemyKill(SaveData save, string id)
    {
        // Add to journal if first time seeing this enemy type
        if (!save.unlockedEnemyIDs.Contains(id))
            save.unlockedEnemyIDs.Add(id);

        // Increment kill count
        save.AddKill(id);
        Save(save);
    }

    public static bool IsEnemyUnlocked(SaveData save, string id) =>
        save.unlockedEnemyIDs.Contains(id);

    // ITEMS
    public static void UnlockItem(SaveData save, string id)
    {
        if (!save.unlockedItemIDs.Contains(id))
        {
            save.unlockedItemIDs.Add(id);
            Save(save);
        }
    }

    public static bool IsItemUnlocked(SaveData save, string id) =>
        save.unlockedItemIDs.Contains(id);
}