using UnityEngine;

public class PlayerCombat : MonoBehaviour
{
    [SerializeField] private float attackDamage = 15f;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        Debug.Log("yes");
        // Your Enemy Prefab MUST have the Tag "Enemy" assigned in the Inspector.
        if (collision.CompareTag("Enemy"))
        {
            EnemyHealth enemy = collision.GetComponent<EnemyHealth>();
            if (enemy != null){
                
                enemy.TakeDamage(attackDamage);
            }
        }
    }
}