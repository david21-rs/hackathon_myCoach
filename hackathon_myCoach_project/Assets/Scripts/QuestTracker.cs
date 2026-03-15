using UnityEngine;
using System.Collections.Generic;

public class QuestTracker : MonoBehaviour
{
    public static QuestTracker Instance;

    // Tracks kill count per quest
    // key = animalID (e.g. "Ndugu"), value = kills so far
    private Dictionary<string, int> killCounts = new Dictionary<string, int>();
    private Dictionary<string, int> killTargets = new Dictionary<string, int>();
    private Dictionary<string, bool> questComplete = new Dictionary<string, bool>();

    void Awake()
    {
        Instance = this;
    }

    // Call this when setting up a quest room
    public void RegisterQuest(string animalID, int killsRequired)
    {
        killCounts[animalID] = 0;
        killTargets[animalID] = killsRequired;
        questComplete[animalID] = false;
        Debug.Log("Quest registered for: " + animalID);
    }

    // Call this when a poacher dies
    public void ReportKill(string animalID)
    {
        Debug.Log("kill reported for: " + animalID);
        if (!killCounts.ContainsKey(animalID)) return;
        if (questComplete[animalID]) return;

        killCounts[animalID]++;
        Debug.Log(animalID + " quest: " 
                           + killCounts[animalID] + "/" + killTargets[animalID]);

        if (killCounts[animalID] >= killTargets[animalID])
        {
            questComplete[animalID] = true;
            Debug.Log("Quest complete for: " + animalID);
        }
    }

    public bool IsQuestComplete(string animalID)
    {
        if (!questComplete.ContainsKey(animalID)) return false;
        return questComplete[animalID];
    }
}