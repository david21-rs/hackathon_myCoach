using UnityEngine;
using UnityEngine.UI;
using System;           // <-- ADD THIS LINE

public class EnemyHealth : MonoBehaviour
{
    [SerializeField] private float maxHealth = 50f;
    private float currentHealth;

    [Header("UI")]
    [SerializeField] private Image healthFill;

    // ADD THIS: other scripts subscribe to get notified on death
    public event Action onDeath;

    void Start()
    {
        currentHealth = maxHealth;
        UpdateUI();
    }

    public void TakeDamage(float amount)
    {
        currentHealth -= amount;
        currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth);
        UpdateUI();

        if (currentHealth <= 0)
            Die();
    }

    private void UpdateUI()
    {
        if (healthFill != null)
            healthFill.fillAmount = currentHealth / maxHealth;
    }

    private void Die()
    {
        onDeath?.Invoke();    // Notify the room
        gameObject.SetActive(false);
    }
}