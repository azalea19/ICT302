using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace SportsKinematics.UI
{
    /// <summary>
    /// TextScript outputs the loaded data, if any, after loading a users PlayerPref data.
    /// </summary>
    public class TextScript : MonoBehaviour
    {
        /// <summary>
        /// Loaded data to be tested for.
        /// </summary>
        public string m_field;

        /// <summary>
        /// Outputs loaded data if it exists to console after being loaded.
        /// </summary>
        void Update()
        {
            if (m_field != null && PlayerPrefs.GetString(m_field) != "" && PlayerPrefs.GetString(m_field) != null)
            {
                GetComponent<Text>().text = PlayerPrefs.GetString(m_field);
            }
        }
    }
}
