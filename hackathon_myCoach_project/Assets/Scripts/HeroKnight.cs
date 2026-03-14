using UnityEngine;
using System.Collections;

public class HeroKnight : MonoBehaviour {

    [SerializeField] float      m_speed = 4.0f;
    [SerializeField] float      m_jumpForce = 7.5f;
    [SerializeField] GameObject m_slideDust;

    [Header("Sensors (Assign in Inspector)")]
    [SerializeField] private float sensorRadius = 0.1f;

    [Header("Dash & I-Frames")]
    [SerializeField] private float dashForce = 15f;
    [SerializeField] private float dashDuration = 0.2f;
    [SerializeField] private float dashCooldown = 1f;
    private bool canDash = true;
    private int originalLayer;

    [Header("Jump Tuning")]
    [SerializeField] private float coyoteTime = 0.15f;

    [Header("Health & UI")]
    [SerializeField] private float maxHealth = 100f;
    private float currentHealth;
    [SerializeField] private HealthBar healthBar; // Drag the Canvas here in the Inspector

    [Header("Combat Hitbox")]
    [SerializeField] private GameObject attackHitbox; // Drag your AttackHitbox here


    private float coyoteTimeCounter;

    private Transform groundSensor, wallSensorR1, wallSensorR2, wallSensorL1, wallSensorL2;
    private LayerMask groundLayer;
    
    private Animator            m_animator;
    private Rigidbody2D         m_body2d;
    private bool                m_isWallSliding = false;
    private bool                m_grounded = false;
    private bool                m_rolling = false;
    private int                 m_facingDirection = 1;
    private float               m_timeSinceAttack = 0.0f;

    void Start()
    {
        m_animator = GetComponent<Animator>();
        m_body2d = GetComponent<Rigidbody2D>();
        originalLayer = gameObject.layer;

        // The strings inside Find() MUST match your child object names perfectly.
        groundSensor = transform.Find("GroundSensor");
        wallSensorR1 = transform.Find("WallSensor_R1");
        wallSensorR2 = transform.Find("WallSensor_R2");
        wallSensorL1 = transform.Find("WallSensor_L1");
        wallSensorL2 = transform.Find("WallSensor_L2");

        // Assumes your actual physics layer is named exactly "Ground"
        groundLayer = LayerMask.GetMask("Ground");

        currentHealth = maxHealth;
        if (healthBar != null) healthBar.UpdateHealth(currentHealth, maxHealth);
    }

        void Update()
    {
        m_timeSinceAttack += Time.deltaTime;

        UpdateGroundState();

        float inputX = Input.GetAxis("Horizontal");
        HandleMovement(inputX);
        UpdateAnimator(inputX);

        // --- Action State Machine (Priority Ordered) ---
        if (Input.GetKeyDown("e") && !m_rolling) TriggerDeath();
        else if (Input.GetKeyDown("q") && !m_rolling) TakeDamage(10);
        else if (Input.GetMouseButtonDown(0) && m_timeSinceAttack > 0.25f && !m_rolling) PerformAttack();
        else if (Input.GetMouseButtonDown(1) && !m_rolling) StartBlock();
        else if (Input.GetMouseButtonUp(1)) EndBlock();
        else if (Input.GetKeyDown(KeyCode.LeftShift) && canDash && !m_isWallSliding) PerformRoll();
        else if (Input.GetKeyDown(KeyCode.Space) && coyoteTimeCounter > 0f && !m_rolling) PerformJump();

    }

    // ==========================================
    // HELPER FUNCTIONS
    // ==========================================

    private void UpdateGroundState()
    {
        bool newGrounded = IsGrounded();
        //if (newGrounded != m_grounded) Debug.Log($"[ANIM CHANGE] Grounded: {newGrounded}");
        
        m_grounded = newGrounded;
        m_animator.SetBool("Grounded", m_grounded);

        if (m_grounded) coyoteTimeCounter = coyoteTime;
        else coyoteTimeCounter -= Time.deltaTime;
    }

    private void HandleMovement(float inputX)
    {
        if (inputX > 0) 
        { 
            GetComponent<SpriteRenderer>().flipX = false; 
            m_facingDirection = 1; 
            // Face Hitbox Right
            attackHitbox.transform.localRotation = Quaternion.Euler(0, 0, 0);
        }
        else if (inputX < 0) 
        { 
            GetComponent<SpriteRenderer>().flipX = true; 
            m_facingDirection = -1; 
            // Face Hitbox Left
            attackHitbox.transform.localRotation = Quaternion.Euler(0, 180, 0);
        }

        if (!m_rolling) m_body2d.linearVelocity = new Vector2(inputX * m_speed, m_body2d.linearVelocity.y);
    }

