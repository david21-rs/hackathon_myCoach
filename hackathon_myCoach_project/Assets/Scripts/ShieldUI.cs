using UnityEngine;
using UnityEngine.UI;

public class ShieldUI : MonoBehaviour
{
    [SerializeField] private HeroKnight player;
    
    [Header("UI Colors")]
    [SerializeField] private Color readyColor = Color.green;
    [SerializeField] private Color activeColor = Color.yellow;
    [SerializeField] private Color chargingColor = Color.red;
    
    private Image cooldownImage;

    void Start()
    {
        cooldownImage = GetComponent<Image>();
    }

    void Update()
    {
        if (player == null) return;

        float fill = player.GetShieldFillAmount();
        cooldownImage.fillAmount = fill;

        if (player.isBlocking)
        {
            cooldownImage.color = activeColor;
        }
        else if (fill >= 1f)
        {
            cooldownImage.color = readyColor;
        }
        else
        {
            cooldownImage.color = chargingColor;
        }
    }
}