using UnityEngine;

public class Projectile_Script : MonoBehaviour
{
    [SerializeField] float lifetime = 3f; // Projectile destroys itself after 3 seconds
    [SerializeField] int damage = 1; // Damage value is optional if Puri_Script doesn't use it

    void Start()
    {
        // Start a timer to destroy the projectile automatically
        Destroy(gameObject, lifetime);
    }

    // Called when the collider hits another object
    void OnTriggerEnter2D(Collider2D other)
    {
        // Check if the projectile hit the player using the "Player" tag
        if (other.gameObject.tag == "Player")
        {
            // 1. Get the Puri_Script component from the collided object (the Player)
            Puri_Script player = other.GetComponent<Puri_Script>(); 

            // 2. Check if the component was found (good practice!)
            if (player != null)
            {
                // 3. Call the TakeDamage method on the player
                // NOTE: Your Puri_Script.TakeDamage() does not take the 'damage' parameter, 
                // it just handles the life loss.
                player.TakeDamage(); 
                
                Debug.Log("Hit Player! Calling TakeDamage on Puri_Script.");
            }
            
            // Destroy the projectile after impact
            Destroy(gameObject);
        }

        // Optional: Destroy the projectile if it hits a wall/obstruction
        if (other.gameObject.tag == "Prop")
        {
            Destroy(other.gameObject);
            Destroy(gameObject);
        }
    }
}