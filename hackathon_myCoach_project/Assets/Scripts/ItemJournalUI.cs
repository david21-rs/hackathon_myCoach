using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ItemJournalUI : MonoBehaviour
{
    public static ItemJournalUI Instance;

    [Header("Wire in Inspector")]
    public GameObject panel;
    public Image itemIcon;
    public TextMeshProUGUI itemNameText;
    public TextMeshProUGUI itemDescriptionText;

    void Awake()
    {
        Instance = this;
        panel.SetActive(false);
    }

    public void ShowItemJournal(ItemData item)
    {
        itemIcon.sprite = item.icon;
        itemNameText.text = item.itemName;
        itemDescriptionText.text = item.journalDescription;
        panel.SetActive(true);
        Time.timeScale = 0f;
    }

    void Update()
    {
        if (panel.activeSelf && Input.GetKeyDown(KeyCode.Space))
            ClosePanel();
    }

    void ClosePanel()
    {
        panel.SetActive(false);
        Time.timeScale = 1f;
    }
}