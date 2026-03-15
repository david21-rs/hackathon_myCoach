using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;

public class IntroManager : MonoBehaviour
{
    [Header("Wire these in the Inspector")]
    public TextMeshProUGUI speakerText;
    public TextMeshProUGUI dialogueText;
    public Image background;
    public Sprite labBackground;
    public Sprite mapBackground;
    
    [Header("Characters")]
    public GameObject scientistSprite; // <-- NEW: The slot for our scientist!

    private string[] lines = new string[]
    {
        "Ah! Oh good, you're here. Or... you will be. Look, the point is: I need your help. And I don't say that lightly, because I am a brilliant scientist with THREE doctorates — two of which are from fully accredited institutions (we don't talk about it) and no I DID NOT use AI to graduate unlike kids nowadays — and I STILL cannot fix this alone.",
        "The planet is in crisis. I know, I know — you've heard this before, BOOOORING, am I right? WELL SHUT UP AND PAY ATTENTION. THIS time, it's personal. Because somewhere out in those burning savannas, those dying rainforests, those melting tundras... there are poachers. Illegal hunters. People who see a magnificent creature on the edge of extinction and think: 'business opportunity'.",
        "SWITCH_TO_MAP",   
        "Here is what's happening. Earth's average temperature has risen about 1.1 degrees Celsius since we started burning fossil fuels in the Industrial Revolution. Now, 1.1 degrees might sound like nothing. BUT. For the planet? Every fraction of a degree matters enormously.",
        "Warmer air means more evaporation. More evaporation means droughts in dry places and mega-storms in wet ones. Ice caps melt, sea levels rise, ocean chemistry changes, seasons shift... and animals that have evolved over millions of years for very specific conditions suddenly find their homes unrecognizable.",
        "Some of them are already gone. We call those extinct. Others are on the edge — we call those endangered or critically endangered. And THEN there are the poachers, who've decided that a climate-stressed animal is an even EASIER target. Ivory. Exotic pets. Traditional medicine markets. The illegal wildlife trade is worth 23 billion dollars a year. BILLION. With a B. You hear me choom?",
        "So I built... THE CLIMATE GUARDIAN SUIT-INATOR. Patent pending. It gives the wearer the ability to travel through climate-collapsed ecosystems, fight those poachers hand-to-hand, AND — this is the important part — document the animals they find. Because data, my friend, is also a weapon. Knowledge is how we change things.",
        "SWITCH_TO_LAB",   
        "Your mission: travel through three ecosystems, from the scorching Savanna all the way to the frozen Tundra. Stop the poaching operations you find. And most importantly — talk to the animals. Yes, the suit does that too. I may have over-engineered it slightly. Also have this weapon and shield: Use the sword to get ahead, and keep the shield just in de-fence.",
        "Every creature you meet, you'll record in here. Their stories. Their struggles. Some of them will have tasks for you — quests, if you want to be dramatic about it, which I personally do. They need your help. And you might learn something too, which I understand is less exciting, but statistically very useful.",
        "Some of the animals you'll meet... you might be the last human they ever talk to. That's not a joke. Treat them like it matters. Because it does.",
        "Now get out there. I'll be monitoring from here, offering real-time scientific commentary via earpiece. You're welcome. And... good luck. The planet is counting on you. No pressure. Well — a little pressure. Actually quite a lot of pressure. AN ABSURD AMOUNT OF PRESSURE. GO."
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
        string line = lines[currentLine];
        
        if (line == "SWITCH_TO_MAP")
        {
            background.sprite = mapBackground;
            scientistSprite.SetActive(false); // <-- NEW: Turns him off!
            currentLine++;
            ShowCurrentLine();   
            return;
        }
        if (line == "SWITCH_TO_LAB")
        {
            background.sprite = labBackground;
            scientistSprite.SetActive(true);  // <-- NEW: Turns him back on!
            currentLine++;
            ShowCurrentLine();
            return;
        }
        
        dialogueText.text = line;
    }

    void NextLine()
    {
        currentLine++;
        if (currentLine >= lines.Length)
        {
            SceneManager.LoadScene("TitleScene"); 
            return;
        }
        ShowCurrentLine();
    }
}