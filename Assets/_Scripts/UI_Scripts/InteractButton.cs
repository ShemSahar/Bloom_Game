using UnityEngine;
using UnityEngine.UI;

public class InteractButton : MonoBehaviour
{
    [SerializeField] private Transform playerTransform;
    [SerializeField] private float interactRange = 2.0f;
    [SerializeField] private LayerMask interactableLayer;

    private void Start()
    {
        // Ensure the button has an OnClick listener
        Button button = GetComponent<Button>();
        button.onClick.AddListener(OnInteractButtonClicked);

        // Check if playerTransform is assigned
        if (playerTransform == null)
        {
            Debug.LogError("Player Transform is not assigned in the InteractButton script.");
        }
    }

    private void OnInteractButtonClicked()
    {
        if (playerTransform == null) return; // Prevent interaction if playerTransform is not assigned

        Collider[] hitColliders = Physics.OverlapSphere(playerTransform.position, interactRange, interactableLayer);
        foreach (var hitCollider in hitColliders)
        {
            IInteractable interactable = hitCollider.GetComponent<IInteractable>();
            if (interactable != null)
            {
                interactable.Interact();
                break; // Only interact with one object at a time
            }
        }
    }
}
