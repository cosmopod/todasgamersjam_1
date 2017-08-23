using UnityEngine;
using System.Collections;

public class Level : MonoBehaviour
{
    #region Default Component Methods

    public SceneHUD.HUDs hudScene;

    void Awake()
    {
        //Initializes the game manager
        GameManager.Instance.Initialize();

        string hudSceneName = SceneHUD.GetHUDSceneName(hudScene);

        //Inform the Manager we are the new level
        GameManager.Instance.UI.InjectUIAsync(hudSceneName);

        //Drains the pool if it contains audio objects
        GameManager.Instance.Audio.DrainPool();
    }

    void Start()
    {
        registerCatalogs();
    }

    // Update is called once per frame
    void Update()
    {
        //  GameManager.GetInstance().Update();
    }
    #endregion

    #region Audio
    /// <summary>
    /// Add the Audiocatalogs required by this level to the AudioManager
    /// </summary>
    public AudioCatalog[] AudioCatalogs;

    /// <summary>
    /// Inform the Audio manager of the Audio Catalogs required by this level
    /// </summary>
    private void registerCatalogs()
    {
        if ((AudioCatalogs != null) && (AudioCatalogs.Length > 0))
            for (int i = 0; i < AudioCatalogs.Length; i++)
            {
                GameManager.Instance.Audio.RegisterCatalog(AudioCatalogs[i]);
            }
    }
    #endregion

}
