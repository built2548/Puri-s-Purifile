using UnityEngine;

public class KillZone : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] private bool killInstantly = false; // True = Game Over, False = Lose 1 Life & Respawn

    private void OnTriggerEnter2D(Collider2D other)
    {
        // Check if the object entering the zone is the Player
        if (other.CompareTag("Player"))
        {
            Puri_Script player = other.GetComponent<Puri_Script>();

            if (player != null)
            {
                if (killInstantly)
                {
                    // Forces lives to 0 and triggers Game Over
                    player.KillInstantly();
                }
                else
                {
                    // Subtracts 1 life and triggers the Respawn at Checkpoint
                    player.TakeDamage();
                }
            }
        }
    }
}