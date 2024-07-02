using UnityEngine;
using UnityEngine.UI;

public class InteractButton : MonoBehaviour
{
    [SerializeField] private JoystickController _joystickController;

    private void Start()
    {
        // Ensure the button has an OnClick listener
        Button button = GetComponent<Button>();
        button.onClick.AddListener(OnInteractButtonClicked);
    }

    private void OnInteractButtonClicked()
    {
        // Call the interaction method on the JoystickController
        _joystickController.Interact();
    }
}
