using UnityEngine;

public class PlayerProjectile : MonoBehaviour
{
    [SerializeField] float lifetime = 3f;
    [SerializeField] int damageAmount = 1;

    // 1. Add a LayerMask to define what counts as "Ground/Wall"
    [SerializeField] private LayerMask groundLayer;

    void Start()
    {
        Destroy(gameObject, lifetime);
    }

    public void Initialize(float direction)
    {
        FlipSprite(direction);
    }

    void FlipSprite(float direction)
    {
        Vector3 currentScale = transform.localScale;
        transform.localScale = new Vector3(
            Mathf.Sign(direction) * Mathf.Abs(currentScale.x), 
            currentScale.y, 
            currentScale.z);
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        // 2. Check for Enemy Tag (Keep this as Tag, it's fine for logic)
        if (other.CompareTag("Enemy"))
        {
            Enemy_Script enemy = other.GetComponent<Enemy_Script>();
            if (enemy != null)
            {
                enemy.TakeDamage(damageAmount); 
            }
            
            Destroy(gameObject); 
            return; // Exit so we don't check layers if we already hit an enemy
        }
        
        // 3. LAYER CHECK: Destroy if it hits Ground, Walls, or your Physical Door
        // This checks if the 'other' object's layer is included in our LayerMask
        if (((1 << other.gameObject.layer) & groundLayer) != 0)
        {
            Debug.Log("Projectile hit wall/ground layer: " + LayerMask.LayerToName(other.gameObject.layer));
            Destroy(gameObject);
        }
    }
}