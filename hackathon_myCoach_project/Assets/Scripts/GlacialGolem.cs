using System.Collections;
using UnityEngine;

public class GlacialGolem : MonoBehaviour
{
    public enum BossState { Chasing, Attacking, Hurt, Dead }
    public BossState currentState = BossState.Chasing;

    [Header("Stats")]
    [SerializeField] private float maxHealth = 600f;
    [SerializeField] private float moveSpeed = 2.5f;
    private float currentHealth;
    private bool isPhaseTwo = false;

    [Header("UI")]
    [SerializeField] private HealthBar bossHealthBar;

    [Header("Melee Attack")]
    [SerializeField] private float attackRange = 2f;
    [SerializeField] private float attackDamage = 20f;
    [SerializeField] private float attackCooldown = 1.5f;

    [Header("Ice Slam (AOE)")]
    [SerializeField] private float slamRange = 3f;
    [SerializeField] private float slamDamage = 35f;
    [SerializeField] private float slamCooldown = 5f;
    [SerializeField] private GameObject slamVFXPrefab;
    private float slamTimer = 0f;

    [Header("Ice Spear (Projectile)")]
    [SerializeField] private float projectileRange = 8f;
    [SerializeField] private float projectileCooldown = 4f;
    [SerializeField] private GameObject iceSpearPrefab;
    [SerializeField] private Transform projectileSpawnPoint;
    [SerializeField] private float projectileSpeed = 7f;
    private float projectileTimer = 0f;

    [Header("Frost Charge (Phase 2)")]
    [SerializeField] private float chargeSpeed = 9f;
    [SerializeField] private float chargeDamage = 40f;
    [SerializeField] private float chargeRange = 7f;
    [SerializeField] private float chargeCooldown = 7f;
    private float chargeTimer = 0f;

    private Transform player;
    private Animator animator;
    private SpriteRenderer spriteRenderer;
    private Vector3 originalScale;

    void Start()
    {
        currentHealth = maxHealth;
        animator = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        originalScale = transform.localScale;

        if (bossHealthBar != null) bossHealthBar.UpdateHealth(currentHealth, maxHealth);

        GameObject p = GameObject.FindGameObjectWithTag("Player");
        if (p != null) player = p.transform;
    }

    void Update()
    {
        if (currentState == BossState.Dead || player == null) return;

        slamTimer += Time.deltaTime;
        projectileTimer += Time.deltaTime;
        chargeTimer += Time.deltaTime;

        switch (currentState)
        {
            case BossState.Chasing:
                HandleChasing();
                break;
            case BossState.Attacking:
                break;
        }
    }

    // ─────────────────────────────────────────────
    //  CHASING + ATTACK SELECTION
    // ─────────────────────────────────────────────
    private void HandleChasing()
    {
        float dist = Vector2.Distance(transform.position, player.position);

        animator.SetBool("isWalking", true);
        spriteRenderer.flipX = player.position.x < transform.position.x;
        transform.position = Vector2.MoveTowards(
            transform.position,
            new Vector2(player.position.x, transform.position.y),
            moveSpeed * Time.deltaTime);

        if (isPhaseTwo && dist >= chargeRange && chargeTimer >= chargeCooldown)
            StartCoroutine(ChargeAttack());
        else if (dist <= slamRange && slamTimer >= slamCooldown)
            StartCoroutine(IceSlamAttack());
        else if (dist <= projectileRange && dist > attackRange && projectileTimer >= projectileCooldown)
            StartCoroutine(IceSpearAttack());
        else if (dist <= attackRange)
            StartCoroutine(BasicMeleeAttack());
    }

    // ─────────────────────────────────────────────
    //  ATTACK 1 — BASIC MELEE
    //  Sprite: quick forward lunge then snap back
    // ─────────────────────────────────────────────
    private IEnumerator BasicMeleeAttack()
    {
        currentState = BossState.Attacking;
        animator.SetBool("isWalking", false);
        animator.SetTrigger("attack");

        Debug.Log("performing melee");


        // Lunge toward player
        Vector3 startPos = transform.position;
        Vector3 lungePos = transform.position + (Vector3)(((Vector2)player.position - (Vector2)transform.position).normalized * 0.6f);
        yield return StartCoroutine(MoveToPosition(startPos, lungePos, 0.15f));

        if (Vector2.Distance(transform.position, player.position) <= attackRange + 0.6f)
            player.GetComponent<HeroKnight>().TakeDamage(attackDamage, this.transform);

        // Snap back
        yield return StartCoroutine(MoveToPosition(transform.position, startPos, 0.2f));

        yield return new WaitForSeconds(attackCooldown);
        if (currentState != BossState.Dead) currentState = BossState.Chasing;
    }

