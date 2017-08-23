using UnityEngine;
using System.Collections;

#if UNITY_ADS
using UnityEngine.Advertisements;
#endif

public class AdsManager
{
    #region Market Services Params

    public const string GOOGLE_PLAY_ADS_ID = "";
    public const string APPLE_STORE_ADS_ID = "";

    public const string ADS_INTEGRATION_VIDEO_ID = "";
    public const string ADS_INTEGRATION_REWARDED_VIDEO = "";

    public const string ZONE_IDS_REWARDED_VIDEO = "";

    #endregion

    #region Initialization

    public void Initialize()
    {
#if UNITY_ADS
		if (Advertisement.isSupported) {
			if (Application.platform == RuntimePlatform.Android) {
				Advertisement.Initialize (GOOGLE_PLAY_ADS_ID, false);

			} else if (Application.platform == RuntimePlatform.IPhonePlayer) {
				Advertisement.Initialize (APPLE_STORE_ADS_ID, false);
			}
		}
#endif
    }

    #endregion

    #region Ads Show Methods
#if UNITY_ADS
    /// <summary>
    /// Shows a single video ad and invoke a callback function after that
    /// </summary>
    /// <returns>The simple add.</returns>
    /// <param name="callback">Callback.</param>
    public IEnumerator ShowSimpleAdd(System.Action callback)
    {

		while (!Advertisement.isInitialized || !Advertisement.IsReady (ADS_INTEGRATION_VIDEO_ID)) {
			yield return new WaitForSeconds (1f);
		}
		Advertisement.Show (ADS_INTEGRATION_VIDEO_ID, new ShowOptions {
			resultCallback = result => {
				if (callback != null)
					callback ();
			}
		});

}

/// <summary>
/// Shows a single video ad and invoke a callback function after that
/// </summary>
/// <returns>The simple add.</returns>
/// <param name="callback">Callback.</param>
/// <param name="p">P.</param>
/// <typeparam name="T">The 1st type parameter.</typeparam>
public IEnumerator ShowSimpleAdd<T>(System.Action<T> callback, T p)
    {

		while (!Advertisement.isInitialized || !Advertisement.IsReady (ADS_INTEGRATION_VIDEO_ID)) {
			yield return new WaitForSeconds (1f);
		}
		var options = new ShowOptions {resultCallback = result => {
				if (callback != null)
					callback (p);
			}
		};
		Advertisement.Show (ADS_INTEGRATION_VIDEO_ID, options);
}

/// <summary>
/// Shows an awarded video
/// </summary>
public IEnumerator ShowRewardedAd(System.Action callback)
{

		while (!Advertisement.isInitialized || !Advertisement.IsReady (ADS_INTEGRATION_REWARDED_VIDEO)) {
			yield return new WaitForSeconds (1f);
		}
		var options = new ShowOptions { resultCallback = handleShowResult => {
				if (callback != null)
					callback ();
			}
		};
		Advertisement.Show (ADS_INTEGRATION_REWARDED_VIDEO, options);

}

/// <summary>
/// Handles the result of the user's interaction with advertising
/// </summary>
/// <param name="result"></param>
private void handleShowResult(ShowResult result)
{
    switch (result)
    {
        case ShowResult.Failed:
            break;
        case ShowResult.Skipped:
            break;
        case ShowResult.Finished:
            break;
        default:
            break;
    }
}
#endif
    #endregion
}