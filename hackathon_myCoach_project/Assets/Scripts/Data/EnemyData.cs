using UnityEngine;

[CreateAssetMenu(fileName = "NewEnemy", menuName = "THAWFALL/Enemy Data")]
public class EnemyData : ScriptableObject
{
    [Header("Identity")]
    public string enemyID;          // "Trapper"
    public string enemyName;        // "The Trapper"
    public Sprite portrait;         // enemy art
    public Sprite fullArt;          // bigger image for journal

    [Header("Journal Entry")]
    [TextArea(2, 4)]
    public string subtitle;         // "Illegal Wildlife Trapper"

    [TextArea(5, 12)]
    public string journalEntry;     // lore description

    [TextArea(2, 4)]
    public string weaknesses;       // "Weak to dash attacks"

    [Header("Stats shown in journal")]
    public float health;
    public string attackStyle;      // "Throws nets that slow the player"
    public string biome;            // "Savanna, Rainforest"
}