    // ─────────────────────────────────────────────
    //  ATTACK 2 — ICE SLAM
    //  Sprite: stretch tall (wind-up), squash flat (slam)
    // ─────────────────────────────────────────────
    private IEnumerator IceSlamAttack()
    {
        currentState = BossState.Attacking;
        slamTimer = 0f;
        animator.SetBool("isWalking", false);
        Debug.Log("performing slam");
        //animator.SetTrigger("attack"); // Reuse attack anim

        // Wind-up: stretch tall
        yield return StartCoroutine(ScaleTo(new Vector3(0.8f, 1.3f, 1f), 0.25f));

        // Slam: squash wide and flat
        yield return StartCoroutine(ScaleTo(new Vector3(1.5f, 0.6f, 1f), 0.1f));

        // Damage + VFX at squash peak
        if (slamVFXPrefab != null)
            Instantiate(slamVFXPrefab, transform.position, Quaternion.identity);

        if (Vector2.Distance(transform.position, player.position) <= slamRange)
            player.GetComponent<HeroKnight>().TakeDamage(slamDamage, this.transform);

        // Spring back to normal
        yield return StartCoroutine(ScaleTo(originalScale, 0.2f));

        yield return new WaitForSeconds(0.5f);
        if (currentState != BossState.Dead) currentState = BossState.Chasing;
    }

    // ─────────────────────────────────────────────
    //  ATTACK 3 — ICE SPEAR
    //  Sprite: double cyan flash as telegraph, then fires
    // ─────────────────────────────────────────────
    private IEnumerator IceSpearAttack()
    {
        currentState = BossState.Attacking;
        projectileTimer = 0f;
        animator.SetBool("isWalking", false);
        animator.SetTrigger("attack"); // Reuse attack anim
        Debug.Log("performing spear");

        // Telegraph: pulse cyan twice so the player knows a projectile is coming
        yield return StartCoroutine(FlashColor(Color.cyan, 0.15f));
        yield return StartCoroutine(FlashColor(Color.white, 0.1f));
        yield return StartCoroutine(FlashColor(Color.cyan, 0.15f));

        // Fire spear
        if (iceSpearPrefab != null && projectileSpawnPoint != null)
        {
            Vector2 dir = ((Vector2)player.position - (Vector2)projectileSpawnPoint.position).normalized;
            GameObject spear = Instantiate(iceSpearPrefab, projectileSpawnPoint.position, Quaternion.identity);

            float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
            spear.transform.rotation = Quaternion.Euler(0f, 0f, angle);

            Rigidbody2D rb = spear.GetComponent<Rigidbody2D>();
            if (rb != null) rb.linearVelocity = dir * projectileSpeed;

            IceSpear spearScript = spear.GetComponent<IceSpear>();
            if (spearScript != null) spearScript.damage = attackDamage * 1.2f;
        }

        yield return new WaitForSeconds(0.8f);
        if (currentState != BossState.Dead) currentState = BossState.Chasing;
    }

