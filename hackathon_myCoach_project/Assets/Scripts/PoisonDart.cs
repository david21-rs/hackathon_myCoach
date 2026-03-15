using UnityEngine;
using System.Collections;

public class PoisonDart : MonoBehaviour
{
    private float speed;
    private float damage;
    private float direction;

    public void Launch(int facingDirection, float dartSpeed, float dartDamage)
    {
        direction = facingDirection;
        speed = dartSpeed;
        damage = dartDamage;

        // Flip sprite if going left
        if (direction < 0)
            transform.localScale = new Vector3(-1, 1, 1);

        Destroy(gameObject, 3f);  // auto destroy after 3 seconds
    }

    void Update()
    {
        transform.Translate(Vector2.right * direction * speed * Time.deltaTime);
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        // Hit an enemy
        EnemyHitReceiver receiver = other.GetComponent<EnemyHitReceiver>();
        if (receiver != null)
        {
            receiver.TakeDamage(damage);
            // Trigger poison DoT on the enemy
            PoisonEffect poison = other.GetComponent<PoisonEffect>();
            if (poison != null) poison.ApplyPoison();

            Destroy(gameObject);
        }

        // Hit a wall
        if (other.gameObject.layer == LayerMask.NameToLayer("Ground"))
            Destroy(gameObject);
    }
}