using UnityEngine;

public class Checkpoint : MonoBehaviour
{
    [SerializeField] private AudioSource checkpointSound; // Optional child sound
    [SerializeField] private Sprite activeSprite; // Sprite for when the flag is "on"
    private SpriteRenderer sr;
    private bool isActivated = false;

    private void Start()
    {
        sr = GetComponent<SpriteRenderer>();
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player") && !isActivated)
        {
            Puri_Script player = other.GetComponent<Puri_Script>();
            if (player != null)
            {
                // Set this checkpoint as the player's new respawn point
                player.UpdateCheckpoint(transform.position);
                ActivateCheckpoint();
            }
        }
    }

    private void ActivateCheckpoint()
    {
        isActivated = true;
        if (sr != null && activeSprite != null) sr.sprite = activeSprite;
        if (checkpointSound != null) checkpointSound.Play();
        Debug.Log("Checkpoint Activated!");
    }
}