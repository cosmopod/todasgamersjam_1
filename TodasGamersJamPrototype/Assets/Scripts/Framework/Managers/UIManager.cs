using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

public class UIManager
{

    private const string UI_ROOT_GAMEOBJECT = "";
    private const string UI_SCENE_NAME = "";

    #region "Initialization"

    /// <summary>
    /// Initializes the Logic Manager
    /// </summary>
    public void Initialize()
    {

    }

    /// <summary>
    /// Loads the UI scene additively if it's not loaded yet
    /// </summary>
    private void injectUIScene()
    {
        //if the UI is not found
        if (GameObject.Find(UI_ROOT_GAMEOBJECT) == null)
        {
            //additivelly load the scene that contains the UI gameobjects
            SceneManager.LoadScene(UI_SCENE_NAME, LoadSceneMode.Additive);
        }
        //waits until the UI has been loaded
        GameManager.Instance.StartCoroutine(waitUntilReady());

    }

    /// <summary>
    /// Waits until the UI has been loaded by the engine
    /// </summary>
    private IEnumerator waitUntilReady()
    {
        //waits until the uiRoot is actually loaded and available
        GameObject uiRoot = null;
        do
        {
            uiRoot = GameObject.Find(UI_ROOT_GAMEOBJECT);
            yield return new WaitForEndOfFrame();
        }
        while (uiRoot == null);
    }

    #endregion

    #region UI Async Injection
    /// <summary>
    /// Calls the async UI injection coroutine
    /// </summary>
    public void InjectUIAsync(string sceneName)
    {
        GameManager.Instance.StartCoroutine(loadSceneAsyncAdditive(sceneName));
    }

    /// <summary>
    /// Loads asynchronously the ui scene
    /// </summary>
    /// <returns></returns>
    IEnumerator loadSceneAsyncAdditive(string sceneName)
    {
        Debug.Log(sceneName);
        if (Application.CanStreamedLevelBeLoaded(sceneName))
        {
            AsyncOperation async = SceneManager.LoadSceneAsync(sceneName, LoadSceneMode.Additive);
            yield return async;
        }
        else
        {
            Debug.LogWarning("No existe la escena solicitada");
        }
    }

    #endregion
}
