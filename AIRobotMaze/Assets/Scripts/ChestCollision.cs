using UnityEngine;
using UnityEngine.SceneManagement;

public class ChestCollision : MonoBehaviour
{
    // This method will be called when the Chest collides with any object
    void OnCollisionEnter(Collision collision)
    {
        // Check if the collided object is tagged as "Pathfinder"
        if (collision.gameObject.CompareTag("Pathfinder"))
        {
            // If a Pathfinder collides with the Chest, load the WinScene
            Debug.Log("Hello");
            SceneManager.LoadScene("WinScene");
        }
    }
}
