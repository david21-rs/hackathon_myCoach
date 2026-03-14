using UnityEngine;

public class EnemyHitReceiver : MonoBehaviour
{
    private BanditAI banditAI;
    private PoacherAI poacherAI;

    void Start()
    {
        banditAI = GetComponent<BanditAI>();
        poacherAI = GetComponent<PoacherAI>();
    }

    public void TakeDamage(float amount)
    {
        if (banditAI != null)  banditAI.TakeDamage(amount);
        if (poacherAI != null) poacherAI.TakeDamage(amount);
    }
}