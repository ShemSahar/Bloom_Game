using UnityEngine;

public class QuestManager : MonoBehaviour
{
    public static QuestManager instance;  // Singleton instance

    public float totalWaterNeeded = 30f;  // Total amount of water needed to complete the quest
    public float currentWaterLevel = 0f;  // Current total amount of water collected

    void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);  // Optional: Makes the manager persist across scenes
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void AddWater(float amount)
    {
        currentWaterLevel += amount;
        Debug.Log("Water added. Current water level: " + currentWaterLevel);

        if (currentWaterLevel >= totalWaterNeeded)
        {
            Debug.Log("Quest Completed! You have drunk enough water.");
            CompleteQuest();
        }
    }

    private void CompleteQuest()
    {
        // Here you can add any actions that happen when the quest is completed
        // For example, unlocking a new level, giving a reward, etc.
        Debug.Log("Congratulations! Quest completed.");
    }
}
