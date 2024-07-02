using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class JumpButton : MonoBehaviour
{
    [SerializeField] private JoystickController _joystickController;

    private void Start()
    {
        // Ensure the button has an OnClick listener
        Button button = GetComponent<Button>();
        button.onClick.AddListener(OnJumpButtonClicked);
    }

    private void OnJumpButtonClicked()
    {
        _joystickController.Jump();
    }
}
