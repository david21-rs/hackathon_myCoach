using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class JournalUI : MonoBehaviour
{
    public static JournalUI Instance;

    [Header("Wire these in the Inspector")]
    public GameObject panel;
    public Transform cardContainer;        // the ScrollRect content panel
    public GameObject journalCardPrefab;   // your card prefab

    [Header("Drag ALL animal .asset files here")]
    public AnimalData[] allAnimals;        // drag Ndugu, Pixel, Bjorn, Fern here

    // Detail view
    public GameObject detailPanel;
    public Image detailPortrait;
    public TextMeshProUGUI detailName;
    public TextMeshProUGUI detailEntry;

    void Awake()
    {
        Instance = this;
        panel.SetActive(false);
        detailPanel.SetActive(false);
    }

    public void OpenJournal()
    {
        panel.SetActive(true);
        Time.timeScale = 0f;
        BuildCards();
    }

    public void CloseJournal()
    {
        panel.SetActive(false);
        detailPanel.SetActive(false);
        Time.timeScale = 1f;
    }

    void BuildCards()
    {
        // Clear existing cards
        foreach (Transform child in cardContainer)
            Destroy(child.gameObject);

        SaveData save = GameManager.Instance.saveData;

        foreach (AnimalData animal in allAnimals)
        {
            bool unlocked = SaveSystem.IsAnimalUnlocked(save, animal.animalID);

            GameObject card = Instantiate(journalCardPrefab, cardContainer);

            Image cardImage = card.GetComponentInChildren<Image>();
            TextMeshProUGUI cardName = card.GetComponentInChildren<TextMeshProUGUI>();
            Button cardButton = card.GetComponent<Button>();

            if (unlocked)
            {
                cardImage.sprite = animal.portrait;
                cardImage.color = Color.white;         // full colour
                cardName.text = animal.animalName;
            }
            else
            {
                cardImage.sprite = animal.portrait;
                cardImage.color = Color.black;         // silhouette
                cardName.text = "???";
            }

            // Only open detail if unlocked
            if (unlocked)
            {
                AnimalData captured = animal;
                cardButton.onClick.AddListener(() => ShowDetail(captured));
            }
        }
    }

    void ShowDetail(AnimalData animal)
    {
        detailPanel.SetActive(true);
        detailPortrait.sprite = animal.portrait;
        detailName.text = animal.animalName + " — " + animal.species;
        detailEntry.text = animal.journalEntry
            + "\n\n<i>Doof's Note: " + animal.doofNote + "</i>";
    }
}