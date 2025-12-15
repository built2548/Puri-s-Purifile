using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class HeartDisplayManager : MonoBehaviour
{
    [Header("UI References")]
    // Drag your 3 Image components (Heart1, Heart2, Heart3) here in the Inspector
    [SerializeField] private List<Image> heartImages; 

    [Header("Sprites")]
    // Drag your Full Heart Sprite here
    [SerializeField] private Sprite fullHeartSprite; 
    
    // NOTE: The 'emptyHeartSprite' is removed as requested.

    // Public method called by the player script when health changes
    public void UpdateHealthDisplay(int currentLives)
    {
        // Safety check to ensure we have images and sprites assigned
        if (heartImages == null || heartImages.Count == 0 || fullHeartSprite == null)
        {
            Debug.LogError("Heart Display Manager is missing references in the Inspector (heartImages or fullHeartSprite)!");
            return;
        }

        // Loop through all heart images
        for (int i = 0; i < heartImages.Count; i++)
        {
            // Safety check for the specific heart image
            if (heartImages[i] == null) continue;

            // If the heart's index (i) is less than the current lives, it's a full heart and should be VISIBLE.
            if (i < currentLives)
            {
                // Ensure the heart has the correct sprite and is visible/active
                heartImages[i].sprite = fullHeartSprite;
                heartImages[i].enabled = true; // Use 'enabled = true' to show the Image component
            }
            else
            {
                // Heart is lost (Lives <= Index) and should be HIDDEN.
                // We simply disable the Image component, which is cleaner than setting the sprite to null.
                heartImages[i].enabled = false; 
            }
        }
    }
}