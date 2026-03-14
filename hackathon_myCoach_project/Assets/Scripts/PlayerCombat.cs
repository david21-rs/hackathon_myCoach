using UnityEngine;

public class PlayerCombat : MonoBehaviour
{
    [SerializeField] private float attackDamage = 15f;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        // Your Enemy Prefab MUST have the Tag "Enemy" assigned in the Inspector.
        if (collision.CompareTag("Enemy"))
        {
            PoacherAI enemy = collision.GetComponent<PoacherAI>();
            if (enemy != null){
                
                enemy.TakeDamage(attackDamage);
            }
        }
    }
}