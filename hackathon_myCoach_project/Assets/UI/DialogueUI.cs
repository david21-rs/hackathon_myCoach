using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;

public class DialogueUI : MonoBehaviour
{
    public static DialogueUI Instance;

    [Header("Wire these in the Inspector")]
    public GameObject panel;
    public Image portraitImage;
    public TextMeshProUGUI dialogueText;
    public TextMeshProUGUI speakerName;

    private Action onCloseCallback;

    void Awake()
    {
        Instance = this;
        panel.SetActive(false);
    }

    public void ShowDialogue(Sprite portrait, string text, Action onClose = null)
    {
        portraitImage.sprite = portrait;
        dialogueText.text = text;
        onCloseCallback = onClose;
        panel.SetActive(true);
        Time.timeScale = 0f;           // pause game
    }

    void Update()
    {
        if (!panel.activeSelf) return;

        if (Input.GetKeyDown(KeyCode.Space) || Input.GetKeyDown(KeyCode.Return))
            CloseDialogue();
    }

    public void CloseDialogue()
    {
        panel.SetActive(false);
        Time.timeScale = 1f;           // unpause game

        // Fire the callback (e.g. open quiz after intro closes)
        onCloseCallback?.Invoke();
        onCloseCallback = null;
    }
}