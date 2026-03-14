using UnityEngine;
using UnityEngine.SceneManagement;
using static Unity.Collections.AllocatorManager;

public class ExitDoor : MonoBehaviour
{
    [Header("Exact name of the next scene")]
    public string nextSceneName;

    [Header("Enemies in this scene (drag them all in)")]
    public GameObject[] enemies;

    private bool doorOpen = false;

    void Update()
    {
        // Check every frame if all enemies are dead
        if (!doorOpen && AllEnemiesDead())
            OpenDoor();
    }

    private bool AllEnemiesDead()
    {
        if (enemies.Length == 0) return true;

        foreach (GameObject enemy in enemies)
        {
            // Enemy is dead if the object is inactive
            if (enemy != null && enemy.activeInHierarchy)
                return false;
        }
        return true;
    }

    private void OpenDoor()
    {
        doorOpen = true;
        Debug.Log("All enemies dead — door is open!");

        // Turn the door green so player knows to go here
        SpriteRenderer sr = GetComponent<SpriteRenderer>();
        if (sr != null) sr.color = Color.green;
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Player")) return;

        if (doorOpen)
        {
            if (string.IsNullOrEmpty(nextSceneName))
            {
                Debug.LogError("No next scene name set on ExitDoor!");
                return;
            }
            SceneManager.LoadScene(nextSceneName);
        }
        else
        {
            Debug.Log("Kill all enemies first!");
        }
    }
}