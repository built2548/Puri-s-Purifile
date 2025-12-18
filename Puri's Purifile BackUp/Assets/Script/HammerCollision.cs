// NEW/REVISED SCRIPT: HammerCollision.cs (Attach to Child Damage Zone)
using UnityEngine;

public class HammerCollision : MonoBehaviour
{
    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            Puri_Script player = other.GetComponent<Puri_Script>();
            if (player != null)
            {
                player.TakeDamage(); 
            }
        }
    }
}