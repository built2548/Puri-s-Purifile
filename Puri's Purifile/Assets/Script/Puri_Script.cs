using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using FirstGearGames.SmoothCameraShaker;

public class Puri_Script : MonoBehaviour
{
    [Header("Bullet Levels")]
    [SerializeField] private GameObject[] bulletPrefabs; // Slot 0 = Lv1, Slot 1 = Lv2, Slot 2 = Lv3
    private int bulletLevel = 0; // 0 is Lv1, 1 is Lv2, 2 is Lv3

    [Header("Audio Child References")]
    [SerializeField] private AudioSource jumpSource;
    [SerializeField] private AudioSource shootSource;
    [SerializeField] private AudioSource hitSource;
    [SerializeField] private AudioSource pickupSource;
    [SerializeField] private AudioSource deathSource;

    [Header("UI")]
    [Header("Ground Check")]
    [SerializeField] float groundCheckDistance = 0.1f;
    [SerializeField] LayerMask whatIsGround;
    [Header("Player Stats")]
    [SerializeField] int lives = 3;
    public int Lives { get { return lives; } }
    // 💡 NEW: Duration player is invulnerable after a hit
    [SerializeField] float invulnerableDuration = 1.5f;
    private bool isInvulnerable = false; // 💡 NEW: Flag to block damage
    [Header("Respawn Logic")]
    private Vector3 currentCheckpoint;

    // ⭐ NEW: Shooting Fields
    [Header("Shooting")]
    [SerializeField] GameObject projectilePrefab; // Assign the bullet prefab in the Inspector
    [SerializeField] float fireRate = 0.5f; // Time between shots
    private float nextFireTime;
    // ⭐ END NEW

    [Header("Movement")]
    [SerializeField] float runspeed = 5f;
    [SerializeField] float jumpSpeed = 5f;
    [SerializeField] float climbSpeed = 5f;
    [SerializeField] Vector2 deathkick = new Vector2(1f, 1f);

    // References and Variables
    Vector2 moveInput;
    Rigidbody2D myRigidbody;
    Vector2 originalScale;
    Animator myAnimator;
    CapsuleCollider2D myCapsule;
    float gravityScaleAtStart;
    bool isAlive = true;
    public ShakeData smallShake;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    private HeartDisplayManager heartDisplayManager;

    void Awake()
    {
        // If the HeartPanel is a permanent object in the scene:
        heartDisplayManager = FindObjectOfType<HeartDisplayManager>();

        if (heartDisplayManager == null)
        {
            Debug.LogError("HeartDisplayManager not found in the scene! Ensure the HeartPanel prefab is instantiated.");
        }
    }
    void Start()
    {
        myRigidbody = GetComponent<Rigidbody2D>();
        originalScale = new Vector2(transform.localScale.x, transform.localScale.y);
        myAnimator = GetComponent<Animator>();
        myCapsule = GetComponent<CapsuleCollider2D>();
        gravityScaleAtStart = myRigidbody.gravityScale;
        currentCheckpoint = transform.position;
        // ⭐ NEW: Initialize the health display on start (show 3 hearts)
        if (heartDisplayManager != null)
        {
            heartDisplayManager.UpdateHealthDisplay(lives);
        }
        // ⭐ END NEW
    }

    // Update is called once per frame
    void Update()
    {
        if (!isAlive) { return; }

        if (myCapsule.IsTouchingLayers(LayerMask.GetMask("Ground", "Climbing", "MovingPlatform")) && Mathf.Abs(myRigidbody.linearVelocity.y) <= Mathf.Epsilon)
        {
            myAnimator.SetBool("isJumping", false);
        }

        Run();
        FlipSprite();
        CheckForDeath();
        ClimbLadder();
    }
    public void UpdateCheckpoint(Vector3 newPos)
    {
        currentCheckpoint = newPos;
    }   

