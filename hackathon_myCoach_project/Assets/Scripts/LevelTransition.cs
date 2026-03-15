using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelTransition : MonoBehaviour
{
    [Tooltip("The exact name of the scene to load")]
    [SerializeField] private string nextSceneName = "OutroScene";

    private void OnTriggerEnter2D(Collider2D collision)
    {
        // Only trigger if the actual player touches it
        if (collision.CompareTag("Player"))
        {
            // Unpause the game just in case a UI panel froze time right before touching the trigger
            Time.timeScale = 1f; 
            
            SceneManager.LoadScene(nextSceneName);
        }
    }
}