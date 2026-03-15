using UnityEngine;

public class ItemPickup : MonoBehaviour
{
    public ItemData item;

    void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Player")) return;

        // Apply stat bonuses via PlayerStatModifier
        PlayerStatModifier stats = other.GetComponent<PlayerStatModifier>();
        if (stats != null) stats.ApplyItem(item);

        // Save to journal
        SaveSystem.UnlockItem(GameManager.Instance.saveData, item.itemID);

        // Show item journal popup
        ItemJournalUI.Instance.ShowItemJournal(item);

        Destroy(gameObject);
    }
}