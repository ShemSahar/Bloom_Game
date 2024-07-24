using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class BlinkingImage : MonoBehaviour
{
    public Image targetImage;  // Assign the image you want to blink in the Inspector
    public Button targetButton;  // Assign the button to stop blinking when clicked
    public float blinkInterval = 0.5f;  // Time in seconds between blinks
    public Image tintImage;  // Assign the tint image in the Inspector
    public TextMeshProUGUI targetText;  // Assign the TextMeshProUGUI object in the Inspector
    public Image anotherImage;  // Assign the additional image in the Inspector

    private bool isBlinking = true;

    private void Start()
    {
        if (targetImage == null || targetButton == null || tintImage == null || targetText == null || anotherImage == null)
        {
            Debug.LogError("Target Image, Button, Tint Image, Target Text, or Another Image is not assigned.");
            return;
        }

        tintImage.enabled = true;  // Show the tint image initially
        targetButton.onClick.AddListener(OnButtonClick);
        StartCoroutine(Blink());
    }

    private void OnButtonClick()
    {
        isBlinking = false;
        targetImage.enabled = false;  // Ensure the image is off when blinking stops
        tintImage.enabled = false;  // Turn off the tint image
        targetText.enabled = false;  // Turn off the text
        anotherImage.enabled = false;  // Turn off the additional image
    }

    private IEnumerator Blink()
    {
        while (isBlinking)
        {
            targetImage.enabled = !targetImage.enabled;
            yield return new WaitForSeconds(blinkInterval);
        }
    }
}
