using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

public class Enemy_Script : MonoBehaviour
{
    public enum EnemyState { Patrol, Chase, Attack }

    

    [Header("Enemy Stats")]
    [SerializeField] int lives = 3;
    private bool isDead = false;

    // Public getter for the current lives
    public int Lives { get { return lives; } } 
    // Public getter for the max lives (used for the health bar's ratio calculation)
    public int MaxLives { get; private set; } // Auto-property to store the initial max lives

    [Header("State Control")]
    public EnemyState currentState = EnemyState.Patrol;

    [Header("Movement")]
    [SerializeField] float moveSpeed = 3f;
    [SerializeField] float patrolDistance = 5f;
    private Vector3 startPosition;
    private float patrolDirection = 1f; // 1 for right, -1 for left
    [SerializeField] float wallCheckDistance = 0.5f; // Distance to look for a wall

    [Header("Detection & Attack")]
    [SerializeField] float verticalTolerance = 3f; // Max height difference the enemy will tolerate
    [SerializeField] LayerMask obstructionLayer; // Layer that blocks line of sight (e.g., "Ground")
    [SerializeField] float chaseRange = 8f; // Distance at which enemy starts chasing
    [SerializeField] float attackRange = 5f; // Distance at which enemy stops and attacks
    [SerializeField] GameObject projectilePrefab;
    [SerializeField] float projectileSpeed = 10f;
    [SerializeField] float fireRate = 2f;

    // This will be found in Awake()
    private HealthBar healthBar; 
    private float nextFireTime;
    private float dyingTime = 2.0f; 


    Animator myAnimator;
    CapsuleCollider2D myCapsule;
    // References
    private Rigidbody2D rb;
    private Transform player; 
    AudioManager audioManager;


    private void Awake()
    {
        // Store the initial lives count as the MaxLives
        MaxLives = lives; 
        // GetComponentInChildren will find the HealthBar script on a child object
        healthBar = GetComponentInChildren<HealthBar>();
        audioManager = GameObject.FindGameObjectWithTag("Audio").GetComponent<AudioManager>();
    }
 void Start()
    {
        // --- HEALTH BAR INITIALIZATION ---
        // Display full health at the start of the game
        if (healthBar != null)
        {
            healthBar.UpdateHealthBar(Lives, MaxLives);
        }
        // ---------------------------------
        
        rb = GetComponent<Rigidbody2D>();
        myCapsule = GetComponent<CapsuleCollider2D>();
        startPosition = transform.position;
        // Find the player object
        player = GameObject.FindGameObjectWithTag("Player").transform; 
        nextFireTime = Time.time;
        myAnimator = GetComponent<Animator>();

        // IMPORTANT: If any of the GetComponent calls return null, the script will eventually fail.
    }

    void Update()
    {
        if (player == null) return; 

        float distanceToPlayer = Vector2.Distance(transform.position, player.position);
        float verticalDifference = Mathf.Abs(transform.position.y - player.position.y);

        // --- LINE OF SIGHT CHECK ---
        bool hasLineOfSight = !Physics2D.Raycast(
            transform.position, 
            player.position - transform.position, 
            distanceToPlayer, 
            obstructionLayer
        );
        
        // --- STATE SWITCHING LOGIC ---
        // Save the previous state to check for a transition
        EnemyState previousState = currentState;
        
        if (verticalDifference <= verticalTolerance && hasLineOfSight)
        {
            if (distanceToPlayer <= attackRange)
            {
                currentState = EnemyState.Attack;
            }
            else if (distanceToPlayer <= chaseRange)
            {
                currentState = EnemyState.Chase;
            }
            else
            {
                currentState = EnemyState.Patrol;
            }
        }
        else
        {
            currentState = EnemyState.Patrol;
        }


        // *** Cooldown reset logic REMOVED for stability ***
        if (previousState != EnemyState.Attack && currentState == EnemyState.Attack)
        {
            // Do not reset nextFireTime here to respect fire rate
        }
        


        // --- Execute Current State (switch block) ---
        switch (currentState)
        {
            case EnemyState.Patrol:
            
                Patrol();
                break;
            case EnemyState.Chase:
            audioManager.PlaySFX(audioManager.alienChasing);
                ChasePlayer();
                break;
            case EnemyState.Attack:
            audioManager.PlaySFX(audioManager.laser);
                AttackPlayer();
                break;
        }

    }

void Patrol()
{
    // 1. WALL DETECTION
    // Create a Raycast looking ahead based on the current patrol direction
    Vector2 rayOrigin = transform.position;
    Vector2 rayDirection = new Vector2(patrolDirection, 0);
    
    RaycastHit2D wallHit = Physics2D.Raycast(rayOrigin, rayDirection, wallCheckDistance, obstructionLayer);

    // DEBUG: Shows the wall check ray in the Scene view (Green if clear, Red if hit)
    Debug.DrawRay(rayOrigin, rayDirection * wallCheckDistance, wallHit.collider != null ? Color.red : Color.green);

    // 2. FLIP DIRECTION (Wall Hit OR Boundary Reached)
    float currentX = transform.position.x;

    if (wallHit.collider != null)
    {
        // Hit a wall! Turn around immediately
        patrolDirection *= -1f;
        // Optional: Move startPosition to current spot to reset patrol distance logic
        startPosition.x = transform.position.x; 
    }
    else if (currentX > startPosition.x + patrolDistance)
    {
        patrolDirection = -1f; // Move Left
    }
    else if (currentX < startPosition.x - patrolDistance)
    {
        patrolDirection = 1f; // Move Right
    }

    // 3. APPLY MOVEMENT
    rb.velocity = new Vector2(moveSpeed * patrolDirection, rb.velocity.y);

    // 4. VISUALS
    FlipSprite(patrolDirection); 
    myAnimator.SetBool("isWalking", true);
}
    void ChasePlayer()
    {
        // Determine direction to player
        float directionToPlayer = Mathf.Sign(player.position.x - transform.position.x);
        
        // Move
        rb.velocity = new Vector2(moveSpeed * directionToPlayer, rb.velocity.y);
        
        // Flip Sprite
        FlipSprite(directionToPlayer);
        
        // Animate
        myAnimator.SetBool("isWalking", true);
    }

