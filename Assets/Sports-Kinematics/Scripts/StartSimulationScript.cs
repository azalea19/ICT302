using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SportsKinematics.UI
{
    /// <summary>
    /// Starts simulation scene with action 0 as the first to be rendered
    /// </summary>
    public class StartSimulationScript : MonoBehaviour
    {
        /// <summary>
        /// Callback to load simulation scene
        /// </summary>
        public void TaskOnClick(GameObject Popup)
        {
            PlayerPrefs.SetInt("ScenarioIndex", 0);

            GetComponent<ButtonManager>().LoadLevel(Popup);
        }
    }
}