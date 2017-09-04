using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace SportsKinematics.UI
{
    /// <summary>
    /// Helper for main menu canvas
    /// </summary>
    public class MainMenuCanvas : MonoBehaviour
    {
        /// <summary>
        /// settings canvas for user
        /// </summary>
        public GameObject m_settings;

        /// <summary>
        /// Awake function for the GameObject. Sets all settings toggles to Player Prefs value
        /// </summary>
        void Awake()
        {
            GetComponent<SwitchCanvasOnClick>().GoToWelcomePage();
            GameObject.Find("UserManager").GetComponent<UserManager>().SetCurrentUserDataPath();

            string canvas = "Settings/SettingsCanvas/";

            m_settings.SetActive(true);

            Toggle emailEnT = GameObject.Find(canvas + "EmailingToggle").GetComponent<Toggle>();
            Toggle recPopupT = GameObject.Find(canvas + "RecPopupToggle").GetComponent<Toggle>();
            Toggle editPopupT = GameObject.Find(canvas + "EditPopupToggle").GetComponent<Toggle>();
            Toggle simPopupT = GameObject.Find(canvas + "SimPopupToggle").GetComponent<Toggle>();

            if (PlayerPrefs.GetInt("EnableEmail") == 0)
            {
                emailEnT.isOn = false;
            }

            if (PlayerPrefs.GetInt("RecPopup") == 0)
            {
                recPopupT.isOn = false;
            }

            if (PlayerPrefs.GetInt("EditPopup") == 0)
            {
                editPopupT.isOn = false;
            }

            if (PlayerPrefs.GetInt("SimPopup") == 0)
            {
                simPopupT.isOn = false;
            }

            m_settings.SetActive(false);
        }
    }
}