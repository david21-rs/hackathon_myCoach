using UnityEngine;

public class PlayerCombat : MonoBehaviour
{
    [SerializeField] private float attackDamage = 15f;

    private void OnTriggerEnter2D(Collider2D collision)
    {   
        if (collision.CompareTag("Enemy"))
        {
            // 1. Check if it is a Poacher
            PoacherAI poacher = collision.GetComponent<PoacherAI>();
            if (poacher != null)
            {
                poacher.TakeDamage(attackDamage);
                return; // Stop checking, we hit our target
            }
            
            // 2. Check if it is a Bandit
            BanditAI bandit = collision.GetComponent<BanditAI>();
            if (bandit != null)
            {
                bandit.TakeDamage(attackDamage);
                return;
            }

            GlacialGolem golem = collision.GetComponent<GlacialGolem>();
            if (golem != null)
            {
                golem.TakeDamage(attackDamage);
                return;
            }

            // 3. When you add a Slime, you will have to come back and write this again.
            // 4. When you add a Boss, you will have to write it again.
        }
    }
}