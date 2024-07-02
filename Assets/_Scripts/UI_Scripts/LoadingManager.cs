using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.Collections;

public class LoadingManager : MonoBehaviour
{
    public Image progressBar;
    public string sceneToLoad;
    public float minimumLoadingTime = 5f;

    void Start()
    {
        StartCoroutine(LoadSceneAsync(sceneToLoad));
    }

    IEnumerator LoadSceneAsync(string sceneName)
    {
        AsyncOperation operation = SceneManager.LoadSceneAsync(sceneName);
        operation.allowSceneActivation = false;

        float elapsedTime = 0f;

        while (!operation.isDone)
        {
            // Update the progress bar
            float progress = Mathf.Clamp01(operation.progress / 0.9f);
            progressBar.fillAmount = progress;

            // Check if the load has finished and if the minimum loading time has passed
            if (operation.progress >= 0.9f && elapsedTime >= minimumLoadingTime)
            {
                // Wait a frame to ensure the progress bar is updated
                yield return null;
                // Activate the scene
                operation.allowSceneActivation = true;
            }

            elapsedTime += Time.deltaTime;
            yield return null;
        }
    }
}
