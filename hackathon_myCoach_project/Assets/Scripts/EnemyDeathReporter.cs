using UnityEngine;

public class EnemyDeathReporter : MonoBehaviour
{
    [Header("Wire in Inspector")]
    public EnemyData enemyData;       // drag Trapper.asset or RifleHunter.asset
    public string linkedAnimalID;     // "Ndugu", "Bjorn", "Zara" — which quest

    private bool isApplicationQuitting = false;

    // Unity calls this when the app closes too, not just when enemy dies
    // This flag stops false positives on app quit
    void OnApplicationQuit()
    {
        isApplicationQuitting = true;
    }

    void OnDestroy()
    {
        Debug.Log("kill pre reported for: " + linkedAnimalID);
        // Don't fire on scene unload or app quit
        if (isApplicationQuitting) return;

        // Don't fire if GameManager doesn't exist yet
        if (GameManager.Instance == null) return;
        
        SaveData save = GameManager.Instance.saveData;

        // 1 — Tell the quest tracker one poacher died
        if (!string.IsNullOrEmpty(linkedAnimalID))
        {
            QuestTracker.Instance.ReportKill(linkedAnimalID);
                // not going here ????
        }
        

        // 2 — Unlock enemy in journal + increment kill count
        if (enemyData != null)
        {
            bool firstTimeSeeing = !SaveSystem.IsEnemyUnlocked(save, enemyData.enemyID);

            SaveSystem.RegisterEnemyKill(save, enemyData.enemyID);

            // 3 — Show unlock popup only on first kill of this type
            if (firstTimeSeeing)
            {
                JournalUnlockPopup.Instance.Show(
                    enemyData.portrait,
                    enemyData.enemyName,
                    "New journal entry unlocked!"
                );
            }
        }
    }
}