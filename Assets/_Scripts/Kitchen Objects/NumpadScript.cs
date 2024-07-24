using System.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace MyGame
{
    public class NumpadScript : MonoBehaviour
    {
        [Header("Numpad Buttons")]
        public Button[] numpadButtons;  // Assign the 10 buttons (0-9) in the Inspector
        public Button clearButton;  // Assign the 'C' button in the Inspector
        public Button okButton;  // Assign the 'OK' button in the Inspector

        [Header("Passcode Settings")]
        public string correctPasscode = "5705";  // The correct passcode
        private string enteredPasscode = "";  // The currently entered passcode
        private bool isPasscodeCorrect = false;  // Track if the passcode is correct

        [Header("Display Settings")]
        public Image[] asterisks;  // Assign the 4 Image objects representing asterisks in the Inspector
        private const int passcodeLength = 4;  // The length of the passcode
        private Color defaultColor = Color.white;  // Default color of the asterisks

        private void Start()
        {
            // Assign button listeners
            for (int i = 0; i < numpadButtons.Length; i++)
            {
                int index = i;  // Capture the current value of i
                numpadButtons[index].onClick.AddListener(() => OnNumberButtonClick(index));
            }

            clearButton.onClick.AddListener(OnClearButtonClick);
            okButton.onClick.AddListener(OnOkButtonClick);

            // Initialize display
            UpdateDisplay();

            // Store the default color of the asterisks
            if (asterisks.Length > 0 && asterisks[0] != null)
            {
                defaultColor = asterisks[0].color;
            }

            // Initially hide all asterisks
            for (int i = 0; i < asterisks.Length; i++)
            {
                if (asterisks[i] != null)
                {
                    asterisks[i].enabled = false;
                }
            }
        }

        private void OnNumberButtonClick(int number)
        {
            if (enteredPasscode.Length < passcodeLength)
            {
                enteredPasscode += number.ToString();
                UpdateDisplay();
            }
        }

        private void OnClearButtonClick()
        {
            enteredPasscode = "";
            UpdateDisplay();
        }

        private void OnOkButtonClick()
        {
            if (enteredPasscode == correctPasscode)
            {
                isPasscodeCorrect = true;
                ShowFeedback(true);
                StartCoroutine(TriggerDoorUnlock());
            }
            else
            {
                ShowFeedback(false);
                StartCoroutine(ResetAsterisksAfterDelay(1.0f));  // Reset the asterisks after a delay
            }
        }

        private void UpdateDisplay()
        {
            for (int i = 0; i < passcodeLength; i++)
            {
                if (i < enteredPasscode.Length)
                {
                    if (asterisks[i] != null)
                    {
                        asterisks[i].enabled = true;
                        asterisks[i].color = defaultColor;  // Set the color to default for each input
                    }
                }
                else
                {
                    if (asterisks[i] != null)
                    {
                        asterisks[i].enabled = false;
                    }
                }
            }
        }

        private void ShowFeedback(bool isCorrect)
        {
            Color feedbackColor = isCorrect ? Color.green : Color.red;

            for (int i = 0; i < passcodeLength; i++)
            {
                if (i < enteredPasscode.Length && asterisks[i] != null)
                {
                    asterisks[i].color = feedbackColor;
                }
            }

            if (isCorrect)
            {
                // Keep the asterisks green if correct
                enteredPasscode = "";
                okButton.interactable = false;
            }
            else
            {
                // Turn the asterisks red and then turn off after a delay
                enteredPasscode = "";
            }
        }

        private IEnumerator ResetAsterisksAfterDelay(float delay)
        {
            yield return new WaitForSeconds(delay);

            for (int i = 0; i < passcodeLength; i++)
            {
                if (asterisks[i] != null)
                {
                    asterisks[i].color = defaultColor;
                    asterisks[i].enabled = false;
                }
            }
            enteredPasscode = "";
        }

        private IEnumerator TriggerDoorUnlock()
        {
            yield return new WaitForSeconds(2.0f);
            if (isPasscodeCorrect)
            {
                DoorWithPasscodeScript doorScript = FindObjectOfType<DoorWithPasscodeScript>();
                if (doorScript != null)
                {
                    doorScript.UnlockDoor();
                }
            }
        }

        public bool IsPasscodeCorrect()
        {
            return isPasscodeCorrect;
        }
    }
}
