using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace SportsKinematics.UI
{
    /// <summary>
    /// Sets the action renderer to either pause or play current action
    /// </summary>
    public class Pause : MonoBehaviour
    {
        /// <summary>
        /// GameObject reference to action renderer
        /// </summary>
        public GameObject m_actionRenderer;
        /// <summary>
        /// GameObject reference to ball
        /// </summary>
        public GameObject m_ball;
        /// <summary>
        /// Text value to either display play or pause on button
        /// </summary>
        public Text m_text;
        /// <summary>
        /// Bool representing whether we're paused or playing
        /// </summary>
        private bool m_play;

        /// <summary>
        /// Sets play value to false
        /// </summary>
        void Start()
        {
            m_play = false;
        }

        /// <summary>
        /// Toggles current play mode and sets value in action renderer
        /// </summary>
        public void TaskOnClick()
        {
            m_play = !m_play;

            if (!m_play)
            {
                m_text.text = "Play";
            }
            else
            {
                m_text.text = "Pause";
            }

            if (m_actionRenderer && m_actionRenderer.GetComponent<ActionRenderer>())
            {
                m_actionRenderer.GetComponent<ActionRenderer>().m_play = m_play;
            }

            if (m_ball && m_ball.GetComponent<BallScript>())
            {
                m_ball.GetComponent<BallScript>().m_play = m_play;
            }
        }
    }
}