    // ... (OnMove, OnJump, Run, FlipSprite, ClimbLadder, OnAttack are unchanged) ...
    void OnMove(InputValue value)
    {
        if (!isAlive) { return; }
        moveInput = value.Get<Vector2>();
    }
    public void PlayPickupSound()
    {
        if (pickupSource != null)
        {
            pickupSource.pitch = Random.Range(0.9f, 1.1f);
            pickupSource.Play();
        }
    }
    bool IsGrounded()
    {
        // Check below the player's bottom edge for the ground layers
        // Adjust the origin and size based on your player's collider setup (using raycast is simpler)

        // Raycast straight down from the center bottom of the collider:
        Vector2 raycastOrigin = myCapsule.bounds.center + Vector3.down * myCapsule.size.y / 2f;
        RaycastHit2D hit = Physics2D.Raycast(raycastOrigin, Vector2.down, groundCheckDistance, whatIsGround);

        // Optional: Draw a debug line to visualize the check in the Scene View
        Debug.DrawRay(raycastOrigin, Vector2.down * groundCheckDistance, hit.collider != null ? Color.green : Color.red);

        return hit.collider != null;
    }
    void OnJump(InputValue value)
    {
        if (!isAlive) { return; }
        if (!IsGrounded()) { return; }

        if (value.isPressed)
        {
            myRigidbody.linearVelocity += new Vector2(0f, jumpSpeed);
            myAnimator.SetBool("isJumping", true);

            // ⭐ PLAY JUMP SOUND
            if (jumpSource != null) jumpSource.Play();
        }
    }

    void Run()
    {
        if (myAnimator.GetBool("isClimbing"))
        {
            myAnimator.SetBool("isRunning", false);
            return;
        }
        Vector2 playerVelocity = new Vector2(moveInput.x * runspeed, myRigidbody.linearVelocity.y);
        myRigidbody.linearVelocity = playerVelocity;
        bool playerHasHorizontalSpeed = Mathf.Abs(myRigidbody.linearVelocity.x) > Mathf.Epsilon;
        myAnimator.SetBool("isRunning", playerHasHorizontalSpeed);
    }

    void FlipSprite()
    {
        bool playerHasHorizontalSpeed = Mathf.Abs(myRigidbody.linearVelocity.x) > Mathf.Epsilon;
        if (playerHasHorizontalSpeed)
        {
            transform.localScale = new Vector2(Mathf.Sign(myRigidbody.linearVelocity.x), 1f) * originalScale;
        }
    }

    void ClimbLadder()
    {
        if (!myCapsule.IsTouchingLayers(LayerMask.GetMask("Climbing")))
        {
            myRigidbody.gravityScale = gravityScaleAtStart;
            myAnimator.SetBool("isClimbing", false);
            return;
        }

        Vector2 climbVelocity = new Vector2(myRigidbody.linearVelocity.x, moveInput.y * climbSpeed);
        myRigidbody.linearVelocity = climbVelocity;
        myRigidbody.gravityScale = 0f;

        bool playerHasVerticalSpeed = Mathf.Abs(myRigidbody.linearVelocity.y) > Mathf.Epsilon;
        myAnimator.SetBool("isClimbing", playerHasVerticalSpeed);
    }

    void OnAttack(InputValue value)
    {
        if (!isAlive) { return; }

        if (value.isPressed)
        {
            if (Time.time >= nextFireTime) // Check fire rate
            {
                myAnimator.SetTrigger("Attack");
                Shoot(); // ⭐ CALL THE NEW SHOOT FUNCTION
                nextFireTime = Time.time + fireRate; // Reset fire time
            }
        }
    }

    void Shoot()
    {
        // 1. Safety Check
        if (bulletPrefabs == null || bulletLevel >= bulletPrefabs.Length || bulletPrefabs[bulletLevel] == null)
        {
            return;
        }

        // 2. ⭐ PLAY DEFAULT SOUND (From the Player's ShootSource)
        if (shootSource != null)
        {
            shootSource.Play();
        }

        // 3. Calculate Direction & Position
        float direction = Mathf.Sign(transform.localScale.x);
        Vector3 spawnPos = transform.position + new Vector3(direction * 0.8f, 0, 0);

        // 4. Spawn the current level bullet
        GameObject projectile = Instantiate(bulletPrefabs[bulletLevel], spawnPos, Quaternion.identity);

        // 5. Initialize Speed/Damage inside the bullet
        PlayerProjectile projScript = projectile.GetComponent<PlayerProjectile>();
        if (projScript != null)
        {
            projScript.Initialize(direction);
        }
    }
    public void UpgradeBullet()
    {
        // Only upgrade if the NEXT level actually has a prefab assigned
        if (bulletLevel + 1 < bulletPrefabs.Length && bulletPrefabs[bulletLevel + 1] != null)
        {
            bulletLevel++;
        }
        else
        {
            Debug.Log("Already at Max Level or Next Level Prefab is missing!");
        }
    }

