using UnityEngine;
using System.Reflection;

public class PlayerStatModifier : MonoBehaviour
{
    private float totalDamageMultiplier = 1f;
    private float totalSpeedBonus = 0f;
    public bool hasPoisonDarts = false;

    private HeroKnight heroKnight;
    private PlayerCombat playerCombat;
    private float baseSpeed = -1f;
    private float baseDamage = 15f;  // must match the default in PlayerCombat

    void Start()
    {
        heroKnight = GetComponent<HeroKnight>();
        playerCombat = GetComponent<PlayerCombat>();

        // Read base speed from HeroKnight via reflection
        if (heroKnight != null)
        {
            FieldInfo speedField = typeof(HeroKnight).GetField(
                "m_speed",
                BindingFlags.NonPublic | BindingFlags.Instance
            );
            if (speedField != null)
                baseSpeed = (float)speedField.GetValue(heroKnight);
        }
    }

    public void ApplyItem(ItemData item)
    {
        totalDamageMultiplier += item.meleeDamageBonus;
        totalSpeedBonus += item.movementSpeedBonus;

        if (item.grantsPoisonDarts)
            hasPoisonDarts = true;

        ApplySpeedToHeroKnight();
        ApplyDamageToPlayerCombat();

        Debug.Log("Item applied: " + item.itemName);
        Debug.Log("Damage multiplier: " + totalDamageMultiplier);
        Debug.Log("Speed bonus: " + totalSpeedBonus);
    }

    private void ApplySpeedToHeroKnight()
    {
        if (heroKnight == null || baseSpeed < 0) return;

        FieldInfo speedField = typeof(HeroKnight).GetField(
            "m_speed",
            BindingFlags.NonPublic | BindingFlags.Instance
        );

        if (speedField != null)
        {
            float newSpeed = baseSpeed + (baseSpeed * totalSpeedBonus);
            speedField.SetValue(heroKnight, newSpeed);
            Debug.Log("Speed set to: " + newSpeed);
        }
    }

    private void ApplyDamageToPlayerCombat()
    {
        if (playerCombat == null) return;

        FieldInfo damageField = typeof(PlayerCombat).GetField(
            "attackDamage",
            BindingFlags.NonPublic | BindingFlags.Instance
        );

        if (damageField != null)
        {
            float newDamage = baseDamage * totalDamageMultiplier;
            damageField.SetValue(playerCombat, newDamage);
            Debug.Log("Attack damage set to: " + newDamage);
        }
    }

    public float GetDamageMultiplier() => totalDamageMultiplier;
}