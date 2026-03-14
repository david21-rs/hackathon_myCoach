using UnityEngine;
using UnityEngine.SceneManagement;

public class ExitDoor : MonoBehaviour
{
    [Header("Exact name of the next scene")]
    public string nextSceneName;

    private bool doorOpen = false;
    private SpriteRenderer spriteRenderer;
    private Collider2D doorCollider;

    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        doorCollider = GetComponent<Collider2D>();

        // Hide the visual and disable the trigger so the player cannot interact early
        if (spriteRenderer != null) spriteRenderer.enabled = false;
        if (doorCollider != null) doorCollider.enabled = false;

        // Check for enemies twice a second instead of every single frame
        InvokeRepeating(nameof(CheckEnemies), 0.5f, 0.5f);
    }

    private void CheckEnemies()
    {
        if (doorOpen) return;

        // FindGameObjectsWithTag ONLY finds active objects in the hierarchy.
        // If your enemies are Destroyed or SetActive(false) upon death, this array becomes empty.
        GameObject[] activeEnemies = GameObject.FindGameObjectsWithTag("Enemy");

        if (activeEnemies.Length == 0)
        {
            OpenDoor();
        }
    }

    private void OpenDoor()
    {
        doorOpen = true;
        CancelInvoke(nameof(CheckEnemies)); // Stop scanning the scene to save CPU

        // Reveal the door and allow collisions
        if (spriteRenderer != null) spriteRenderer.enabled = true;
        if (doorCollider != null) doorCollider.enabled = true;
        
        Debug.Log("All enemies dead — door revealed!");
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (!doorOpen || !other.CompareTag("Player")) return;

        if (string.IsNullOrEmpty(nextSceneName))
        {
            Debug.LogError("No next scene name set on ExitDoor!");
            return;
        }
        
        SceneManager.LoadScene(nextSceneName);
    }
}