    public void ResetBulletLevel()
    {
        bulletLevel = 0; // Reset to Lv1
        Debug.Log("Bullet Reset to Lv1");
    }

    // --- LIVES AND DEATH LOGIC ---

    void CheckForDeath()
    {
        // 💡 ONLY CALL TakeDamage IF NOT INVULNERABLE
        if (!isInvulnerable && myCapsule.IsTouchingLayers(LayerMask.GetMask("Flame")))
        {
            TakeDamage();
        }
    }

public void TakeDamage()
{
    if (!isAlive || isInvulnerable) return;

    lives--;
    ResetBulletLevel();
    if (hitSource != null) hitSource.Play();

    CameraShakerHandler.Shake(smallShake);
    
    if (heartDisplayManager != null)
    {
        heartDisplayManager.UpdateHealthDisplay(lives);
    }

    if (lives <= 0)
    {
        StartCoroutine(HandleDeath(true)); // Total Game Over
    }
    else
    {
        // 💡 ADD THIS: Trigger respawn at checkpoint if player still has lives
        StartCoroutine(HandleDeath(false)); 
    }
}
    public void KillInstantly()
    {
        if (!isAlive) return;
        lives = 0; // Drop lives to zero
        if (heartDisplayManager != null) heartDisplayManager.UpdateHealthDisplay(0);
        StartCoroutine(HandleDeath(true));
    }

    IEnumerator BecomeTemporarilyInvulnerable()
    {
        isInvulnerable = true;

        // Optional: Add visual flashing effect here (e.g., SpriteRenderer.enabled = false/true loop)
        // loop for flashing effect;
        SpriteRenderer spriteRenderer = GetComponent<SpriteRenderer>();
        float flashInterval = 0.1f;
        float elapsed = 0f;
        while (elapsed < invulnerableDuration)
        {

            spriteRenderer.enabled = !spriteRenderer.enabled;
            yield return new WaitForSeconds(flashInterval);
            elapsed += flashInterval;
        }



        yield return new WaitForSeconds(invulnerableDuration);

        isInvulnerable = false;

        // Optional: Stop flashing effect
        spriteRenderer.enabled = true; // Ensure sprite is visible at end

    }

    IEnumerator HandleDeath(bool isGameOver)
    {
        isAlive = false;
        if (deathSource != null)
        {
            deathSource.pitch = Random.Range(0.9f, 1.1f); // Tiny variety makes it feel better
            deathSource.Play();
        }

        // Apply Death Animation and Physics
        myAnimator.SetTrigger("Dying");
        myCapsule.offset = new Vector2(0f, 0.7f);

        // Apply death kick force
        myRigidbody.velocity = new Vector2(
            deathkick.x * -transform.localScale.x,
            deathkick.y
        );

        yield return new WaitForSeconds(1.5f);

        // --- Handle Respawn or Game Over ---
        if (isGameOver)
        {
            Debug.Log("GAME OVER!");
            // SceneManager.LoadScene("GameOverScene"); 
        }
else
{
    // RESPAWN LOGIC
    isAlive = true;
    
    // Move to checkpoint
    transform.position = currentCheckpoint;
    
    // Reset Physics and Visuals
    myRigidbody.linearVelocity = Vector2.zero;
    myRigidbody.gravityScale = gravityScaleAtStart; // Restore gravity
    myCapsule.offset = new Vector2(0f, 0f); // Reset collider offset
    
    // Optional: Make player invulnerable for a second after respawning
    StartCoroutine(BecomeTemporarilyInvulnerable());
    
    myAnimator.SetBool("isRunning", false);
    myAnimator.Play("Idle"); // Force animation back to idle
}
    }
}