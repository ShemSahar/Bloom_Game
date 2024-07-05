using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class MissionManager : MonoBehaviour
{
    public GameObject missionListPanel;  // Panel that contains the mission list
    public Button missionListButton;  // Button to open/close the mission list
    public TMP_Text mission1Text;  // Text component for mission 1
    public TMP_Text mission2Text;  // Text component for mission 2
    public TMP_Text mission3Text;  // Text component for mission 3
    public TMP_Text mission4Text;  // Text component for mission 4
    public TMP_Text mission5Text;  // Text component for mission 5
    public Animator missionListAnimator; // Animator component for mission list panel

    public Mission[] missions; // Array to hold all missions

    private int currentMissionIndex = 0;
    private bool isMissionListVisible = false;

    private void Start()
    {
        if (missionListPanel == null || missionListButton == null || mission1Text == null ||
            mission2Text == null || mission3Text == null || mission4Text == null ||
            mission5Text == null || missionListAnimator == null)
        {
            Debug.LogError("One or more required references are not assigned in the Inspector.");
            return;
        }

        missionListButton.onClick.AddListener(ToggleMissionList);
        InitializeMissionList();
        UpdateMissionState();
    }

    public void ToggleMissionList()
    {
        isMissionListVisible = !isMissionListVisible;
        missionListAnimator.SetTrigger("Show");
    }

    public void CompleteMission()
    {
        if (currentMissionIndex < missions.Length)
        {
            GetMissionText(currentMissionIndex).color = Color.gray;  // Mark current mission as completed
            missions[currentMissionIndex].CompleteMission();
            currentMissionIndex++;
            if (currentMissionIndex < missions.Length)
            {
                GetMissionText(currentMissionIndex).gameObject.SetActive(true);  // Reveal the next mission
            }
            UpdateMissionState();
        }
    }

    private TMP_Text GetMissionText(int index)
    {
        switch (index)
        {
            case 0: return mission1Text;
            case 1: return mission2Text;
            case 2: return mission3Text;
            case 3: return mission4Text;
            case 4: return mission5Text;
            default: return null;
        }
    }

    private void InitializeMissionList()
    {
        mission1Text.gameObject.SetActive(true);
        mission2Text.gameObject.SetActive(false);
        mission3Text.gameObject.SetActive(false);
        mission4Text.gameObject.SetActive(false);
        mission5Text.gameObject.SetActive(false);
    }

    private void UpdateMissionState()
    {
        for (int i = 0; i < missions.Length; i++)
        {
            if (i == currentMissionIndex)
            {
                missions[i].SetInteractable(true);
            }
            else
            {
                missions[i].SetInteractable(false);
            }
        }
    }
}

[System.Serializable]
public class Mission
{
    public string missionName;
    public GameObject interactableObject;
    private MonoBehaviour interactableScript;

    public void SetInteractable(bool state)
    {
        if (interactableObject != null)
        {
            interactableScript = interactableObject.GetComponent<MonoBehaviour>();
            if (interactableScript != null)
            {
                interactableScript.enabled = state;
            }
        }
    }

    public void CompleteMission()
    {
        // Logic to mark mission as complete (e.g., updating UI, saving progress, etc.)
    }
}
