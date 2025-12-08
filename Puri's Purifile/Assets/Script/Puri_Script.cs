using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class Puri_Script : MonoBehaviour
{
    [Header("Player Stats")]
    [SerializeField] int lives = 3;
    public int Lives { get { return lives; } } 
    // 💡 NEW: Duration player is invulnerable after a hit
    [SerializeField] float invulnerableDuration = 1.5f; 
    private bool isInvulnerable = false; // 💡 NEW: Flag to block damage

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
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
      myRigidbody = GetComponent<Rigidbody2D>();  
      originalScale = new Vector2 (transform.localScale.x, transform.localScale.y);
      myAnimator = GetComponent<Animator>();
      myCapsule = GetComponent<CapsuleCollider2D>();
      gravityScaleAtStart = myRigidbody.gravityScale; 
    }

    // Update is called once per frame
    void Update()
    {
        if (!isAlive) { return; }

        if (myCapsule.IsTouchingLayers(LayerMask.GetMask("Ground", "Climbing")) && Mathf.Abs(myRigidbody.linearVelocity.y) <= Mathf.Epsilon)
        {
            myAnimator.SetBool("isJumping", false);
        }

        Run();
        FlipSprite();
        CheckForDeath(); 
        ClimbLadder();
    }

    // ... (OnMove, OnJump, Run, FlipSprite, ClimbLadder, OnAttack are unchanged) ...
    void OnMove(InputValue value)
    {
        if (!isAlive) { return; }
        moveInput = value.Get<Vector2>();
    }
    
    void OnJump(InputValue value)
    {
        if (!isAlive) { return; }
        if (!myCapsule.IsTouchingLayers(LayerMask.GetMask("Ground")))
        {
            return;
        }
        if (value.isPressed )
        {
            myRigidbody.linearVelocity += new Vector2(0f, jumpSpeed);
            myAnimator.SetBool("isJumping", true);
        } 
    }

    void Run()
    {
        if (myAnimator.GetBool("isClimbing"))
        {
            myAnimator.SetBool("isRunning", false);
            return; 
        }
        Vector2 playerVelocity = new Vector2 (moveInput.x * runspeed, myRigidbody.linearVelocity.y);
        myRigidbody.linearVelocity = playerVelocity;
        bool playerHasHorizontalSpeed = Mathf.Abs(myRigidbody.linearVelocity.x) > Mathf.Epsilon;
        myAnimator.SetBool("isRunning", playerHasHorizontalSpeed);
    }

    void FlipSprite()
    {
        bool playerHasHorizontalSpeed = Mathf.Abs(myRigidbody.linearVelocity.x) > Mathf.Epsilon;
        if (playerHasHorizontalSpeed)
        {
            transform.localScale = new Vector2 (Mathf.Sign(myRigidbody.linearVelocity.x), 1f) * originalScale;
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
            myAnimator.SetTrigger("Attack"); 
        }
    }

    // --- LIVES AND DEATH LOGIC ---

    void CheckForDeath()
    {
        // 💡 ONLY CALL TakeDamage IF NOT INVULNERABLE
        if (!isInvulnerable && myCapsule.IsTouchingLayers(LayerMask.GetMask("Enemy", "Flame")))
        {
            TakeDamage();
        }
    }

    public void TakeDamage() 
    {
        if (!isAlive || isInvulnerable) return; // 💡 Double check protection

        // 1. Reduce a life
        lives--;
        Debug.Log("Player hit! Lives remaining: " + lives);

        // 2. Start the temporary invulnerability period
        StartCoroutine(BecomeTemporarilyInvulnerable());

        // 3. Check for game over
        if (lives <= 0)
        {
            StartCoroutine(HandleDeath(true)); // True means final death
        }
        else
        {
            // You might want a slight knockback/damage animation here if not dying.
            // StartCoroutine(HandleDeath(false)); // Use this line if you want the full 'death' animation/kick per hit.
            // If you only want lives to decrease without the death animation/respawn:
        }
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
            // 💡 If the enemy killed the player (lives=0), the code below will not run.
            // If you decide to use HandleDeath(false) for every hit, this is your respawn logic:
            
            isAlive = true;
            myCapsule.offset = new Vector2(0f, 0f); 
            myRigidbody.linearVelocity = Vector2.zero;
            myRigidbody.gravityScale = gravityScaleAtStart;
            transform.position = new Vector3(0, 0, 0); 
            myAnimator.SetBool("isRunning", false);
            myAnimator.SetBool("isClimbing", false);
            myAnimator.SetBool("isJumping", false);
        }
    }
}