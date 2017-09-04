using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System;

namespace SportsKinematics.UI
{
    /// <summary>
    /// Updates slider values based on Unity slider GameObject
    /// 
    /// Currently unused.
    /// </summary>
    public class SliderInputFieldUpdate : MonoBehaviour
    {
        /// <summary>
        /// Slider object used to track rotation
        /// </summary>
        public Slider m_slider;
        /// <summary>
        /// Slider text value used to validate input
        /// </summary>
        public InputField m_input;

        /// <summary>
        /// Adds callback listener for when slider value is changed.
        /// </summary>
        void Start()
        {
            m_slider.onValueChanged.AddListener(ChangeValue);
            ChangeValue(m_slider.value);
        }

        /// <summary>
        /// Callback for when slider value is changed
        /// </summary>
        /// <param name="value">Value slider has been changed to</param>
        void ChangeValue(float value)//FR6 - UI environment for client/test administrator.
        {
            if (!m_input.isFocused)
            {
                m_input.text = value.ToString("n");
            }
        }
    }
}