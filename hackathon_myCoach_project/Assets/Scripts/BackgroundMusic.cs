using UnityEngine;

// This physically forces Unity to add an AudioSource to the object so you can't forget it.
[RequireComponent(typeof(AudioSource))]
public class BackgroundMusic : MonoBehaviour
{
    private static BackgroundMusic instance;
    public AudioSource audioSource;

    void Awake()
    {
        // 1. The Clone Killer (Singleton Pattern)
        if (instance != null && instance != this)
        {
            Destroy(gameObject); 
            return;
        }

        // 2. The Immortality Declaration
        instance = this;
        DontDestroyOnLoad(gameObject);

        // 3. Ensure the audio is actually set to loop
        audioSource = GetComponent<AudioSource>();
        audioSource.loop = true;

        if (!audioSource.isPlaying)
        {
            audioSource.Play();
        }
    }
}