    // ─────────────────────────────────────────────
    //  ATTACK 4 — FROST CHARGE (Phase 2 only)
    //  Sprite: red flashes as warning, then dashes across screen
    // ─────────────────────────────────────────────
    private IEnumerator ChargeAttack()
    {
        currentState = BossState.Attacking;
        chargeTimer = 0f;
        animator.SetBool("isWalking", false);
        Debug.Log("performing charge");

        // Telegraph: rapid red flashes so the player can react
        yield return StartCoroutine(FlashColor(Color.red, 0.15f));
        yield return StartCoroutine(FlashColor(Color.white, 0.08f));
        yield return StartCoroutine(FlashColor(Color.red, 0.15f));
        yield return StartCoroutine(FlashColor(Color.white, 0.08f));
        yield return StartCoroutine(FlashColor(Color.red, 0.15f));
        yield return new WaitForSeconds(0.1f);

        // Lock in direction at the moment the charge starts
        Vector2 chargeDir = ((Vector2)player.position - (Vector2)transform.position).normalized;
        spriteRenderer.flipX = chargeDir.x < 0;
        animator.SetBool("isWalking", true);

        float elapsed = 0f;
        float chargeTime = 0.55f;
        bool hitPlayer = false;

        while (elapsed < chargeTime)
        {
            transform.position += (Vector3)(chargeDir * chargeSpeed * Time.deltaTime);

            if (!hitPlayer && Vector2.Distance(transform.position, player.position) <= attackRange)
            {
                player.GetComponent<HeroKnight>().TakeDamage(chargeDamage, this.transform);
                hitPlayer = true;
                StartCoroutine(ScaleTo(new Vector3(1.4f, 0.7f, 1f), 0.05f));
            }

            elapsed += Time.deltaTime;
            yield return null;
        }

        animator.SetBool("isWalking", false);
        spriteRenderer.color = Color.white;
        yield return StartCoroutine(ScaleTo(originalScale, 0.15f));

        yield return new WaitForSeconds(0.4f);
        if (currentState != BossState.Dead) currentState = BossState.Chasing;
    }

    // ─────────────────────────────────────────────
    //  DAMAGE + PHASE 2 + DEATH
    // ─────────────────────────────────────────────
    public void TakeDamage(float damage)
    {
        if (currentState == BossState.Dead) return;

        currentHealth -= damage;
        if (bossHealthBar != null) bossHealthBar.UpdateHealth(currentHealth, maxHealth);

        if (!isPhaseTwo && currentHealth <= maxHealth * 0.5f)
        {
            isPhaseTwo = true;
            moveSpeed *= 1.3f;
            attackCooldown *= 0.75f;
            StartCoroutine(Phase2Flash());
            Debug.Log("Glacial Golem — Phase 2!");
        }

        if (currentHealth <= 0)
            Die();
        else if (currentState != BossState.Attacking)
            StartCoroutine(HurtRoutine());
    }

    private IEnumerator HurtRoutine()
    {
        currentState = BossState.Hurt;
        animator.SetTrigger("hurt");
        yield return StartCoroutine(FlashColor(new Color(1f, 0.4f, 0.4f), 0.12f));
        spriteRenderer.color = Color.white;
        yield return new WaitForSeconds(0.2f);
        if (currentState != BossState.Dead) currentState = BossState.Chasing;
    }

    private void Die()
    {
        currentState = BossState.Dead;
        StopAllCoroutines();
        transform.localScale = originalScale;
        spriteRenderer.color = Color.white;
        animator.SetTrigger("death");

        if (bossHealthBar != null) bossHealthBar.gameObject.SetActive(false);

        GetComponent<Collider2D>().enabled = false;
        Destroy(gameObject, 3f);
    }

    // ─────────────────────────────────────────────
    //  SPRITE HELPERS
    // ─────────────────────────────────────────────

    // Smoothly move from a to b over duration seconds
    private IEnumerator MoveToPosition(Vector3 from, Vector3 to, float duration)
    {
        float t = 0f;
        while (t < duration)
        {
            transform.position = Vector3.Lerp(from, to, t / duration);
            t += Time.deltaTime;
            yield return null;
        }
        transform.position = to;
    }

    // Smoothly scale to target over duration seconds
    private IEnumerator ScaleTo(Vector3 target, float duration)
    {
        Vector3 start = transform.localScale;
        float t = 0f;
        while (t < duration)
        {
            transform.localScale = Vector3.Lerp(start, target, t / duration);
            t += Time.deltaTime;
            yield return null;
        }
        transform.localScale = target;
    }

    // Flash sprite to a color then back to white
    private IEnumerator FlashColor(Color color, float duration)
    {
        spriteRenderer.color = color;
        yield return new WaitForSeconds(duration);
        spriteRenderer.color = Color.white;
    }

    // Rapid cyan flashing for the phase 2 transition
    private IEnumerator Phase2Flash()
    {
        for (int i = 0; i < 6; i++)
        {
            spriteRenderer.color = Color.cyan;
            yield return new WaitForSeconds(0.1f);
            spriteRenderer.color = Color.white;
            yield return new WaitForSeconds(0.1f);
        }
    }
}