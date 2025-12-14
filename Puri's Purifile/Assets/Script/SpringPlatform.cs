using UnityEngine;

public class SpringPlatform : MonoBehaviour
{
    // Adjust this value in the Inspector to control jump height
    [SerializeField] float launchForce = 20f; 
    
    // Optional: Add a reference to the Animator component if the spring animates
    [SerializeField] Animator springAnimator;

    // Use a private variable to store the player's Rigidbody, 
    // so we can apply the force directly.
    private Rigidbody2D playerRb;

    // This runs when another object enters the trigger zone
    void OnTriggerEnter2D(Collider2D other)
    {
        // 1. Check if the object entering the trigger is the player
        if (other.CompareTag("Player"))
        {
            // 2. Get the player's Rigidbody component
            playerRb = other.GetComponent<Rigidbody2D>();

            if (playerRb != null)
            {
                // 3. Optional: Play the spring compression/launch animation
                if (springAnimator != null)
                {
                    springAnimator.SetTrigger("Launch"); 
                }

                // 4. Reset the player's vertical velocity to ensure a consistent jump
                playerRb.velocity = new Vector2(playerRb.velocity.x, 0f);

                // 5. Apply the upward force using Impulse for an immediate, sharp effect
                playerRb.AddForce(Vector2.up * launchForce, ForceMode2D.Impulse);
                
                Debug.Log($"Player launched with force: {launchForce}");
            }
        }
    }
}