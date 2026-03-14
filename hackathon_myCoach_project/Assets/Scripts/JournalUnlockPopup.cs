using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;

public class JournalUnlockPopup : MonoBehaviour
{
    public static JournalUnlockPopup Instance;

    [Header("Wire in Inspector")]
    public GameObject panel;
    public Image portrait;
    public TextMeshProUGUI nameText;
    public TextMeshProUGUI messageText;

    void Awake()
    {
        Instance = this;
        panel.SetActive(false);
    }

    public void Show(Sprite icon, string entryName, string message)
    {
        portrait.sprite = icon;
        nameText.text = entryName;
        messageText.text = message;
        panel.SetActive(true);
        StartCoroutine(AutoHide());
    }

    IEnumerator AutoHide()
    {
        yield return new WaitForSeconds(3f);
        panel.SetActive(false);
    }
}