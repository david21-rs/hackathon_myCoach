using UnityEngine;
using System.Collections;

public class PoisonEffect : MonoBehaviour
{
    public float poisonDamagePerTick = 5f;
    public float tickInterval = 1f;
    public int totalTicks = 4;            // 4 ticks = 4 seconds of poison

    private EnemyHitReceiver receiver;
    private bool isPoisoned = false;

    void Start()
    {
        receiver = GetComponent<EnemyHitReceiver>();
    }

    public void ApplyPoison()
    {
        if (isPoisoned) return;  // don't stack
        StartCoroutine(PoisonRoutine());
    }

    IEnumerator PoisonRoutine()
    {
        isPoisoned = true;
        int ticks = 0;

        while (ticks < totalTicks)
        {
            yield return new WaitForSeconds(tickInterval);
            if (receiver != null)
                receiver.TakeDamage(poisonDamagePerTick);
            ticks++;
        }

        isPoisoned = false;
    }
}