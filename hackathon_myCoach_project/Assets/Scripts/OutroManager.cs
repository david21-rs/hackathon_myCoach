using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;

public class OutroManager : MonoBehaviour
{
    [Header("UI References")]
    public TextMeshProUGUI speakerText;
    public TextMeshProUGUI dialogueText;

    private string[] lines = new string[]
    {
        "You're back! And you're not a frozen popsicle. I mean, of course you aren't, I designed the suit! But, you know, it's nice to have empirical proof.",
        "I've been looking at the data you transmitted. The animals, the poachers, that giant... ice golem thing. Frankly, I didn't program the suit for magic ice giants, so good job not dying.",
        "Look at these biomes now. Because you took out those poaching rings and gathered this data, we actually have a fighting chance. The authorities are moving in, and the animal populations are stabilizing.",
        "But let's be real here. The climate didn't magically fix itself just because you hit a golem with a sword. We're still up against that 1.1-degree wall of global warming.",
        "The damage to the savanna, the shrinking of the tundra... that takes decades to reverse. What you did was buy us time. The most valuable resource on Earth.",
        "So, what happens now? You go rest. Take the suit off, it probably smells terrible anyway. I need to run diagnostics on it.",
        "As for the journal data... I'm publishing it. All of it. The whole world is going to hear the stories of the creatures you met. You were their voice.",
        "You did good, kid. The planet is slightly less doomed than it was yesterday.",
        "Now get out of my lab before I invent a Sentimental-Inator and ruin the moment. GO!"
    };
    private int currentLine = 0;

    void Start()
    {
        speakerText.text = "Professor Doofenshmirtz";
        ShowCurrentLine();
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space) || Input.GetKeyDown(KeyCode.Return))
        {
            NextLine();
        }
    }

    void ShowCurrentLine()
    {
        dialogueText.text = lines[currentLine];
    }

    void NextLine()
    {
        currentLine++;
        if (currentLine >= lines.Length)
        {
            // Wipe the save so your presentation loop is perfectly clean for the next run
            if (GameManager.Instance != null) GameManager.Instance.DeleteSave();
            
            SceneManager.LoadScene("TITLE_SCENE"); 
        }
        else
        {
            ShowCurrentLine();
        }
    }
}