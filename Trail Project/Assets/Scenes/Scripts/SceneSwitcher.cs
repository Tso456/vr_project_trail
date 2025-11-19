using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class SceneSwitcher : MonoBehaviour
{
    string[] scenes = { "POI_1_Alex", "POI_2_Heath", "POI_3_Ray" };

    ///<summary>
    /// Switches to the scene based on the provided index.
    /// Provided Index is supposed to be the number of the scene in the list (1-based index).
    /// </summary>
    public void switchScene(int sceneIndex)
    {
        if ((sceneIndex - 1) >= 0 && (sceneIndex - 1) < scenes.Length)
        {
            StartCoroutine(LoadSceneAsync(scenes[sceneIndex - 1]));
        }
        else
        {
            Debug.LogError("Scene index out of range");
        }
    }

    private IEnumerator LoadSceneAsync(string sceneName)
    {
        AsyncOperation operation = SceneManager.LoadSceneAsync(sceneName);
        
        while (!operation.isDone)
        {
            // Show loading progress if needed
            float progress = operation.progress;
            yield return null;
        }
    }
}
