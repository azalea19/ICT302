using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace SportsKinematics.UI
{
    /// <summary>
    /// Checks if data is loaded 
    /// </summary>
    public class CheckLoad : MonoBehaviour
    {
        /// <summary>
        /// Determine if button is interactable
        /// </summary>
        private bool m_interactable;

        /// <summary>
        /// button to enable
        /// </summary>
        public Button m_button;

        /// <summary>
        /// accessor method for interactions
        /// </summary>
        public bool Interactable
        {
            get { return m_interactable; }
            set { m_interactable = value; }
        }

        /// <summary>
        /// allow button to be interacted with
        /// </summary>
        void Update()
        {
            if(m_button.interactable != m_interactable)
                m_button.interactable = m_interactable;
        }
    }
}