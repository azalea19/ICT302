using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace SportsKinematics.UI
{
    /// <summary>
    /// Timer which displays how long an 'action' has been rendering.
    /// </summary>
    public class Timer : MonoBehaviour
    {
        /// <summary>
        /// GameObject which contains action renderer, set in Unity.
        /// </summary>
        public GameObject m_actionRender;

        /// <summary>
        /// Updates the member time variables contained within action renderer GameObject.
        /// </summary>
        void Update()
        {
            int seconds;
            int frame;
            float miliseconds;

            miliseconds = m_actionRender.GetComponent<ActionRenderer>().TimeElapsed;
            frame = m_actionRender.GetComponent<ActionRenderer>().RenderFrame;

            seconds = (int)miliseconds;
            miliseconds -= seconds;

            miliseconds *= 1000.0f;

            GetComponent<Text>().text = "Time Elapsed:\n" + seconds + " seconds\n" + (int)miliseconds + " milliseconds\n" + "Frame Number: " + frame;//;
        }
    }
}