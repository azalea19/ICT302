using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace SportsKinematics.UI
{ 
    /// <summary>
    /// Returns data depending on the button clicked after a popup.
    /// </summary>
    /// 
    public class Popup : MonoBehaviour
    {
        /// <summary>
        /// Button used to create popup
        /// </summary>
        private Button m_callingButton;

        /// <summary>
        /// Possible return values from popup interactions
        /// </summary>
        public enum DialogResults
        {
            No = 0,
            Yes = 1,
            Continue = 2,
            Break = 3,
            Exit = 4,
            Error = 5,
            Logout = 6,
            SaveScenarioConfig = 7,
            DeleteScenarioFromPlaylist = 8,
            SaveSettings = 9,
        }

        /// <summary>
        /// Button property used to get and set button value
        /// </summary>
        public Button CallingButton
        {
            get { return m_callingButton; }
            set { m_callingButton = value; }
        }

        /// <summary>
        /// Returns yes.
        /// </summary>
        public void TaskOnClickYes()
        {
            m_callingButton.GetComponent<ButtonManager>().DestroyPopup(DialogResults.Yes);
        }
        /// <summary>
        /// Returns no.
        /// </summary>
        public void TaskOnClickNo()
        {
            m_callingButton.GetComponent<ButtonManager>().DestroyPopup(DialogResults.No);
        }
        /// <summary>
        /// Returns continue
        /// </summary>
        public void TaskOnClickContinue()
        {
            m_callingButton.GetComponent<ButtonManager>().DestroyPopup(DialogResults.Continue);
        }
        /// <summary>
        /// Return break.
        /// </summary>
        public void TaskOnClickBreak()
        {
            m_callingButton.GetComponent<ButtonManager>().DestroyPopup(DialogResults.Break);
        }
        /// <summary>
        /// Return exit.
        /// </summary>
        public void TaskOnClickExit()
        {
            m_callingButton.GetComponent<ButtonManager>().DestroyPopup(DialogResults.Exit);
        }
        /// <summary>
        /// Return error.
        /// </summary>
        public void TaskOnClickError()
        {
            m_callingButton.GetComponent<ButtonManager>().DestroyPopup(DialogResults.Error);
        }
        /// <summary>
        /// Return logout.
        /// </summary>
        public void TaskOnClickLogout()
        {
            m_callingButton.GetComponent<ButtonManager>().DestroyPopup(DialogResults.Logout);
        }
        /// <summary>
        /// Return save scenario config.
        /// </summary>
        public void TaskOnClickSaveScenarioConfig()
        {
            m_callingButton.GetComponent<ButtonManager>().DestroyPopup(DialogResults.SaveScenarioConfig);
        }
        /// <summary>
        /// Return delete scenario playlist.
        /// </summary>
        public void TaskOnClickDeleteScenarioFromPlaylist()
        {
            m_callingButton.GetComponent<ButtonManager>().DestroyPopup(DialogResults.DeleteScenarioFromPlaylist);
        }
        /// <summary>
        /// Return save settings.
        /// </summary>
        public void TaskOnClickSaveSettings()
        {
            m_callingButton.GetComponent<ButtonManager>().DestroyPopup(DialogResults.SaveSettings);
        }
    }
}