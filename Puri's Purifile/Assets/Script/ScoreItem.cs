using UnityEngine;

public class ScoreItem : MonoBehaviour
{
    [SerializeField] private int scoreValue = 100;
    [SerializeField] private GameObject pickupEffect; // Optional: Particle effect

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            // If you have a ScoreManager, call it here:
            // ScoreManager.Instance.AddScore(scoreValue);
            
            Debug.Log("Score + " + scoreValue);

            // Play the pickup sound from the Player's child source
            Puri_Script player = other.GetComponent<Puri_Script>();
            if (player != null)
            {
                player.PlayPickupSound();
            }

            if (pickupEffect != null)
            {
                Instantiate(pickupEffect, transform.position, Quaternion.identity);
            }

            Destroy(gameObject);
        }
    }
}