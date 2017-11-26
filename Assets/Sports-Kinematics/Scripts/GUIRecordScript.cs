using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace SportsKinematics.UI
{
    /// <summary>
    /// Button manager for Record scene
    /// </summary>
    public class GUIRecordScript : MonoBehaviour
    {
        /// <summary>
        /// RecordSystem GamoeObject reference
        /// </summary>
        public GameObject m_recordSystem;

        /// <summary>
        /// Text for Log button
        /// </summary>
        public GameObject m_text;
        
        /// <summary>
        /// Record Kinect Data script retreived from record system gameobject
        /// </summary>
        private RecordKinectData m_recSys;

        /// <summary>
        /// boolean for logging data
        /// </summary>
        private bool m_logging;

        // Use this for initialization
        /// <summary>
        /// GameObject start funciton. Initialise the Record System Script
        /// </summary>
        void Start()//FR6 - UI environment for client/test administrator.
        {
            m_recSys = m_recordSystem.GetComponent<RecordKinectData>();
        }

        /// <summary>
        /// Change Button Text
        /// </summary>
        private void ChangeText()//FR6 - UI environment for client/test administrator.
        {
            if (m_logging)
            {
                m_text.GetComponent<Text>().text = "Stop";
                return;
            }
            else
            {
                m_text.GetComponentInChildren<Text>().text = "Record";
            }
        }

        /// <summary>
        /// Event Handler for logging task click
        /// </summary>
        public void LogTaskOnClick()//FR6 - UI environment for client/test administrator.
        {
            m_recSys.m_logData = !m_recSys.m_logData;
            m_logging = m_recSys.m_logData;
            ChangeText();
        }

        /// <summary>
        /// Event handler for saving task click
        /// </summary>
        public void SaveTaskOnClick()//FR6 - UI environment for client/test administrator.
        {
            m_recSys.m_saveData = true;
            m_logging = false;
            m_recSys.m_LogPath = System.DateTime.Now.ToString("yyyy-MMM-dd HH-mm-ss");
            ChangeText();
        }

        /// <summary>
        /// Task for discarding data click
        /// </summary>
        public void DiscardTaskOnClick()//FR6 - UI environment for client/test administrator.
        {
            m_recSys.m_discardData = true;
            m_logging = false;
            ChangeText();
        }

        /// <summary>
        /// Allow toggling for images to be logged
        /// </summary>
        public void RecordImagesToggle()//FR6 - UI environment for client/test administrator.
        {
            m_recSys.m_logImages = GetComponent<Toggle>().isOn;
        }

        /// <summary>
        /// GameObject Update function. Allows the button text to be changed
        /// </summary>
        void Update()//FR6 - UI environment for client/test administrator.
        {
            //if (m_text.GetComponent<Text>().text != "Record Images")
            //{
                if (m_recSys.m_logData == false && m_text.GetComponent<Text>().text == "Stop")
                {
                    m_logging = false;
                    ChangeText();
                    return;
                }
                else
                    if (m_recSys.m_logData == true && m_text.GetComponent<Text>().text == "Stop")
                    {
                        m_logging = true;
                        ChangeText();
                    }
            //}
        }
    }
}