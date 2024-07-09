using UnityEngine;
using UnityEngine.UI;

public class SpriteSwapper : MonoBehaviour
{
    [Header("Sprites")]
    public Sprite originalSprite; // The original sprite
    public Sprite swappedSprite;  // The sprite to swap to

    private Image imageComponent; // The Image component of the UI element
    private bool isSwapped = false; // Flag to track the current sprite state

    void Start()
    {
        // Get the Image component from the GameObject
        imageComponent = GetComponent<Image>();

        // Set the initial sprite
        if (imageComponent != null)
        {
            imageComponent.sprite = originalSprite;
        }
        else
        {
            Debug.LogError("No Image component found on the GameObject.");
        }
    }

    // This method will be called when the image is clicked
    public void OnImageClick()
    {
        if (imageComponent != null)
        {
            // Swap the sprite based on the current state
            if (isSwapped)
            {
                imageComponent.sprite = originalSprite;
            }
            else
            {
                imageComponent.sprite = swappedSprite;
            }

            // Toggle the state
            isSwapped = !isSwapped;
        }
    }
}
