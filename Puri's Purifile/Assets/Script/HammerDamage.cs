using UnityEngine;

public class HammerDamage : MonoBehaviour
{
    [SerializeField] int damageAmount = 1; 
    
    // ⭐ NEW REFERENCE: Drag the Child Damage Zone's Collider here in the Inspector
    [SerializeField] Collider2D damageCollider; 

    // --- Methods called by Animation Events ---
    
    public void EnableDamage()
    {
        // Now enables the collider reference we assigned in the Inspector
        if (damageCollider != null)
        {
             damageCollider.enabled = true;
             Debug.Log("Hammer Damage ENABLED.");
        }
    }
    
    public void DisableDamage()
    {
        // Now disables the collider reference we assigned in the Inspector
        if (damageCollider != null)
        {
             damageCollider.enabled = false;
             Debug.Log("Hammer Damage DISABLED.");
        }
    }
    
    // ⭐ IMPORTANT: This OnTriggerEnter2D method MUST be moved back 
    // to the Child Damage Zone script/object if you want the trigger detection 
    // to work there, OR you can keep it here if the main hammer has the trigger collider.
    // However, since the *Child* is the trigger, you need a small script *there* too.
    
    // This script should ONLY handle the enable/disable functions.
    // The actual collision detection should stay on the Child Damage Zone.
}