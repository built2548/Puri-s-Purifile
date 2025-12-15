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
    // Drag your Empty Heart Sprite here
    [SerializeField] private Sprite emptyHeartSprite; 

    // Public method called by the player script when health changes
    public void UpdateHealthDisplay(int currentLives)
    {
        // Safety check to ensure we have images and sprites assigned
        if (heartImages == null || heartImages.Count == 0 || fullHeartSprite == null || emptyHeartSprite == null)
        {
            Debug.LogError("Heart Display Manager is missing references in the Inspector!");
            return;
        }

        // Loop through all heart images
        for (int i = 0; i < heartImages.Count; i++)
        {
            // If the heart's index (i) is less than the current lives, it's a full heart.
            // Since lists are 0-indexed, index 0 is life 1, index 1 is life 2, etc.
            if (i < currentLives)
            {
                // Heart is full (Lives > Index)
                heartImages[i].sprite = fullHeartSprite;
            }
            else
            {
                // Heart is empty (Lives <= Index)
                heartImages[i].sprite = emptyHeartSprite;
            }
        }
    }
}