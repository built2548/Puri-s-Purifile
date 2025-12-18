using UnityEngine;

public class PowerUpItem : MonoBehaviour
{
    [SerializeField] private AudioSource pickupSound; // Optional: child audio source

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            Puri_Script player = other.GetComponent<Puri_Script>();
            if (player != null)
            {
                player.UpgradeBullet(); // Tell player to level up
                
                // Sound logic
                if (pickupSound != null)
                {
                    pickupSound.transform.SetParent(null);
                    pickupSound.Play();
                    Destroy(pickupSound.gameObject, 2f);
                }
                
                Destroy(gameObject); // Remove item from scene
            }
        }
    }
}