    void AttackPlayer()
    {
        // 1. Stop Movement
        rb.velocity = new Vector2(0f, rb.velocity.y);
        
        // 2. Stop walking animation
        myAnimator.SetBool("isWalking", false);

        // 3. Face Player Direction
        float directionToPlayer = Mathf.Sign(player.position.x - transform.position.x);
        FlipSprite(directionToPlayer); 
        
        // 4. Attack (Shoot)
        if (Time.time >= nextFireTime)
        {
            // Set the trigger and shoot only when ready to fire
            myAnimator.SetTrigger("Shoot"); 
            ShootProjectile();
            nextFireTime = Time.time + fireRate;
            
        }
    }

    public void ShootProjectile() 
    {
        
        // 1. Get the direction *just before* spawning the projectile
        float direction = Mathf.Sign(player.position.x - transform.position.x);

        // 2. Create the projectile slightly in front of the enemy
        Vector3 spawnPosition = transform.position + new Vector3(direction * 0.5f, 0, 0); 
        
        // 3. Instantiate and get reference
        GameObject projectile = Instantiate(projectilePrefab, spawnPosition, Quaternion.identity);
        
        // 4. Get the Rigidbody2D of the projectile
        Rigidbody2D projRb = projectile.GetComponent<Rigidbody2D>();
        // Adjust projectile scale based on direction
        Vector3 currentScale = projectile.transform.localScale;
        projectile.transform.localScale = new Vector3(
            Mathf.Sign(direction) * Mathf.Abs(currentScale.x), 
            currentScale.y, 
            currentScale.z
        );
        
        // 5. Set the velocity
        projRb.velocity = new Vector2(direction * projectileSpeed, 0f);
        
    }

    void FlipSprite(float direction)
    {
        // Flips the sprite based on the sign of the direction
        if (Mathf.Abs(direction) > 0)
        {
            transform.localScale = new Vector3(Mathf.Sign(direction) * Mathf.Abs(transform.localScale.x), transform.localScale.y, transform.localScale.z);
        }
    }

    /// <summary>
    /// Reduces enemy lives by the damage amount and triggers death if lives <= 0.
    /// </summary>
public void TakeDamage(int damageAmount)
    {
        if (isDead) return; // Ignore damage if already dead
        lives -= damageAmount;
        Debug.Log($"Enemy took {damageAmount} damage. Lives remaining: {lives}");
        audioManager.PlaySFX(audioManager.alienHit);

        // --- HEALTH BAR UPDATE ---
        if (healthBar != null)
        {
            healthBar.UpdateHealthBar(Lives, MaxLives);
        }
        // -------------------------

        myAnimator.SetTrigger("Hurt");
        
        if (lives <= 0)
        {
            Die();
        }
    }

    /// <summary>
    /// Handles the enemy's destruction or death sequence.
    /// </summary>
private void Die()
    {
        isDead = true; 
        myAnimator.ResetTrigger("Hurt");
        Debug.Log("Enemy destroyed!");
        
        // Hide/Clear Health Bar on death
        if (healthBar != null)
        {
            audioManager.PlaySFX(audioManager.alienDeath);
            healthBar.UpdateHealthBar(0, MaxLives);
        }
        
        // Animation
        myAnimator.SetBool("isWalking", false);
        myAnimator.ResetTrigger("Shoot");
        myAnimator.SetTrigger("Dead");
        myCapsule.offset = new Vector2(0f, 0.7f); // Adjust collider offset
        // Physics & Script Control
        rb.velocity = Vector2.zero;
        this.enabled = false; 

        Destroy(gameObject, dyingTime); 
    }
}