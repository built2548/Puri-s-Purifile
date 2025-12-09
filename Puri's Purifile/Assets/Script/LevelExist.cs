using UnityEngine;
using UnityEngine.SceneManagement;
public class LevelExist : MonoBehaviour
{
    void OnTriggerEnter2D(Collider2D other)
    {
      if (other.CompareTag("Player"))
        {
            // If it is the Player, proceed to the next scene
            int currentSceneIndex = SceneManager.GetActiveScene().buildIndex;
            
            // Note: You might want to use a Coroutine here to add a brief delay
            // or fade effect before loading the next scene.
            
            SceneManager.LoadScene(currentSceneIndex + 1);
        }
    }
}
