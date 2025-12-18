using UnityEngine;
using System.Collections;

public class BulletUpgradeItem : MonoBehaviour
{
    private Animator myAnimator;
    private SpriteRenderer mySprite;
    private bool isCollected = false;

    [Header("Settings")]
    [SerializeField] float delayBeforeDestroy = 0.5f; 
    
    [Header("Flash Effect")]
    [SerializeField] Color flashColor = Color.white;
    [SerializeField] float flashDuration = 0.1f;

    private void Start()
    {
        myAnimator = GetComponent<Animator>();
        mySprite = GetComponent<SpriteRenderer>();
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player") && !isCollected)
        {
            Puri_Script player = other.GetComponent<Puri_Script>();
            if (player != null)
            {
                isCollected = true;
                StartCoroutine(CollectSequence(player));
            }
        }
    }

    IEnumerator CollectSequence(Puri_Script player)
    {
        // 1. Trigger Animation
        if (myAnimator != null) myAnimator.SetTrigger("Open"); 

        // 2. ⭐ START FLASH EFFECT
        StartCoroutine(FlashEffect());

        // 3. Logic & Sound
        player.UpgradeBullet();
        player.PlayPickupSound();

        yield return new WaitForSeconds(delayBeforeDestroy);

        Destroy(gameObject);
    }

    IEnumerator FlashEffect()
    {
        if (mySprite != null)
        {
            Color originalColor = mySprite.color;
            
            // Set to solid white (or chosen flash color)
            mySprite.color = flashColor;
            
            yield return new WaitForSeconds(flashDuration);
            
            // Return to original color
            mySprite.color = originalColor;
        }
    }
}