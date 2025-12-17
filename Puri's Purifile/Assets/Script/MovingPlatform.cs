using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class MovingPlatform : MonoBehaviour
{
    public Transform posA, posB;
    public float speed;
     [Header("Audio Settings")]
     [SerializeField] private AudioSource ElevatorSource;
    // ⭐ NEW: Duration of the pause at each end.
    [SerializeField] float pauseDuration = 0.5f; 
    
    private Vector3 targetPos;

    void Start()
    {
        // Set the initial target
        targetPos = posB.position; 
        
        // ⭐ NEW: Start the main movement loop in a Coroutine
        StartCoroutine(MoveLoop());
    }
    
    // We can remove the Update() method entirely!
    
    // ⭐ NEW: Coroutine to handle movement and pausing
    IEnumerator MoveLoop()
    {
        // This loop runs continuously while the component is active
        while (true) 
        {
            if (ElevatorSource != null && !ElevatorSource.isPlaying)
            {
                ElevatorSource.Play();
            }
            // 1. Move to the target (A or B)
            while (Vector3.Distance(transform.position, targetPos) > 0.05f)
            {
                // Continue moving smoothly toward the target position
                transform.position = Vector3.MoveTowards(
                    transform.position, 
                    targetPos, 
                    speed * Time.deltaTime
                );
                
                // Wait for the next frame before continuing the loop
                yield return null; 
            }
            
            // 2. We have reached the target position (posA or posB)
            
            // ⭐ PAUSE: Wait for the specified duration
            yield return new WaitForSeconds(pauseDuration);
            
            // 3. Switch the target position
            if (targetPos == posB.position)
            {
                targetPos = posA.position;
            }
            else // targetPos must be posA.position
            {
                targetPos = posB.position;
            }
            
            // The loop repeats, starting movement toward the new target.
        }
    }
    
    // You should still include your OnTriggerEnter/Exit methods for player parenting
    // ... (Your OnTriggerEnter2D and OnTriggerExit2D methods go here) ...
}