    private void UpdateAnimator(float inputX)
    {
        bool newWallSlide = ((IsWallRight() && inputX > 0) || (IsWallLeft() && inputX < 0)) && !m_grounded;
        //if (newWallSlide != m_isWallSliding) Debug.Log($"[ANIM CHANGE] WallSlide: {newWallSlide}");
        m_isWallSliding = newWallSlide;
        m_animator.SetBool("WallSlide", m_isWallSliding);

        m_animator.SetFloat("speedY", m_body2d.linearVelocity.y); 

        bool isMoving = Mathf.Abs(inputX) > 0.1f;
        bool animIsMoving = isMoving && !m_isWallSliding;
        //if (m_animator.GetBool("isMoving") != animIsMoving) Debug.Log($"[ANIM CHANGE] isMoving: {animIsMoving}");
        m_animator.SetBool("isMoving", animIsMoving);
    }

    private void PerformAttack()
    {
        m_timeSinceAttack = 0.0f; // Minor debounce to prevent multi-frame firing
        //Debug.Log("[ANIM TRIGGER] Attack");
        m_animator.SetTrigger("Attack");
    }

    private void StartBlock()
    {
        //Debug.Log("[ANIM TRIGGER] Block");
        m_animator.SetTrigger("Block");
        //Debug.Log("[ANIM CHANGE] IdleBlock: True");
        m_animator.SetBool("IdleBlock", true);
    }

    private void EndBlock()
    {
        //Debug.Log("[ANIM CHANGE] IdleBlock: False");
        m_animator.SetBool("IdleBlock", false);
    }

    private void PerformRoll()
    {
        StartCoroutine(PerformDash());
    }

    private void PerformJump()
    {
        //Debug.Log("[ANIM TRIGGER] Jump");
        m_animator.SetTrigger("Jump");
        
        m_grounded = false;
        //Debug.Log("[ANIM CHANGE] Grounded: False");
        m_animator.SetBool("Grounded", false);
        
        m_body2d.linearVelocity = new Vector2(m_body2d.linearVelocity.x, m_jumpForce);
        coyoteTimeCounter = 0f;
    }

    private void TriggerDeath()
    {
        //Debug.Log("[ANIM TRIGGER] Death");
        m_animator.SetTrigger("Death");
    }

    private void TriggerHurt()
    {
        //Debug.Log("[ANIM TRIGGER] Hurt");
        m_animator.SetTrigger("Hurt");
    }


    private IEnumerator PerformDash()
    {
        canDash = false;
        m_rolling = true; 
        //Debug.Log("[ANIM TRIGGER] Roll");
        m_animator.SetTrigger("Roll");

        gameObject.layer = LayerMask.NameToLayer("Invincible");
        m_body2d.linearVelocity = new Vector2(m_facingDirection * dashForce, 0f);

        yield return new WaitForSeconds(dashDuration);

        gameObject.layer = originalLayer;
        m_rolling = false;

        yield return new WaitForSeconds(dashCooldown);
        canDash = true;
    }

    public void TakeDamage(float damageAmount)
    {
        currentHealth -= damageAmount;
        currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth); // Prevents negative health

        if (healthBar != null) healthBar.UpdateHealth(currentHealth, maxHealth);

        if (currentHealth <= 0) TriggerDeath();
        else TriggerHurt();
    }

    private bool IsGrounded() => Physics2D.OverlapCircle(groundSensor.position, sensorRadius, groundLayer);
    private bool IsWallRight() => Physics2D.OverlapCircle(wallSensorR1.position, sensorRadius, groundLayer) && Physics2D.OverlapCircle(wallSensorR2.position, sensorRadius, groundLayer);
    private bool IsWallLeft() => Physics2D.OverlapCircle(wallSensorL1.position, sensorRadius, groundLayer) && Physics2D.OverlapCircle(wallSensorL2.position, sensorRadius, groundLayer);
    public void EnableHitbox() => attackHitbox.SetActive(true);
    public void DisableHitbox() => attackHitbox.SetActive(false);

    void AE_SlideDust()
    {
        Vector3 spawnPosition = m_facingDirection == 1 ? wallSensorR2.position : wallSensorL2.position;

        if (m_slideDust != null)
        {
            GameObject dust = Instantiate(m_slideDust, spawnPosition, gameObject.transform.localRotation) as GameObject;
            dust.transform.localScale = new Vector3(m_facingDirection, 1, 1);
        }
    }
}