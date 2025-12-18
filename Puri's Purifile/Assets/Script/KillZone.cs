using UnityEngine;

public class KillZone : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            Puri_Script player = collision.GetComponent<Puri_Script>();
            if (player != null)
            {
                // Force health to 0 to trigger the full death sequence
                player.KillInstantly();
            }
        }
    }
}