using UnityEngine;
using UnityEngine.UI;

public class BanditAI : MonoBehaviour 
{
    [Header("Targeting")]
    private Transform player;

    [Header("Combat Stats")]
    public float chaseRange = 8.0f;
    public float attackRange = 1.5f; 
    public float moveSpeed = 3.0f;
    public float attackCooldown = 1.2f;
    public float damageAmount = 10f;

    [Header("Health")]
    public float maxHealth = 50f;
    private float currentHealth;
    public Image healthFill;

    private Animator m_animator;
    private Rigidbody2D m_body2d;
    private float nextAttackTime;
    private bool isDead = false;

    void Start() 
    {
        m_animator = GetComponent<Animator>();
        m_body2d = GetComponent<Rigidbody2D>();
        currentHealth = maxHealth;
        
        // Initialize UI on spawn
        if (healthFill != null) 
        {
            healthFill.fillAmount = currentHealth / maxHealth;
        }
        
        // Locating the player dynamically.
        GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
        if (playerObj != null) 
        {
            player = playerObj.transform;
        }
        else
        {
            Debug.LogError("BanditAI: Cannot find anything tagged 'Player'. Fix your tags in the Inspector.");
        }
    }

    void Update() 
    {
        if (isDead || player == null) return;

        float distance = Vector2.Distance(transform.position, player.position);

        if (distance <= attackRange) 
        {
            FacePlayer();
            ExecuteCombat();
        } 
        else if (distance <= chaseRange) 
        {
            FacePlayer();
            ExecutePursuit();
        }
        else 
        {
            // If the player is outside the chase range, do nothing.
            ExecuteIdle(); 
        }
    }

    private void ExecuteIdle()
    {
        // Slam the brakes
        m_body2d.linearVelocity = new Vector2(0, m_body2d.linearVelocity.y);
        
        // AnimState 0 is the default Idle animation in your legacy Bandit code
        m_animator.SetInteger("AnimState", 0); 
    }

    private void FacePlayer() 
    {
        float lookDirection = (player.position.x > transform.position.x) ? -1f : 1f;
        transform.localScale = new Vector3(lookDirection, 1.0f, 1.0f);
    }

    private void ExecutePursuit() 
    {
        float moveDir = (player.position.x > transform.position.x) ? 1f : -1f;
        m_body2d.linearVelocity = new Vector2(moveDir * moveSpeed, m_body2d.linearVelocity.y);
        m_animator.SetInteger("AnimState", 2); 
    }

    private void ExecuteCombat() 
    {
        // Stop moving
        m_body2d.linearVelocity = new Vector2(0, m_body2d.linearVelocity.y);
        m_animator.SetInteger("AnimState", 1); 

        // ONLY trigger the visual animation and reset the cooldown here. 
        // Do NOT apply damage yet.
        if (Time.time >= nextAttackTime) 
        {
            m_animator.SetTrigger("Attack");
            nextAttackTime = Time.time + attackCooldown;
        }
    }

    // THIS IS THE FUNCTION YOUR ANIMATION WILL CALL
    public void PerformMeleeStrike()
    {
        // If the Bandit died mid-swing, abort. No phantom hits from corpses.
        if (isDead || player == null) return;

        // Recalculate distance. This allows the player to actually step back and DODGE the swing.
        float distance = Vector2.Distance(transform.position, player.position);
        
        if (distance <= attackRange) 
        {
            HeroKnight playerScript = player.GetComponent<HeroKnight>();
            if (playerScript != null) 
            {
                playerScript.TakeDamage(damageAmount, transform);
            }
        }
    }

    public void TakeDamage(float amount)
    {
        if (isDead) return;

        // 1. Subtract the damage first
        currentHealth -= amount;
        
        // 2. Clamp it so health doesn't drop below 0 and break UI logic
        currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth);
        
        // 3. Update the UI with the accurate math
        if (healthFill != null) 
        {
            healthFill.fillAmount = currentHealth / maxHealth;
        }

        if (currentHealth <= 0)
        {
            Die();
        }
        else
        {
            m_animator.SetTrigger("Hurt");
            nextAttackTime = Time.time + 0.5f; 
        }
    }

    private void Die()
    {
        isDead = true;
        m_animator.SetTrigger("Death");
        m_body2d.linearVelocity = Vector2.zero;
        
        // 1. Grab both physical bodies
        Collider2D banditCollider = GetComponent<Collider2D>();
        Collider2D playerCollider = player.GetComponent<Collider2D>();
        if (banditCollider != null && playerCollider != null)
        {
            Physics2D.IgnoreCollision(banditCollider, playerCollider);
        }

        // Kill the UI Canvas
        if (healthFill != null && healthFill.canvas != null)
        {
            healthFill.canvas.enabled = false;
        }
        // 3. Shut down the AI loop so it stops thinking
        
        gameObject.tag = "Untagged";
        Destroy(gameObject, 5f);
    }
}