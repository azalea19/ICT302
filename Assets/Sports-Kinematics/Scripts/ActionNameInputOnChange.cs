using UnityEngine;
using UnityEngine.UI;

namespace SportsKinematics
{
    /// <summary>
    /// Used to get the action name for the saved action data.
    /// </summary>
    public class ActionNameInputOnChange : MonoBehaviour
    {
        /// <summary>
        /// GameObject to set record system, set in Unity.
        /// </summary>
        public GameObject m_recordSystem;
        /// <summary>
        /// Record system reference to pass saved name.
        /// </summary>
        private RecordKinectData m_recSys;

        /// <summary>
        /// Input field used to store the action datas name.
        /// </summary>
        InputField m_actionNameInput;
        /// <summary>
        /// Event used to display the action datas name.
        /// </summary>
        InputField.SubmitEvent m_actionNameEvent;

        /// <summary>
        /// Initializes RecordKinectData and Input field values.
        /// </summary>
        private void Start()
        {
            m_recSys = m_recordSystem.GetComponent<RecordKinectData>();
            m_actionNameInput = gameObject.GetComponent<InputField>();
            m_actionNameEvent = new InputField.SubmitEvent();

            m_actionNameEvent.AddListener(ActionNameChange);
            m_actionNameInput.onEndEdit = m_actionNameEvent;
        }
        /// <summary>
        /// Used to set the log path of an action to a specific value.
        /// </summary>
        /// <param name="name">Name to set log path as</param>
        private void ActionNameChange(string name)
        {
            m_recSys.m_LogPath = name;
            //m_actionNameInput.Select();
            //m_actionNameInput.text = "";
        }
    }
}

