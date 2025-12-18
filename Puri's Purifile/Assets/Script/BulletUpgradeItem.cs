using UnityEngine;

public class BulletUpgradeItem : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            Puri_Script player = other.GetComponent<Puri_Script>();
            if (player != null)
            {
                player.UpgradeBullet(); // Call the upgrade method we made
                player.PlayPickupSound();
            }

            Destroy(gameObject);
        }
    }
}