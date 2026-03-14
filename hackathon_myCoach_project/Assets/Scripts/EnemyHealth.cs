using UnityEngine;
using UnityEngine.UI;

public class EnemyHealth : MonoBehaviour
{
    [SerializeField] private float maxHealth = 50f;
    private float currentHealth;
    
    [Header("UI")]
    [SerializeField] private Image healthFill; // Drag EnemyHealthFill here

    void Start()
    {
        currentHealth = maxHealth;
        UpdateUI();
    }

    public void TakeDamage(float amount)
    {
        Debug.Log("taking damage: " + amount);
        currentHealth -= amount;
        currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth);
        
        UpdateUI();

        if (currentHealth <= 0) 
        {
            Die();
        }
    }

    private void UpdateUI()
    {
        if (healthFill != null) 
            healthFill.fillAmount = currentHealth / maxHealth;
    }

    private void Die()
    {
        Debug.Log("Enemy Killed");
        gameObject.SetActive(false); // Temporary death
    }
}