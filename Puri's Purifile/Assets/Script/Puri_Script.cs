using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class Puri_Script : MonoBehaviour
{
    [SerializeField] float runspeed = 5f;
    [SerializeField] float jumpSpeed = 5f;
    [SerializeField] float climbSpeed = 5f;
    [SerializeField] Vector2 deathkick = new Vector2(1f, 1f);

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
 if (myCapsule.IsTouchingLayers(LayerMask.GetMask("Ground", "Climbing")) && myRigidbody.linearVelocity.y <= Mathf.Epsilon)
    {
        // Now it only sets to false when the player has landed/settled.
        myAnimator.SetBool("isJumping", false);
    }

        Run();
        FlipSprite();
        Die();
        ClimbLadder();

 
    }
        void OnMove(InputValue value)
    {
        if (!isAlive) { return; }
        moveInput = value.Get<Vector2>();
        Debug.Log(moveInput);
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
        // In Run()
if (myAnimator.GetBool("isClimbing"))
{
    myAnimator.SetBool("isRunning", false);
    return; 
}
// ... rest of Run()   
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

    // Use value.isPressed to ensure the code only runs when the button is pressed down.
    if (value.isPressed)
    {
        // 1. Set the Animator Trigger
        myAnimator.SetTrigger("Attack"); 
        
        // 2. OPTIONAL: If you need to stop horizontal movement during the attack, you can set the x-velocity to 0.
        // myRigidbody.linearVelocity = new Vector2(0f, myRigidbody.linearVelocity.y);
    }
}
    void Die()
    {
        if (myCapsule.IsTouchingLayers(LayerMask.GetMask("Enemy")) || myCapsule.IsTouchingLayers(LayerMask.GetMask("Flame")))
        {
            isAlive = false;
            //When the player dies collider is adjusted and death animation triggered
            myCapsule.offset = new Vector2(0f, 0.7f);
            myAnimator.SetTrigger("Dying");
            
            
        }
    }
}
