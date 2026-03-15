using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class JournalUI : MonoBehaviour
{
    public static JournalUI Instance;

    [Header("Panel")]
    public GameObject panel;

    [Header("Tab Buttons")]
    public Button animalsTabButton;
    public Button enemiesTabButton;
    public Button itemsTabButton;

    [Header("Card Grid")]
    public Transform cardContainer;
    public GameObject cardPrefab;

    [Header("Detail View")]
    public GameObject detailPanel;
    public Image detailFullArt;
    public TextMeshProUGUI detailName;
    public TextMeshProUGUI detailSubtitle;
    public TextMeshProUGUI detailBody;
    public TextMeshProUGUI detailExtra; 
    public Button closeDetailButton;

    [Header("All Data — drag .asset files here")]
    public AnimalData[] allAnimals;
    public EnemyData[] allEnemies;
    public ItemData[] allItems;

    private enum Tab { Animals, Enemies, Items }
    private Tab currentTab = Tab.Animals;

    void Awake()
    {
        // Singleton Pattern
        if (Instance == null) Instance = this;
        else Destroy(gameObject);

        panel.SetActive(false);
        detailPanel.SetActive(false);

        animalsTabButton.onClick.AddListener(() => SwitchTab(Tab.Animals));
        enemiesTabButton.onClick.AddListener(() => SwitchTab(Tab.Enemies));
        itemsTabButton.onClick.AddListener(() => SwitchTab(Tab.Items));
        closeDetailButton.onClick.AddListener(CloseDetail);
    }

    public void OpenJournal()
    {
        panel.SetActive(true);
        Time.timeScale = 0f; // Pause game
        SwitchTab(Tab.Animals);
    }

    public void CloseJournal()
    {
        panel.SetActive(false);
        detailPanel.SetActive(false);
        Time.timeScale = 1f; // Unpause
    }

    void SwitchTab(Tab tab)
    {
        currentTab = tab;
        BuildCards();
    }

    void BuildCards()
    {
        // Clear old cards
        foreach (Transform child in cardContainer)
            Destroy(child.gameObject);

        // Make sure GameManager exists before accessing
        if (GameManager.Instance == null) return;
        SaveData save = GameManager.Instance.saveData;

        if (currentTab == Tab.Animals)
        {
            foreach (AnimalData animal in allAnimals)
            {
                bool unlocked = SaveSystem.IsAnimalUnlocked(save, animal.animalID);
                CreateCard(
                    unlocked ? animal.portrait : null,
                    unlocked ? animal.animalName : "???",
                    unlocked,
                    () => ShowAnimalDetail(animal)
                );
            }
        }
        else if (currentTab == Tab.Enemies)
        {
            foreach (EnemyData enemy in allEnemies)
            {
                bool unlocked = SaveSystem.IsEnemyUnlocked(save, enemy.enemyID);
                int kills = save.GetKillCount(enemy.enemyID);
                CreateCard(
                    unlocked ? enemy.portrait : null,
                    unlocked ? enemy.enemyName + "\n×" + kills : "???",
                    unlocked,
                    () => ShowEnemyDetail(enemy)
                );
            }
        }
        else if (currentTab == Tab.Items)
        {
            foreach (ItemData item in allItems)
            {
                bool unlocked = SaveSystem.IsItemUnlocked(save, item.itemID);
                CreateCard(
                    unlocked ? item.icon : null,
                    unlocked ? item.itemName : "???",
                    unlocked,
                    () => ShowItemDetail(item)
                );
            }
        }
    }

    void CreateCard(Sprite art, string label, bool unlocked, System.Action onClickAction)
    {
        GameObject card = Instantiate(cardPrefab, cardContainer);
        Image cardImage = card.GetComponentInChildren<Image>();
        TextMeshProUGUI cardLabel = card.GetComponentInChildren<TextMeshProUGUI>();
        Button cardButton = card.GetComponent<Button>();

        if (unlocked && art != null)
        {
            cardImage.sprite = art;
            cardImage.color = Color.white;
        }
        else
        {
            cardImage.color = Color.black; // Silhouette mode
        }

        cardLabel.text = label;

        if (unlocked)
        {
            cardButton.interactable = true;
            cardButton.onClick.AddListener(() => onClickAction());
        }
        else
        {
            cardButton.interactable = false;
        }
    }

    void ShowAnimalDetail(AnimalData animal)
    {
        detailPanel.SetActive(true);
        detailFullArt.sprite = animal.journalArt != null ? animal.journalArt : animal.portrait;
        detailName.text = animal.animalName;
        detailSubtitle.text = $"{animal.species} | {animal.scientificName}";
        detailBody.text = animal.journalEntry;
        detailExtra.text = "Doof's Note: " + animal.doofNote;
    }

    void ShowEnemyDetail(EnemyData enemy)
    {
        SaveData save = GameManager.Instance.saveData;
        detailPanel.SetActive(true);
        detailFullArt.sprite = enemy.fullArt != null ? enemy.fullArt : enemy.portrait;
        detailName.text = enemy.enemyName;
        detailSubtitle.text = enemy.subtitle;
        detailBody.text = $"{enemy.journalEntry}\n\nAttack: {enemy.attackStyle}\nFound in: {enemy.biome}";
        detailExtra.text = $"Weakness: {enemy.weaknesses}\n\nTotal killed: {save.GetKillCount(enemy.enemyID)}";
    }

    void ShowItemDetail(ItemData item)
    {
        detailPanel.SetActive(true);
        detailFullArt.sprite = item.fullArt != null ? item.fullArt : item.icon;
        detailName.text = item.itemName;
        detailSubtitle.text = item.subtitle;
        detailBody.text = item.journalDescription;
        detailExtra.text = BuildStatString(item);
    }

    string BuildStatString(ItemData item)
    {
        string stats = "EFFECTS:\n";
        if (item.meleeDamageBonus > 0)
            stats += $"+ {item.meleeDamageBonus * 100}% melee damage\n";
        if (item.movementSpeedBonus > 0)
            stats += $"+ {item.movementSpeedBonus * 100}% movement speed\n";
        if (item.grantsPoisonDarts)
            stats += "+ Poison dart attack unlocked\n";
        return stats;
    }

    void CloseDetail()
    {
        detailPanel.SetActive(false);
    }
}