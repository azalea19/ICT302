using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace SportsKinematics.UI
{
    /// <summary>
    /// Check if playlist is loaded and allow interactions with button if playlist is loaded
    /// </summary>
    public class CheckPlaylistLoad : MonoBehaviour
    {
        /// <summary>
        /// Text to change, if playlist is loaded
        /// </summary>
        public Text m_text;

        /// <summary>
        /// Button to enable
        /// </summary>
        public Button m_button;
        
        /// <summary>
        /// GameObject Start function. Instantiate basic values
        /// </summary>
        void Start()
        {
            m_text.text = "None";
            m_button.interactable = false;
        }

        /// <summary>
        /// If text is changed, change button being interactable
        /// </summary>
        private void OnValueChanged()
        {
            if (m_text.text != "None")
            {
                m_button.interactable = true;
            }
            else
            {
                m_button.interactable = false;
            }
        }

        /// <summary>
        /// change text to match playlist name
        /// </summary>
        /// <param name="name">name of playlist</param>
        public void ChangeText(string name)
        { 
            if (name != "")
            {
                m_text.text = name;
            }

            OnValueChanged();
        }
    }
}