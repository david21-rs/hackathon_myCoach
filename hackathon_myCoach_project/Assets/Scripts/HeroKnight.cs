using UnityEngine;
using System.Collections;

public class HeroKnight : MonoBehaviour {

    [SerializeField] float      m_speed = 4.0f;
    [SerializeField] float      m_jumpForce = 7.5f;
    [SerializeField] bool       m_noBlood = false;
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

    void Update ()
    {
        Debug.Log("is_grounded:" + m_grounded + " wall_left:" + IsWallLeft()+ " wall_right:" + IsWallRight());
        m_timeSinceAttack += Time.deltaTime;

        bool touchingRight = IsWallRight();
        bool touchingLeft = IsWallLeft();

        bool wasGrounded = m_grounded;
        m_grounded = IsGrounded();

        if (!wasGrounded && m_grounded)
            m_animator.SetBool("Grounded", true);
        
        if (wasGrounded && !m_grounded)
            m_animator.SetBool("Grounded", false);

        if (m_grounded)
            coyoteTimeCounter = coyoteTime;
        else
            coyoteTimeCounter -= Time.deltaTime;

        float inputX = Input.GetAxis("Horizontal");

        if (inputX > 0)
        {
            GetComponent<SpriteRenderer>().flipX = false;
            m_facingDirection = 1;
        }
        else if (inputX < 0)
        {
            GetComponent<SpriteRenderer>().flipX = true;
            m_facingDirection = -1;
        }

        if (!m_rolling)
            m_body2d.linearVelocity = new Vector2(inputX * m_speed, m_body2d.linearVelocity.y);

        m_animator.SetFloat("AirSpeedY", m_body2d.linearVelocity.y);

        m_isWallSliding = (touchingRight || touchingLeft) && !m_grounded;
        m_animator.SetBool("WallSlide", m_isWallSliding);

        // Instantly break the slide if pulling away
        if (touchingRight && inputX < 0) m_isWallSliding = false;
        if (touchingLeft && inputX > 0) m_isWallSliding = false;

        if (Input.GetKeyDown("e") && !m_rolling)
        {
            m_animator.SetBool("noBlood", m_noBlood);
            m_animator.SetTrigger("Death");
        }
        else if (Input.GetKeyDown("q") && !m_rolling)
            m_animator.SetTrigger("Hurt");
        else if(Input.GetMouseButtonDown(0) && m_timeSinceAttack > 0.25f && !m_rolling)
        {
            m_currentAttack++;
            if (m_currentAttack > 3) m_currentAttack = 1;
            if (m_timeSinceAttack > 1.0f) m_currentAttack = 1;

            m_animator.SetTrigger("Attack" + m_currentAttack);
            m_timeSinceAttack = 0.0f;
        }
        else if (Input.GetMouseButtonDown(1) && !m_rolling)
        {
            m_animator.SetTrigger("Block");
            m_animator.SetBool("IdleBlock", true);
        }
        else if (Input.GetMouseButtonUp(1))
            m_animator.SetBool("IdleBlock", false);

        else if (Input.GetKeyDown(KeyCode.LeftShift) && canDash && !m_isWallSliding)
        {
            StartCoroutine(PerformDash());
        }

        else if (Input.GetKeyDown(KeyCode.Space) && coyoteTimeCounter > 0f && !m_rolling)
        {
            m_animator.SetTrigger("Jump");
            m_grounded = false;
            m_animator.SetBool("Grounded", false);
            m_body2d.linearVelocity = new Vector2(m_body2d.linearVelocity.x, m_jumpForce);
            coyoteTimeCounter = 0f;
        }

        else if (Mathf.Abs(inputX) > Mathf.Epsilon)
        {
            m_delayToIdle = 0.05f;
            m_animator.SetInteger("AnimState", 1);
        }
        else
        {
            m_delayToIdle -= Time.deltaTime;
            if(m_delayToIdle < 0)
                m_animator.SetInteger("AnimState", 0);
        }
    }

    private IEnumerator PerformDash()
    {
        canDash = false;
        m_rolling = true; 
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