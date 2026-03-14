using UnityEngine;

public class GameManager : MonoBehaviour
{
    // Singleton — access from anywhere with GameManager.Instance
    public static GameManager Instance;

    [HideInInspector] public SaveData saveData;
    [HideInInspector] public RunState currentRun;

    void Awake()
    {
        // Singleton setup
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);  // survives scene changes

        // Load persistent save
        saveData = SaveSystem.Load();

        // Start a fresh run
        currentRun = new RunState();
    }

    public void StartNewRun()
    {
        currentRun = new RunState();
        Debug.Log("New run started.");
    }

    public void EndRun(bool completed)
    {
        // Write run stats into permanent save
        saveData.totalRunsCompleted++;
        saveData.totalPoachersDefeated += currentRun.poachersDefeatedThisRun;
        saveData.totalAnimalsDocumented += currentRun.animalsDocumentedThisRun;

        if (completed)
        {
            // award completion achievement etc.
        }

        SaveSystem.Save(saveData);
    }
}