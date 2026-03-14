using UnityEngine;

[CreateAssetMenu(fileName = "NewAnimal", menuName = "THAWFALL/Animal Data")]
public class AnimalData : ScriptableObject
{
    [Header("Identity")]
    public string animalID;            // "Ndugu" - used as unique key
    public string animalName;          // "Ndugu"
    public string species;             // "African Bush Elephant"
    public string scientificName;      // "Loxodonta africana"
    public ConservationStatus status;
    public Biome biome;

    [Header("Visuals")]
    public Sprite portrait;
    public Sprite journalArt;

    [Header("Dialogue")]
    [TextArea(3, 8)]
    public string introDialogue;

    [TextArea(3, 8)]
    public string questDialogue;

    [TextArea(3, 8)]
    public string completionDialogue;

    [Header("Journal Entry")]
    [TextArea(5, 15)]
    public string journalEntry;

    public string conservationFact;
    public string doofNote;

    [Header("Quest")]
    public QuestType questType;
    public QuizData linkedQuiz;        // drag OceanQuiz.asset here for Pixel
    public string rewardItemID;        // "ReforestationSeed"
}

// These enums live here too — same file, no extra file needed
public enum ConservationStatus { Vulnerable, Endangered, Critical, Extinct }
public enum Biome { Savanna, Rainforest, Ocean, Tundra }
public enum QuestType { Combat, Quiz, Exploration, Hybrid }