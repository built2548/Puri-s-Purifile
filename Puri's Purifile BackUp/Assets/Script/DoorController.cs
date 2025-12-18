using UnityEngine;

public class DoorController : MonoBehaviour
{
    [SerializeField] private Animator doorAnimator;
    [SerializeField] private string openTrigger = "Open";
    [SerializeField] private string closeTrigger = "Close";

    // Reference to the physical collider that blocks bullets/player
    [SerializeField] private Collider2D physicalCollider; 

    private void Start()
    {
        // Ensure the door starts closed and blocking
        if (physicalCollider != null) physicalCollider.enabled = true;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            doorAnimator.SetTrigger(openTrigger);
            doorAnimator.ResetTrigger(closeTrigger);
            
            // OPEN: Disable the collider so bullets can pass
            if (physicalCollider != null) physicalCollider.enabled = false;
            
            Debug.Log("Door Opening - Path Clear");
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            doorAnimator.SetTrigger(closeTrigger);
            doorAnimator.ResetTrigger(openTrigger);
            
            // CLOSE: Enable the collider to block bullets
            if (physicalCollider != null) physicalCollider.enabled = true;
            
            Debug.Log("Door Closing - Path Blocked");
        }
    }
}