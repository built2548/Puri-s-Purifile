using UnityEngine;

public class DoorController : MonoBehaviour
{
    [SerializeField] private Animator doorAnimator;
    [SerializeField] private string openTrigger = "Open";
    [SerializeField] private string closeTrigger = "Close";

    // Detect when player enters the door's trigger area
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            doorAnimator.SetTrigger(openTrigger);
            doorAnimator.ResetTrigger(closeTrigger); // Prevent conflicting triggers
            Debug.Log("Door Opening");
        }
    }

    // Detect when player leaves the door's trigger area
    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            doorAnimator.SetTrigger(closeTrigger);
            doorAnimator.ResetTrigger(openTrigger);
            Debug.Log("Door Closing");
        }
    }
}