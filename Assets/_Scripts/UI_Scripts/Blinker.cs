using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class BlinkingImage : MonoBehaviour
{
    public Image targetImage;  // Assign the image you want to blink in the Inspector
    public Button targetButton;  // Assign the button to stop blinking when clicked
    public float blinkInterval = 0.5f;  // Time in seconds between blinks

    private bool isBlinking = true;

    private void Start()
    {
        if (targetImage == null || targetButton == null)
        {
            Debug.LogError("Target Image or Button is not assigned.");
            return;
        }

        targetButton.onClick.AddListener(OnButtonClick);
        StartCoroutine(Blink());
    }

    private void OnButtonClick()
    {
        isBlinking = false;
        targetImage.enabled = false;  // Ensure the image is off when blinking stops
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
