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
    public TMP_Text mission6Text;  // Text component for mission 6

    private int currentMissionIndex = 0;

    private void Start()
    {
        missionListPanel.SetActive(false);
        missionListButton.onClick.AddListener(ToggleMissionList);
        UpdateMissionListUI();
    }

    public void ToggleMissionList()
    {
        missionListPanel.SetActive(!missionListPanel.activeSelf);
    }

    public void CompleteMission()
    {
        if (currentMissionIndex < 6)
        {
            GetMissionText(currentMissionIndex).color = Color.gray;  // Mark current mission as completed
            currentMissionIndex++;
            if (currentMissionIndex < 6)
            {
                GetMissionText(currentMissionIndex).gameObject.SetActive(true);  // Reveal the next mission
            }
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
            case 5: return mission6Text;
            default: return null;
        }
    }

    private void UpdateMissionListUI()
    {
        mission1Text.gameObject.SetActive(true);
        mission2Text.gameObject.SetActive(false);
        mission3Text.gameObject.SetActive(false);
        mission4Text.gameObject.SetActive(false);
        mission5Text.gameObject.SetActive(false);
        mission6Text.gameObject.SetActive(false);
    }
}
