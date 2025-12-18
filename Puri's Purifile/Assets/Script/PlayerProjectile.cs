using UnityEngine;

public class PlayerProjectile : MonoBehaviour
{
    [Header("Adjustable Stats")]
    [SerializeField] private float speed = 20f;
    [SerializeField] private int damageAmount = 1;
    [SerializeField] private float lifetime = 3f;

    [Header("Collision Layers")]
    [SerializeField] private LayerMask groundLayer;

    private Rigidbody2D rb;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    void Start()
    {
        Destroy(gameObject, lifetime);
    }

    public void Initialize(float direction)
    {
        if (rb != null)
        {
            rb.linearVelocity = new Vector2(direction * speed, 0f);
        }
        FlipSprite(direction);
    }

    void FlipSprite(float direction)
    {
        Vector3 scale = transform.localScale;
        transform.localScale = new Vector3(Mathf.Sign(direction) * Mathf.Abs(scale.x), scale.y, scale.z);
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player") || other.isTrigger) return;

        if (other.CompareTag("Enemy"))
        {
            Enemy_Script enemy = other.GetComponent<Enemy_Script>();
            if (enemy != null) enemy.TakeDamage(damageAmount);
            Destroy(gameObject);
        }
        
        if (((1 << other.gameObject.layer) & groundLayer) != 0)
        {
            Destroy(gameObject);
        }
    }
}