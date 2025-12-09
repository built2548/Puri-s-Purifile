using UnityEngine;

public class PlayerProjectile : MonoBehaviour
{
    [SerializeField] float lifetime = 3f;
    [SerializeField] int damageAmount = 1; // Damage value for enemies

    void Start()
    {
        Destroy(gameObject, lifetime);
    }
    public void Initialize(float direction)
    {
        // Flip the sprite scale to match the direction
        FlipSprite(direction);
    }
    void FlipSprite(float direction)
    {
        // Get the current local scale of the projectile
        Vector3 currentScale = transform.localScale;

        // Apply the flip: use the sign of the direction to set the X scale.
        // The Mathf.Abs ensures you keep the magnitude of the original scale.
        transform.localScale = new Vector3(
            Mathf.Sign(direction) * Mathf.Abs(currentScale.x), 
            currentScale.y, 
            currentScale.z)
            ;
    }

void OnTriggerEnter2D(Collider2D other)
{
    if (other.CompareTag("Enemy"))
    {
        // Get the Enemy_Script component from the object we hit
        Enemy_Script enemy = other.GetComponent<Enemy_Script>();
        
        if (enemy != null)
        {
            // Call the new TakeDamage method, passing 1 as the damage amount.
            enemy.TakeDamage(1); 
        }
        
        Destroy(gameObject); // Destroy the bullet
    }
        
        // 2. Destroy if it hits ground or walls
        if (other.gameObject.layer == LayerMask.NameToLayer("Ground") || other.gameObject.layer == LayerMask.NameToLayer("Obstruction"))
        {
             Destroy(gameObject);
        }
    }
}