using UnityEngine;
using System.Collections;

public class HeroKnight : MonoBehaviour {

    [SerializeField] float      m_speed = 4.0f;
    [SerializeField] float      m_jumpForce = 7.5f;
    [SerializeField] GameObject m_slideDust;

    [Header("Sensors (Assign in Inspector)")]
    [SerializeField] private Transform groundSensor;
    [SerializeField] private Transform wallSensorR1;
    [SerializeField] private Transform wallSensorR2;
    [SerializeField] private Transform wallSensorL1;
    [SerializeField] private Transform wallSensorL2;
    [SerializeField] private float sensorRadius = 0.1f;
    [SerializeField] private LayerMask groundLayer;

    [Header("Dash & I-Frames")]
    [SerializeField] private float dashForce = 15f;
    [SerializeField] private float dashDuration = 0.2f;
    [SerializeField] private float dashCooldown = 1f;
    private bool canDash = true;
    private int originalLayer;

    [Header("Jump Tuning")]
    [SerializeField] private float coyoteTime = 0.15f;
    private float coyoteTimeCounter;

    private Animator            m_animator;
    private Rigidbody2D         m_body2d;
    private bool                m_isWallSliding = false;
    private bool                m_grounded = false;
    private bool                m_rolling = false;
    private int                 m_facingDirection = 1;
    private int                 m_currentAttack = 0;
    private float               m_timeSinceAttack = 0.0f;
    private float               m_delayToIdle = 0.0f;

    void Start ()
    {
        m_animator = GetComponent<Animator>();
        m_body2d = GetComponent<Rigidbody2D>();
        originalLayer = gameObject.layer;
    }

    void Update()
{
    m_timeSinceAttack += Time.deltaTime;

    // --- Grounded ---
    bool newGrounded = IsGrounded();
    if (newGrounded != m_grounded) Debug.Log($"[ANIM CHANGE] Grounded: {newGrounded}");
    m_grounded = newGrounded;
    m_animator.SetBool("Grounded", m_grounded);

    if (m_grounded) coyoteTimeCounter = coyoteTime;
    else coyoteTimeCounter -= Time.deltaTime;

    // --- Input ---
    float inputX = Input.GetAxis("Horizontal");
    bool isMoving = Mathf.Abs(inputX) > 0.1f; // Fixed deadzone

    if (inputX > 0) { GetComponent<SpriteRenderer>().flipX = false; m_facingDirection = 1; }
    else if (inputX < 0) { GetComponent<SpriteRenderer>().flipX = true; m_facingDirection = -1; }

    if (!m_rolling) m_body2d.linearVelocity = new Vector2(inputX * m_speed, m_body2d.linearVelocity.y);

    // --- Wall Slide ---
    bool newWallSlide = ((IsWallRight() && inputX > 0) || (IsWallLeft() && inputX < 0)) && !m_grounded;
    if (newWallSlide != m_isWallSliding) Debug.Log($"[ANIM CHANGE] WallSlide: {newWallSlide}");
    m_isWallSliding = newWallSlide;
    m_animator.SetBool("WallSlide", m_isWallSliding);

    // --- Speed & Moving ---
    m_animator.SetFloat("speedY", m_body2d.linearVelocity.y); 

    bool animIsMoving = isMoving && !m_isWallSliding;
    if (m_animator.GetBool("isMoving") != animIsMoving) Debug.Log($"[ANIM CHANGE] isMoving: {animIsMoving}");
    m_animator.SetBool("isMoving", animIsMoving);

    // --- Triggers ---
    if (Input.GetKeyDown("e") && !m_rolling) 
    {
        Debug.Log("[ANIM TRIGGER] Death");
        m_animator.SetTrigger("Death");
    }
    else if (Input.GetKeyDown("q") && !m_rolling) 
    {
        Debug.Log("[ANIM TRIGGER] Hurt");
        m_animator.SetTrigger("Hurt");
    }
    else if (Input.GetMouseButtonDown(0) && m_timeSinceAttack > 0.25f && !m_rolling)
    {
        m_timeSinceAttack = 0.0f;
        m_currentAttack++;
        if (m_currentAttack > 3) m_currentAttack = 1;

        Debug.Log($"[ANIM CHANGE] ComboStep: {m_currentAttack}");
        m_animator.SetInteger("ComboStep", m_currentAttack);
        Debug.Log("[ANIM TRIGGER] Attack");
        m_animator.SetTrigger("Attack");
    }
    else if (Input.GetMouseButtonDown(1) && !m_rolling)
    {
        Debug.Log("[ANIM TRIGGER] Block");
        m_animator.SetTrigger("Block");
        Debug.Log("[ANIM CHANGE] IdleBlock: True");
        m_animator.SetBool("IdleBlock", true);
    }
    else if (Input.GetMouseButtonUp(1))
    {
        Debug.Log("[ANIM CHANGE] IdleBlock: False");
        m_animator.SetBool("IdleBlock", false);
    }
    else if (Input.GetKeyDown(KeyCode.LeftShift) && canDash && !m_isWallSliding)
    {
        StartCoroutine(PerformDash());
    }
    else if (Input.GetKeyDown(KeyCode.Space) && coyoteTimeCounter > 0f && !m_rolling)
    {
        Debug.Log("[ANIM TRIGGER] Jump");
        m_animator.SetTrigger("Jump");
        m_grounded = false;
        Debug.Log("[ANIM CHANGE] Grounded: False");
        m_animator.SetBool("Grounded", false);
        m_body2d.linearVelocity = new Vector2(m_body2d.linearVelocity.x, m_jumpForce);
        coyoteTimeCounter = 0f;
    }

    // --- Combo Reset ---
    if (m_timeSinceAttack > 1.0f && m_currentAttack != 0)
    {
        m_currentAttack = 0;
        Debug.Log("[ANIM CHANGE] ComboStep: 0");
        m_animator.SetInteger("ComboStep", 0);
    }
}

private IEnumerator PerformDash()
{
    canDash = false;
    m_rolling = true; 
    Debug.Log("[ANIM TRIGGER] Roll");
    m_animator.SetTrigger("Roll");

    gameObject.layer = LayerMask.NameToLayer("Ignore Raycast");
    m_body2d.linearVelocity = new Vector2(m_facingDirection * dashForce, 0f);

    yield return new WaitForSeconds(dashDuration);

    gameObject.layer = originalLayer;
    m_rolling = false;

    yield return new WaitForSeconds(dashCooldown);
    canDash = true;
}

    private bool IsGrounded() => Physics2D.OverlapCircle(groundSensor.position, sensorRadius, groundLayer);
    private bool IsWallRight() => Physics2D.OverlapCircle(wallSensorR1.position, sensorRadius, groundLayer) && Physics2D.OverlapCircle(wallSensorR2.position, sensorRadius, groundLayer);
    private bool IsWallLeft() => Physics2D.OverlapCircle(wallSensorL1.position, sensorRadius, groundLayer) && Physics2D.OverlapCircle(wallSensorL2.position, sensorRadius, groundLayer);

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