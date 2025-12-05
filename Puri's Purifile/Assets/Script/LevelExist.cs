using UnityEngine;
using UnityEngine.SceneManagement;
public class LevelExist : MonoBehaviour
{
    void OnTriggerEnter2D(Collider2D other)
    {

        int currentSceneIndex = SceneManager.GetActiveScene().buildIndex;
        SceneManager.LoadScene(currentSceneIndex + 1);
    }
}
