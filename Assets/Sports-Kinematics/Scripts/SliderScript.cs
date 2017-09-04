using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace SportsKinematics
{
    /// <summary>
    /// Slider class used to set maximum and minimum rotation values
    /// for the strikers begining and ending rotation values.
    /// 
    /// Not used anymore.
    /// </summary>
    public class SliderScript : MonoBehaviour
    {
        /// <summary>
        /// Slider value containing variable
        /// </summary>
        public Slider m_slider;
        /// <summary>
        /// Input field containing sliders input
        /// </summary>
        public InputField m_input;
        /// <summary>
        /// minimum and maximum rotation values
        /// </summary>
        public float m_minRot, m_maxRot;

        /// <summary>
        /// Callback used to update min and max rotation values on slider
        /// change. Also parses the input text values to validate min and max values.
        /// </summary>
        public void TaskOnValueChange()//FR6 - UI environment for client/test administrator.
        {
            string s = m_input.text;

            if (float.Parse(s) > m_maxRot)
            {
                s = m_maxRot.ToString();
                m_input.text = s;
            }
            else
                if (float.Parse(s) < m_minRot)
            {
                s = m_minRot.ToString();
                m_input.text = s;
            }

            m_slider.value = float.Parse(s);
        }
    }
}