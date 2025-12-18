
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using FirstGearGames.SmoothCameraShaker;

public class Puri_Script : MonoBehaviour
{
    [Header("Bullet Levels")]
    [SerializeField] private GameObject[] bulletPrefabs; // Slot 0 = Lv1, Slot 1 = Lv2, Slot 2 = Lv3
    private int bulletLevel = 0;

    [Header("Audio Child References")]
    [SerializeField] private AudioSource jumpSource;
    [SerializeField] private AudioSource shootSource;
    [SerializeField] private AudioSource hitSource;
    [SerializeField] private AudioSource pickupSource;
    [SerializeField] private AudioSource deathSource;

    [Header("Ground Check")]
    [SerializeField] float groundCheckDistance = 0.1f;
    [SerializeField] LayerMask whatIsGround;

    [Header("Player Stats")]
    [SerializeField] int lives = 3;
    public int Lives { get { return lives; } }
    [SerializeField] float invulnerableDuration = 1.5f;
    private bool isInvulnerable = false;

    [Header("Respawn Logic")]
    private Vector3 currentCheckpoint;

    [Header("Shooting")]
    [SerializeField] float projectileSpeed = 15f;
    [SerializeField] float fireRate = 0.5f;
    private float nextFireTime;

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
    private HeartDisplayManager heartDisplayManager;

    void Awake()
    {
        heartDisplayManager = FindObjectOfType<HeartDisplayManager>();
        if (heartDisplayManager == null)
        {
            Debug.LogError("HeartDisplayManager not found!");
        }
    }

    void Start()
    {
        myRigidbody = GetComponent<Rigidbody2D>();
        originalScale = new Vector2(transform.localScale.x, transform.localScale.y);
        myAnimator = GetComponent<Animator>();
        myCapsule = GetComponent<CapsuleCollider2D>();
        gravityScaleAtStart = myRigidbody.gravityScale;

        // Initial Checkpoint is the starting position
        currentCheckpoint = transform.position;

        if (heartDisplayManager != null)
        {
            heartDisplayManager.UpdateHealthDisplay(lives);
        }
    }

    void Update()
    {
        if (!isAlive) { return; }

        // Reset jumping animation when touching ground
        if (myCapsule.IsTouchingLayers(LayerMask.GetMask("Ground", "Climbing", "MovingPlatform")) && Mathf.Abs(myRigidbody.linearVelocity.y) <= Mathf.Epsilon)
        {
            myAnimator.SetBool("isJumping", false);
        }

        Run();
        FlipSprite();
        CheckForDeath();
        ClimbLadder();
    }

    // --- CHECKPOINT LOGIC ---
    public void UpdateCheckpoint(Vector3 newPos)
    {
        currentCheckpoint = newPos;
        Debug.Log("Checkpoint Saved!");
    }

    // --- INPUT ACTIONS ---
    void OnMove(InputValue value)
    {
        if (!isAlive) { return; }
        moveInput = value.Get<Vector2>();
    }

    void OnJump(InputValue value)
    {
        if (!isAlive) { return; }
        if (!IsGrounded()) { return; }

        if (value.isPressed)
        {
            myRigidbody.linearVelocity = new Vector2(myRigidbody.linearVelocity.x, jumpSpeed);
            myAnimator.SetBool("isJumping", true);
            if (jumpSource != null) jumpSource.Play();
        }
    }

    void OnAttack(InputValue value)
    {
        if (!isAlive) { return; }
        if (value.isPressed && Time.time >= nextFireTime)
        {
            myAnimator.SetTrigger("Attack");
            Shoot();
            nextFireTime = Time.time + fireRate;
        }
    }

    // --- MOVEMENT METHODS ---
    bool IsGrounded()
    {
        Vector2 raycastOrigin = (Vector2)myCapsule.bounds.center + Vector2.down * (myCapsule.size.y / 2f);
        RaycastHit2D hit = Physics2D.Raycast(raycastOrigin, Vector2.down, groundCheckDistance, whatIsGround);
        return hit.collider != null;
    }

