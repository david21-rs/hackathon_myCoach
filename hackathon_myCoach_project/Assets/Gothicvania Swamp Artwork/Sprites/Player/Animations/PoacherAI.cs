using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;


public class PoacherAI : MonoBehaviour 
{
    public Transform player;
    public Transform pointA; 
    public Transform pointB; 
    
    public float attackRange = 5f;
    public float moveSpeed = 10f;
    public float fireRate = 2f; 
    
    private Transform currentTarget;
    private PoacherActions actions;
    private float nextShotTime;

    [SerializeField] private float maxHealth = 50f;
    private float currentHealth;
    
    [Header("UI")]
    [SerializeField] private Image healthFill; // Drag EnemyHealthFill here

    void Start() {
        actions = GetComponent<PoacherActions>();
        currentTarget = pointA; 

        if (player == null) {
            GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
            if (playerObj != null) player = playerObj.transform;
        }

        currentHealth = maxHealth;
        UpdateUI();
    }

    void Update() {
        // Stop execution if you failed to assign references in the Inspector. 
        // Check your Unity Console. If you see these warnings, fix your setup.
        if (currentTarget == null) {
            Debug.LogWarning("PoacherAI: pointA or pointB is not assigned!");
            return; 
        }
        if (player == null) {
            Debug.LogWarning("PoacherAI: Player reference is missing or untagged!");
            return;
        }

        float distanceToPlayer = Vector3.Distance(transform.position, player.position);

        if (distanceToPlayer <= attackRange) {
            HandleCombat();
        } else {
            HandlePatrol();
        }
    }

    void HandlePatrol() {
        actions.ToggleCrouch(false); 
        
        float direction = (currentTarget.position.x > transform.position.x) ? 1 : -1;
        actions.Move(direction * moveSpeed);
        Debug.Log("moving: " + direction);
        if (Mathf.Abs(transform.position.x - currentTarget.position.x) < 0.5f) {
            currentTarget = (currentTarget == pointA) ? pointB : pointA;
            Debug.Log("im in range! changing target to: " + currentTarget.GameObject().name);
        }
    }

    void HandleCombat() {
        actions.Move(0); 

        // Let the Actions script handle the flipping to keep scale logic in one place
        float directionToPlayer = (player.position.x > transform.position.x) ? 1 : -1;
        actions.FaceDirection(directionToPlayer);
        if (Time.time >= nextShotTime) {
            actions.Shoot();
            nextShotTime = Time.time + fireRate;
            actions.ToggleCrouch(Random.value > 0.5f);
        }
    }

    private void UpdateUI()
    {
        if (healthFill != null) 
            healthFill.fillAmount = currentHealth / maxHealth;
    }

    public void TakeDamage(float amount)
    {
        Debug.Log("enemy taking damage: " + amount);
        currentHealth -= amount;
        currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth);
        
        UpdateUI();

        if (currentHealth <= 0) 
        {
            Die();
        }
    }

    private void Die()
    {
        Debug.Log("Enemy Killed");
        gameObject.SetActive(false); // Temporary death
    }
}