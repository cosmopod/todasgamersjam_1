using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;
using System.Collections.Generic;

public class LogicManager
{

    #region State Management

    /// <summary>
    /// Available logic states of the game
    /// </summary>
    public enum LogicStates
    {
        Playing, GameOver, Scores
        //TODO: Add more states!
    }

    private LogicStates currentState;

    /// <summary>
    /// Return the current state of the game
    /// </summary>
    public LogicStates CurrentState { get { return this.currentState; } }

    /// <summary>
    /// Changes the logic state of the game
    /// </summary>
    public void SetState(LogicStates newState)
    {
        switch (newState)
        {
            case LogicStates.Playing:
                break;
            case LogicStates.GameOver:
                break;
            case LogicStates.Scores:
                break;
            default:
                break;
                //TODO: more states!
        }

        //save the new state
        this.currentState = newState;
    }

    #endregion

    #region Initialization

    /// <summary>
    /// Initializes the Logic Manager
    /// </summary>
    public void Initialize()
    {
        //  this.currentState = LogicStates.;
    }

    #endregion

    #region Level Management
    /// <summary>
    /// Returns the instance of the current level
    /// </summary>

    private string currentLevelName;

    private string previousLevelName;

    public string PreviousLevelName { get { return this.previousLevelName; } }

    private Level currentLevel;


    /// <summary>
    /// Returns the current level name
    /// </summary>
    public string CurrentLevelName
    {
        get { return this.currentLevelName; }
    }

    /// <summary>
    /// Returns the instance of the current level
    /// </summary>
    public Level CurrentLevel { get { return this.currentLevel; } }

    /// <summary>
    /// Register a new level, typically call from the Level class
    /// </summary>
    /// <param name="newLevel"></param>
    public void SetCurrentLevel(Level newLevel)
    {
        previousLevelName = currentLevelName;
        currentLevel = newLevel;
        currentLevelName = SceneManager.GetActiveScene().name;
    }

    /// <summary>
    /// Loads a level asynchronously
    /// </summary>
    /// <param name="sceneName"></param>
    public void GoNextScene(string sceneName)
    {
       GameManager.Instance.StartCoroutine(loadLevelCoroutine(sceneName));
    }

    protected IEnumerator loadLevelCoroutine(string sceneName)
    {
        AsyncOperation async = SceneManager.LoadSceneAsync(sceneName);
        while (!async.isDone)
        {
            //do stuff
            yield return async;
        }
        Debug.Log("Loading complete");
    }

    #endregion

}
