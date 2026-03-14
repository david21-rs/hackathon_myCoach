using UnityEngine;

public class PoacherActions : MonoBehaviour 
{
    private Animator anim;
    private Rigidbody2D rb;
    private Vector3 startingScale; 

    [Header("Shooting Setup")]
    public GameObject bulletPrefab; // The bullet file you created
    public Transform firePoint;     // The empty barrel object
    public float bulletSpeed = 15f; 

    void Awake() {
        anim = GetComponent<Animator>();
        rb = GetComponent<Rigidbody2D>();
        startingScale = transform.localScale; 
    }

    public void Move(float velocity) {
        rb.linearVelocity = new Vector2(velocity, rb.linearVelocity.y);
        anim.SetFloat("Speed", Mathf.Abs(velocity));
        
        if (velocity != 0) {
            FaceDirection(Mathf.Sign(velocity));
        }
    }

    public void FaceDirection(float directionSign) {
        transform.localScale = new Vector3(
            Mathf.Abs(startingScale.x) * directionSign, 
            startingScale.y, 
            startingScale.z
        );
    }

    public void ToggleCrouch(bool state) {
        anim.SetBool("isCrouching", state);
    }

    public void Shoot() {
        // 1. Play the visual illusion
        anim.SetTrigger("Attack");

        // 2. Spawn the physical bullet at the exact position of the barrel
        GameObject spawnedBullet = Instantiate(bulletPrefab, firePoint.position, Quaternion.identity);

        // 3. Figure out which way the Poacher is currently facing
        float facingDirection = Mathf.Sign(transform.localScale.x);

        // 4. Shove the bullet in that direction
        Rigidbody2D bulletRb = spawnedBullet.GetComponent<Rigidbody2D>();
        if (bulletRb != null) {
            bulletRb.linearVelocity = new Vector2(facingDirection * bulletSpeed, 0);
        }

        // 5. Destroy the bullet after 2 seconds so your game doesn't crash from memory overload
        Destroy(spawnedBullet, 2f);
    }
}