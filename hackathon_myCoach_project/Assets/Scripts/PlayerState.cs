using UnityEngine;

public class PlayerStats : MonoBehaviour
{
    public static PlayerStats Instance;

    [Header("Base Stats")]
    public float baseMeleeDamage = 10f;
    public float baseMovementSpeed = 5f;
    public bool hasPoisonDarts = false;

    // Current stats after items applied
    [HideInInspector] public float currentMeleeDamage;
    [HideInInspector] public float currentMovementSpeed;

    void Awake()
    {
        Instance = this;
        // Start with base stats
        currentMeleeDamage = baseMeleeDamage;
        currentMovementSpeed = baseMovementSpeed;
    }

    public void ApplyItem(ItemData item)
    {
        // Apply percentage bonuses
        currentMeleeDamage += baseMeleeDamage * item.meleeDamageBonus;
        currentMovementSpeed += baseMovementSpeed * item.movementSpeedBonus;

        if (item.grantsPoisonDarts)
            hasPoisonDarts = true;

        Debug.Log("Item applied: " + item.itemName);
        Debug.Log("Melee damage now: " + currentMeleeDamage);
        Debug.Log("Speed now: " + currentMovementSpeed);
    }
}