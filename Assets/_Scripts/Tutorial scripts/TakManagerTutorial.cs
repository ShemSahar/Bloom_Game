using System.Collections.Generic;
using UnityEngine;

namespace MyGame
{
    public class TaskManager : MonoBehaviour
    {
        public static TaskManager Instance { get; private set; }

        public List<GameObject> taskObjects;  // List of objects relevant to tasks
        private int currentTaskIndex = 0;

        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
                DontDestroyOnLoad(gameObject);
            }
            else
            {
                Destroy(gameObject);
            }
        }

        private void Start()
        {
            UpdateTaskObjects();
        }

        public void CompleteTask()
        {
            currentTaskIndex++;
            UpdateTaskObjects();
        }

        private void UpdateTaskObjects()
        {
            for (int i = 0; i < taskObjects.Count; i++)
            {
                bool isActive = (i == currentTaskIndex);
                IInteractable interactable = taskObjects[i].GetComponent<IInteractable>();
                if (interactable != null)
                {
                    interactable.SetInteractable(isActive);
                }
            }
        }
    }
}
