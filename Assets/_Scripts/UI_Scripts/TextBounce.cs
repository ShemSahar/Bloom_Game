using UnityEngine;
using TMPro;

public class BouncingText : MonoBehaviour
{
    public TextMeshProUGUI targetText;  // Assign the TextMeshProUGUI object in the Inspector
    public Animator textAnimator;  // Assign the Animator component in the Inspector

    private void Start()
    {
        if (targetText == null || textAnimator == null)
        {
            Debug.LogError("Target Text or Animator is not assigned.");
            return;
        }

        // Start the animation if needed, you can also control this from your Unity animation setup
    }
}
