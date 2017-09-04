using UnityEngine;
using UnityEngine.UI;

namespace SportsKinematics.UI
{
    /// <summary>
    /// Deletes all unused PlayerPrefs and re-populates spash screen 
    /// canvas with used PlayerPrefs for current user.
    /// </summary>
    public class SplashScreenCanvas : MonoBehaviour
    {
        /// <summary>
        /// Username for user
        /// </summary>
        public InputField m_userName;
        /// <summary>
        /// Remember me toggle
        /// </summary>
        public Toggle m_remember;

        /// <summary>
        /// Populates splash screen with relevant PlayerPrefs for
        /// current user.
        /// </summary>
        void Awake()
        {
            string email = PlayerPrefs.GetString("RememberEmailAddress");
            int emailEn = PlayerPrefs.GetInt("EnableEmail");
            int recPopup = PlayerPrefs.GetInt("RecPopup");
            int editPopup = PlayerPrefs.GetInt("EditPopup");
            int simPopup = PlayerPrefs.GetInt("SimPopup");

            PlayerPrefs.DeleteAll();

            PlayerPrefs.SetInt("RecPopup", recPopup);
            PlayerPrefs.SetInt("EditPopup", editPopup);
            PlayerPrefs.SetInt("SimPopup", simPopup);

            if (m_remember.isOn)
                PlayerPrefs.SetString("RememberEmailAddress", email);

            PlayerPrefs.SetString("ReportEmailAddress", email);
            PlayerPrefs.SetInt("EnableEmail", emailEn);

            PlayerPrefsHelper.SetAllToDefaults();

            if (!string.IsNullOrEmpty(email))
            {
                m_userName.text = email;
                m_remember.isOn = true;
                return;
            }
            else
            {
                m_remember.isOn = false;
                m_userName.text = "";
            }
        }
    }
}