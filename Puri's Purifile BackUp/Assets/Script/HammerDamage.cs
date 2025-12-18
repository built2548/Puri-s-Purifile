using UnityEngine;

public class HammerDamage : MonoBehaviour
{
    [Header("Audio Settings")]
    // ⭐ NEW: Drag a child GameObject with an AudioSource here
    [SerializeField] private AudioSource smashSource; 

    [Header("Hammer Settings")]
    [SerializeField] int damageAmount = 1; 
    [SerializeField] Collider2D damageCollider; 

    private void Awake()
    {
        // We no longer need to find the AudioManager tag!
        // The sound is now handled by the smashSource reference.
    }

    public void EnableDamage()
    {
        if (damageCollider != null)
        {
             damageCollider.enabled = true;

             // ⭐ NEW: Play the local smash sound
             if (smashSource != null)
             {
                 // Randomize pitch slightly for a better "heavy" feel
                 smashSource.pitch = Random.Range(0.85f, 1.15f);
                 smashSource.Play();
             }

             Debug.Log("Hammer Damage ENABLED.");
        }
    }
    
    public void DisableDamage()
    {
        if (damageCollider != null)
        {
             damageCollider.enabled = false;
             Debug.Log("Hammer Damage DISABLED.");
        }
    }
}