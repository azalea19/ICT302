using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace SportsKinematics.UI
{
    /// <summary>
    /// Used to toggle the LineRender for a GameObject
    /// </summary>
    public class ToggleLines : MonoBehaviour
    {
        /// <summary>
        /// GameObject which contains action renderer, set in Unity.
        /// </summary>
        public GameObject m_renderer;

        /// <summary>
        /// Starts the LineRender as enabled for specific GameObject
        /// </summary>
        private void Start()
        {
            m_renderer.GetComponent<ActionRenderer>().m_drawLines = GetComponent<Toggle>().isOn;
        }
        
        /// <summary>
        /// Toggles the LineRender for a specific GameObject
        /// </summary>
        public void TaskOnValueChanged()
        {
            m_renderer.GetComponent<ActionRenderer>().ToggleLines();
        }
    }
}