    void Run()
    {
        if (myAnimator.GetBool("isClimbing"))
        {
            myAnimator.SetBool("isRunning", false);
            return;
        }
        myRigidbody.linearVelocity = new Vector2(moveInput.x * runspeed, myRigidbody.linearVelocity.y);
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

        myRigidbody.linearVelocity = new Vector2(myRigidbody.linearVelocity.x, moveInput.y * climbSpeed);
        myRigidbody.gravityScale = 0f;
        bool playerHasVerticalSpeed = Mathf.Abs(myRigidbody.linearVelocity.y) > Mathf.Epsilon;
        myAnimator.SetBool("isClimbing", playerHasVerticalSpeed);
    }

    // --- COMBAT METHODS ---
    void Shoot()
    {
        if (bulletPrefabs == null || bulletLevel >= bulletPrefabs.Length || bulletPrefabs[bulletLevel] == null) return;

        if (shootSource != null)
        {
            shootSource.pitch = Random.Range(0.9f, 1.1f);
            shootSource.Play();
        }

        float direction = Mathf.Sign(transform.localScale.x);
        Vector3 spawnPos = transform.position + new Vector3(direction * 0.8f, 0, 0);

        GameObject projectile = Instantiate(bulletPrefabs[bulletLevel], spawnPos, Quaternion.identity);
        PlayerProjectile projScript = projectile.GetComponent<PlayerProjectile>();
        if (projScript != null)
        {
            projScript.Initialize(direction);
        }
    }

    public void UpgradeBullet()
    {
        if (bulletLevel + 1 < bulletPrefabs.Length)
        {
            bulletLevel++;
            PlayPickupSound();
        }
    }

    public void ResetBulletLevel()
    {
        bulletLevel = 0;
    }

    public void PlayPickupSound()
    {
        if (pickupSource != null) pickupSource.Play();
    }

    // --- HEALTH & DEATH LOGIC ---
    void CheckForDeath()
    {
        if (!isInvulnerable && (myCapsule.IsTouchingLayers(LayerMask.GetMask("Flame"))))
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
            StartCoroutine(HandleDeath(true));
        }
        else
        {
            StartCoroutine(HandleDeath(false)); // Respawn at checkpoint
        }
    }

    public void KillInstantly()
    {
        if (!isAlive) return;
        lives = 0;
        if (heartDisplayManager != null) heartDisplayManager.UpdateHealthDisplay(0);
        StartCoroutine(HandleDeath(true));
    }

    IEnumerator BecomeTemporarilyInvulnerable()
    {
        isInvulnerable = true;
        SpriteRenderer spriteRenderer = GetComponent<SpriteRenderer>();
        float flashInterval = 0.1f;
        float elapsed = 0f;

        while (elapsed < invulnerableDuration)
        {
            spriteRenderer.enabled = !spriteRenderer.enabled;
            yield return new WaitForSeconds(flashInterval);
            elapsed += flashInterval;
        }

        spriteRenderer.enabled = true;
        isInvulnerable = false;
    }

    IEnumerator HandleDeath(bool isGameOver)
    {
        isAlive = false;
        if (deathSource != null) deathSource.Play();

        myAnimator.SetTrigger("Dying");
        myCapsule.offset = new Vector2(0f, 0.7f);
        myRigidbody.linearVelocity = new Vector2(deathkick.x * -transform.localScale.x, deathkick.y);

        yield return new WaitForSeconds(1.5f);

        if (isGameOver)
        {
            Debug.Log("GAME OVER!");
            // SceneManager.LoadScene(SceneManager.GetActiveScene().name); 
        }
        else
        {
            // RESPAWN AT CHECKPOINT
            transform.position = currentCheckpoint;
            isAlive = true;

            // Reset Physics
            myRigidbody.linearVelocity = Vector2.zero;
            myRigidbody.gravityScale = gravityScaleAtStart;
            myCapsule.offset = Vector2.zero;

            // Reset Visuals
            myAnimator.Play("Idle");
            myAnimator.SetBool("isRunning", false);
            myAnimator.SetBool("isJumping", false);

            StartCoroutine(BecomeTemporarilyInvulnerable());
        }
    }
}