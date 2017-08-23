using UnityEngine;
using System.Collections;
using System;

public class InputManager
{
    #region "Initialization"
    public void Initialize()
    {

    }

    #endregion

    #region "Update"

    public void Update()
    {
        bool isPlaying = GameManager.Instance.Logic.CurrentState == LogicManager.LogicStates.Playing;
        if (isPlaying)
        {
#if  UNITY_WEBPLAYER || UNITY_EDITOR	
            processKeyBoardInputs();
#elif UNITY_IOS || UNITY_ANDROID || UNITY_WP8 || UNITY_IPHONE
            processMobileInputs();
#endif
        }
    }
    #endregion

    #region Input process

    /// <summary>
    /// Process the keyboard Inputs
    /// </summary>
    public void processKeyBoardInputs()
    {

    }

    #endregion

    #region Touch process
    /// <summary>
    /// Process the mobile screen inputs
    /// </summary>
    public void processMobileInputs()
    {

    }

    #endregion

}
