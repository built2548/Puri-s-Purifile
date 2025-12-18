using UnityEngine;

public class GasScript : MonoBehaviour
{
    [Header("Audio Settings")]
    // ⭐ NEW: Drag a child GameObject with an AudioSource here
    [SerializeField] private AudioSource GasSource; 

    [Header("Hammer Settings")]
    [SerializeField] int damageAmount = 1; 
    [SerializeField] Collider2D damageCollider; 

    private void Awake()
    {
        // We no longer need to find the AudioManager tag!
        // The sound is now handled by the GasSource reference.
    }

    public void EnableDamage()
    {
        if (damageCollider != null)
        {
             damageCollider.enabled = true;

             Debug.Log("Gas Damage ENABLED.");
        }
    }
    
    public void DisableDamage()
    {
        if (damageCollider != null)
        {
             damageCollider.enabled = false;

             Debug.Log("Gas Damage DISABLED.");
        }
    }
    public void PlayGasSound()
    {
        // ⭐ NEW: Play the local gas sound
        if (GasSource != null)
        {
            // Randomize pitch slightly for variation
            GasSource.pitch = Random.Range(0.85f, 1.15f);
            GasSource.Play();
        }
    }
}
