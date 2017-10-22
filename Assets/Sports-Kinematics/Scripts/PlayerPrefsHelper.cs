using UnityEngine;

/// <summary>
/// Provides additional functionality to PlayerPrefs.
/// </summary>
public class PlayerPrefsHelper : MonoBehaviour
{
    /// <summary>
    /// Increments the int stored in playerprefs that corresponds to
    /// key.
    /// </summary>
    /// <param name="key">Identifies the PlayerPref to increment.</param>
    public static void IncInt(string key)
    {
        PlayerPrefs.SetInt(key, PlayerPrefs.GetInt(key) + 1);
    }

    /// <summary>
    /// Sets up the default PlayerPrefs values that should be set when the program starts.
    /// </summary>
    public static void SetAllToDefaults()
    {
        PlayerPrefs.SetString("PlaylistExtension", ".plist");
        PlayerPrefs.SetString("ConfigurationExtension", ".config");
        PlayerPrefs.SetString("PositionExtension", ".plpos");
        PlayerPrefs.SetString("OrientationExtension", ".plori");
        PlayerPrefs.SetString("DepthExtension", ".pldepth");
        PlayerPrefs.SetString("ImageExtension", ".jpg");
        PlayerPrefs.SetString("UneditedExtension", ".u-action");
        PlayerPrefs.SetString("EditedExtension", ".e-action");
        PlayerPrefs.DeleteKey("OverallReport");
        PlayerPrefs.SetString("OverallReport", "");
        PlayerPrefs.SetString("EditorSettings", ".csv");
        //PlayerPrefs.SetInt("ReloadBody", 0);
        PlayerPrefs.DeleteKey("EmailAddress");

        PlayerPrefs.SetInt("ScenarioIndex", 0);
    }
}
