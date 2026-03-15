using UnityEngine;

[CreateAssetMenu(fileName = "NewItem", menuName = "THAWFALL/Item Data")]
public class ItemData : ScriptableObject
{
    [Header("Identity")]
    public string itemID;
    public string itemName;

    [Header("Visuals")]
    public Sprite icon;             // small icon shown in HUD
    public Sprite fullArt;          // larger image shown in journal

    [Header("Journal Entry")]
    [TextArea(2, 4)]
    public string subtitle;         // "Ndugu's Gift"
    [TextArea(5, 12)]
    public string journalDescription;

    [Header("Stat Boosts")]
    public float meleeDamageBonus;
    public float movementSpeedBonus;
    public bool grantsPoisonDarts;
}