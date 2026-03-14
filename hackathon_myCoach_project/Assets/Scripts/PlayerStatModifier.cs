using UnityEngine;
using System.Reflection;

public class PlayerStatModifier : MonoBehaviour
{
    // Accumulated bonuses
    private float totalDamageMultiplier = 1f;
    private float totalSpeedBonus = 0f;
    public bool hasPoisonDarts = false;

    // Cache the HeroKnight reference
    private HeroKnight heroKnight;

    // Cache the base speed so we always calculate from the original
    private float baseSpeed = -1f;

    void Start()
    {
        heroKnight = GetComponent<HeroKnight>();

        // Read base speed via reflection so we never lose the original value
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
        // Damage multiplier stacks additively
        // e.g. 1.0 + 0.10 = 1.10 = 10% more damage
        totalDamageMultiplier += item.meleeDamageBonus;

        // Speed bonus stacks additively
        totalSpeedBonus += item.movementSpeedBonus;

        if (item.grantsPoisonDarts)
            hasPoisonDarts = true;

        // Apply the new speed to HeroKnight via reflection
        ApplySpeedToHeroKnight();

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
            // New speed = base speed + (base speed * bonus)
            // e.g. base 4.0 + (4.0 * 0.15) = 4.6
            float newSpeed = baseSpeed + (baseSpeed * totalSpeedBonus);
            speedField.SetValue(heroKnight, newSpeed);
            Debug.Log("Speed set to: " + newSpeed);
        }
    }

    public float GetDamageMultiplier() => totalDamageMultiplier;
}