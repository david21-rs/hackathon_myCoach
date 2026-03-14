using UnityEngine;

public class PoacherActions : MonoBehaviour 
{
    private Animator anim;
    private Rigidbody2D rb;
    private Vector3 startingScale; // Store the scale you set in the editor

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
        // Multiply the intended direction by the absolute value of the original X scale.
        // This prevents the character from shrinking if their scale isn't exactly 1.
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
        // Removed the redundant cooldown logic here. 
        // PoacherAI already dictates when the shot happens.
        anim.SetTrigger("Attack");
    }
}