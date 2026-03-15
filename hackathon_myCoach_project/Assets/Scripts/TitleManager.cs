using UnityEngine;
using UnityEngine.SceneManagement;

public class TitleManager : MonoBehaviour
{
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            // Put the exact spelling of your next scene here
            SceneManager.LoadScene("FINAL_SCENE"); 
        }
    }
}