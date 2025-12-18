using UnityEngine;

public class ScoreItem : MonoBehaviour
{
    [SerializeField] private int scoreValue = 100;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            Puri_Script player = other.GetComponent<Puri_Script>();
            if (player != null)
            {
                player.PlayPickupSound();
                
                // If you have a ScoreManager, you would add points here:
                // ScoreManager.Instance.AddScore(scoreValue);
                
                Debug.Log("Collected Score: " + scoreValue);
                Destroy(gameObject);
            }
        }
    }
}