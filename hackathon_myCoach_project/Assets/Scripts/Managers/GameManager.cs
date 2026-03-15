using UnityEngine;
using UnityEngine.SceneManagement;
using System.IO;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    public SaveData saveData;
    private string saveFilePath;

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject); 

        saveFilePath = Application.persistentDataPath + "/savefile.json";
        LoadGame();
    }

    public void SaveGame()
    {
        string json = JsonUtility.ToJson(saveData, true);
        File.WriteAllText(saveFilePath, json);
        Debug.Log("Saved to: " + saveFilePath);
    }

    public void LoadGame()
    {
        if (File.Exists(saveFilePath))
        {
            string json = File.ReadAllText(saveFilePath);
            saveData = JsonUtility.FromJson<SaveData>(json);
        }
        else
        {
            saveData = new SaveData(); 
        }
    }

    public void PlayerDied()
    {
        SaveGame(); 
        SceneManager.LoadScene("TITLE_SCENE");
    }
    
    // Call this from a UI button to clear the save for your presentation demo
    public void DeleteSave()
    {
        if (File.Exists(saveFilePath)) File.Delete(saveFilePath);
        saveData = new SaveData();
    }
}