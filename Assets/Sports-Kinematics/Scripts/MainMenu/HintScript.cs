using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

/// <summary>
/// Author: Olesia Kochergina
/// Date: 18/09/2017
/// </summary>
namespace SportsKinematics.UI
{
    public class HintScript : MonoBehaviour
    {

        /// <summary>
        /// Hint message 
        /// </summary>
        public string m_text = "Enter your hint.";

        /// <summary>
        /// Whether or not a hint should be displayed
        /// </summary>
        private bool m_displayInfo;

        /// <summary>
        /// Counter since the last time the cursor entered the button area
        /// </summary>
        private float m_counter;

        /// <summary>
        /// The amount of seconds it takes from the moment the cursor has entered the button area to the moment a hint needs to be displayed
        /// </summary>
        public float m_waitingPeriod = 3.10f;

        /// <summary>
        /// Position of the hint message
        /// </summary>
        private Vector3 m_position;

        /// <summary>
        /// Pointer to an image rect which is used to update the position of the hint message
        /// </summary>
        static private RectTransform m_hint;

        /// <summary>
        /// Pointer to a UI Text element which is used to display the text
        /// </summary>
        static private Text m_uiText;

        // Use this for initialization
        void Start()
        {
            m_hint = GameObject.Find("HintCanvas").transform.GetChild(0).GetComponent<RectTransform>();
            m_uiText = m_hint.transform.GetChild(0).GetComponent<Text>();
            m_hint.gameObject.SetActive(false);
        }

        // Update is called once per frame
        void Update()
        {
            if (m_displayInfo && (m_counter + m_waitingPeriod < Time.timeSinceLevelLoad))
            {
                UpdateHintPosition();
                m_hint.gameObject.SetActive(true);
            }
        }

        /// <summary>
        /// Updates the hint's position based on the cursor position.
        /// </summary>
        private void UpdateHintPosition()
        {
            m_position = Input.mousePosition;
            m_position.x += m_hint.rect.width / 2;
            m_position.y -= m_hint.rect.height / 2;
            m_hint.position = m_position;
        }

        /// <summary>
        /// Resets member variables.
        /// </summary>
        public void EnterEvent()
        {
            m_counter = Time.timeSinceLevelLoad;
            m_uiText.text = m_text;
            m_displayInfo = true;
            m_hint.gameObject.SetActive(false);
        }

        /// <summary>
        /// Makes sure the hint is set to inactive.
        /// </summary>
        public void ExitEvent()
        {
            m_displayInfo = false;
            m_hint.gameObject.SetActive(false);

        }
    }
}