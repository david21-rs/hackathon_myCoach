using UnityEngine;

public class AutoDestroyVFX : MonoBehaviour
{
    void Start()
    {
        // Adjust this number to match the exact length of your animation clip
        Destroy(gameObject, 0.6f); 
    }
}