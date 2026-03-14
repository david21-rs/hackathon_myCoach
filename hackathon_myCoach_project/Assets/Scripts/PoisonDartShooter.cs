using UnityEngine;
using System.Reflection; // <--- ADD THIS to fix FieldInfo

public class PoisonDartShooter : MonoBehaviour
{
    [Header("Wire in Inspector")]
    public GameObject dartPrefab;
    public Transform firePoint;

    [Header("Dart Settings")]
    public float dartSpeed = 12f;
    public float dartDamage = 15f;
    public float fireCooldown = 0.8f;
    private float nextFireTime; // Cleaned up the "redundant" warning

    private PlayerStatModifier stats;
    private HeroKnight heroKnight;

    void Start()
    {
        stats = GetComponent<PlayerStatModifier>();
        heroKnight = GetComponent<HeroKnight>();
    }

    void Update()
    {
        if (stats == null || !stats.hasPoisonDarts) return;

        if (Input.GetKeyDown(KeyCode.E) && Time.time >= nextFireTime)
        {
            ShootDart();
            nextFireTime = Time.time + fireCooldown;
        }
    }

    void ShootDart()
    {
        if (dartPrefab == null || firePoint == null) return;

        GameObject dart = Instantiate(dartPrefab, firePoint.position, Quaternion.identity);
        PoisonDart dartScript = dart.GetComponent<PoisonDart>();

        if (dartScript != null)
        {
            // Get the facing direction from HeroKnight
            // (Make sure m_facingDirection is an int in HeroKnight!)
            FieldInfo facingField = typeof(HeroKnight).GetField(
                "m_facingDirection",
                BindingFlags.NonPublic | BindingFlags.Instance
            );

            int facing = 1;
            if (facingField != null)
            {
                facing = (int)facingField.GetValue(heroKnight);
            }

            // If "Launch" still shows red, check the PoisonDart script 
            // and ensure the method is "public void Launch"
            dartScript.Launch(facing, dartSpeed, dartDamage);
        }
    }
}