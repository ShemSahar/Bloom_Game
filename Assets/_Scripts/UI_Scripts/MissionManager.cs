using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class MissionManager : MonoBehaviour
{
    public GameObject missionListPanel;  // Panel that contains the mission list
    public Button missionListButton;  // Button to open/close the mission list
    public Animator missionListAnimator; // Animator component for mission list panel
    public List<TMP_Text> missionTexts;  // List of Text components for the missions

    public Mission[] missions; // Array to hold all missions

    private int currentMissionIndex = 0;
    private bool isMissionListVisible = false;

    private void Start()
    {
        if (missionListPanel == null)
        {
            Debug.LogError("missionListPanel is not assigned in the Inspector.");
            return;
        }

        if (missionListButton == null)
        {
            Debug.LogError("missionListButton is not assigned in the Inspector.");
            return;
        }

        if (missionListAnimator == null)
        {
            Debug.LogError("missionListAnimator is not assigned in the Inspector.");
            return;
        }

        if (missionTexts == null || missionTexts.Count == 0)
        {
            Debug.LogError("missionTexts is not assigned or is empty in the Inspector.");
            return;
        }

        if (missions == null || missions.Length == 0)
        {
            Debug.LogError("missions array is not assigned or is empty in the Inspector.");
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
            TMP_Text missionText = GetMissionText(currentMissionIndex);
            if (missionText != null)
            {
                missionText.color = Color.gray;  // Mark current mission as completed
            }
            else
            {
                Debug.LogError("Mission text at index " + currentMissionIndex + " is null.");
            }

            missions[currentMissionIndex].CompleteMission();
            currentMissionIndex++;
            if (currentMissionIndex < missions.Length)
            {
                TMP_Text nextMissionText = GetMissionText(currentMissionIndex);
                if (nextMissionText != null)
                {
                    nextMissionText.gameObject.SetActive(true);  // Reveal the next mission
                }
                else
                {
                    Debug.LogError("Next mission text at index " + currentMissionIndex + " is null.");
                }
            }
            UpdateMissionState();
        }
    }

    private TMP_Text GetMissionText(int index)
    {
        if (index >= 0 && index < missionTexts.Count)
        {
            return missionTexts[index];
        }
        return null;
    }

    private void InitializeMissionList()
    {
        for (int i = 0; i < missionTexts.Count; i++)
        {
            missionTexts[i].gameObject.SetActive(i == 0);  // Only activate the first mission text initially
        }
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
    private Outline interactableOutline;

    public void SetInteractable(bool state)
    {
        if (interactableObject != null)
        {
            interactableScript = interactableObject.GetComponent<MonoBehaviour>();
            if (interactableScript != null)
            {
                interactableScript.enabled = state;
            }
            else
            {
                Debug.LogError("Interactable script not found on " + interactableObject.name);
            }

            interactableOutline = interactableObject.GetComponent<Outline>();
            if (interactableOutline != null)
            {
                interactableOutline.enabled = state;
            }
            else
            {
                Debug.LogError("Outline component not found on " + interactableObject.name);
            }
        }
        else
        {
            Debug.LogError("Interactable object is not assigned for mission: " + missionName);
        }
    }

    public void CompleteMission()
    {
        // Logic to mark mission as complete (e.g., updating UI, saving progress, etc.)
    }
}
