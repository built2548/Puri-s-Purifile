using UnityEngine;
using System.Collections;
using System.Collections.Generic;
public class Enemy_Script : MonoBehaviour
{
public enum EnemyState { Patrol, Chase, Attack }

[Header("State Control")]
public EnemyState currentState = EnemyState.Patrol;

[Header("Movement")]
[SerializeField] float moveSpeed = 3f;
[SerializeField] float patrolDistance = 5f;
private Vector3 startPosition;
private float patrolDirection = 1f; // 1 for right, -1 for left

[Header("Detection & Attack")]
[SerializeField] float verticalTolerance = 3f; // Max height difference the enemy will tolerate
[SerializeField] LayerMask obstructionLayer; // Layer that blocks line of sight (e.g., "Ground")
[SerializeField] float chaseRange = 8f; // Distance at which enemy starts chasing
[SerializeField] float attackRange = 5f; // Distance at which enemy stops and attacks
[SerializeField] GameObject projectilePrefab;
[SerializeField] float projectileSpeed = 10f;
[SerializeField] float fireRate = 2f;
private float nextFireTime;
Animator myAnimator;
// References
private Rigidbody2D rb;
private Transform player; // Set this in Start or through a Find call
// private bool enteredAttackState = false; // Flag is no longer needed

void Start()
{
    rb = GetComponent<Rigidbody2D>();
    startPosition = transform.position;
    // Find the player object
    player = GameObject.FindGameObjectWithTag("Player").transform; 
    nextFireTime = Time.time;
    myAnimator = GetComponent<Animator>();
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

    // *** FIX 1: Only reset nextFireTime if we've just entered the Attack state. ***
    // This makes sure the enemy can fire immediately upon seeing the player,
    // but only on the first frame of entering the state.
    if (previousState != EnemyState.Attack && currentState == EnemyState.Attack)
    {
        nextFireTime = 0f;
    }


    // --- Execute Current State (switch block) ---
    switch (currentState)
    {
        case EnemyState.Patrol:
            Patrol();
            break;
        case EnemyState.Chase:
            ChasePlayer();
            break;
        case EnemyState.Attack:
            AttackPlayer();
            break;
    }
}
void Patrol()
{
    // 1. Move
    rb.velocity = new Vector2(moveSpeed * patrolDirection, rb.velocity.y);

    // 2. Flip Direction (when boundaries are hit)
    float currentX = transform.position.x;
    
    // Check if the enemy is too far left or right of the starting point
    if (currentX > startPosition.x + patrolDistance)
    {
        patrolDirection = -1f; // Move Left
    }
    else if (currentX < startPosition.x - patrolDistance)
    {
        patrolDirection = 1f; // Move Right
    }

    // 3. Flip Sprite (Visual)
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
    if (Time.time >= nextFireTime) // *** FIX 2: Use >= for consistency/safety, though > works. ***
    {
        // Set the trigger and shoot only when ready to fire
        myAnimator.SetTrigger("Shoot"); 
        ShootProjectile();
        nextFireTime = Time.time + fireRate;
    }
    
    
}
// Change this:
// void Shoot(float direction) { ... }

// To this:
public void ShootProjectile() // 👈 IMPORTANT: Must be public for Animation Events
{
    ;
    // 1. Get the direction *just before* spawning the projectile
    float direction = Mathf.Sign(player.position.x - transform.position.x);

    // 2. Create the projectile slightly in front of the enemy
    Vector3 spawnPosition = transform.position + new Vector3(direction * 0.5f, 0, 0); 
    
    // 3. Instantiate and get reference
    GameObject projectile = Instantiate(projectilePrefab, spawnPosition, Quaternion.identity);
    
    // 4. Get the Rigidbody2D of the projectile
    Rigidbody2D projRb = projectile.GetComponent<Rigidbody2D>();
    
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
}