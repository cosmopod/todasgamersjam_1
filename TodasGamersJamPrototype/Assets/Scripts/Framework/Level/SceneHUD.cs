using UnityEngine;
using System.Collections;

public static class SceneHUD
{
    public enum HUDs { playing }

    public static string GetHUDSceneName(HUDs HUDScene)
    {
        string name = null;
        switch (HUDScene)
        {
            case HUDs.playing:
                name = "UIMain";
                break;

            default:
                name = "";
                break;
        }
        return name;
    }
}
