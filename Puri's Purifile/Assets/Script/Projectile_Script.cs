using UnityEngine;

public class Projectile_Script : MonoBehaviour
{
    [SerializeField] float lifetime = 3f; 
    [SerializeField] int damage = 1; 

    // 1. Added LayerMask for environment collisions
    [SerializeField] private LayerMask obstructionLayer;

    void Start()
    {
        Destroy(gameObject, lifetime);
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        // 2. Player Detection (Keep using Tag)
        if (other.gameObject.CompareTag("Player"))
        {
            Puri_Script player = other.GetComponent<Puri_Script>(); 

            if (player != null)
            {
                player.TakeDamage(); 
                Debug.Log("Enemy hit Player!");
            }
            
            Destroy(gameObject);
            return; // Exit to avoid double-checking layers
        }

        // 3. LAYER CHECK: Destroy if it hits Ground, Walls, or the Physical Door
        // This uses the same bitwise check as your player bullet
        if (((1 << other.gameObject.layer) & obstructionLayer) != 0)
        {
            // Optional: If you want enemy bullets to destroy "Props" specifically
            if (other.gameObject.CompareTag("Prop"))
            {
                Destroy(other.gameObject);
            }

            Destroy(gameObject);
        }
    }
}