using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace SportsKinematics.UI
{

    /// <summary>
    /// Remember me value used to remember a users username if they select the 'Remember Me' checkbox
    /// </summary>
    public class RememberToggleChange : MonoBehaviour
    {
        //GameObject of login canvas
        public GameObject m_canvas;
        //InputField that stores users username
        public InputField m_inp;
        //Ensures remember me action is only done once.
        private int m_count = 0;

        /// <summary>
        /// Sets remember me values if checked
        /// Also handles canvas visibility for a tick.
        /// </summary>
        void Awake()
        {
            bool active = m_canvas.activeInHierarchy;

            if (!active && m_count < 1)
            {
                m_canvas.SetActive(true);
            }

            if (!string.IsNullOrEmpty(PlayerPrefs.GetString("RememberEmailAddress")))
            {
                GetComponent<Toggle>().isOn = true;
            }

            if (active && m_count < 1)
            {
                m_canvas.SetActive(false);
                m_count++;
            }
        }

        /// <summary>
        /// Changes values depending on whether or not the remember me value has been checked.
        /// </summary>
        public void onChange()
        {
            if (GetComponent<Toggle>().isOn)
            {
                string email = PlayerPrefs.GetString("RememberEmailAddress");

                if (m_inp)
                     email = m_inp.text;

                PlayerPrefs.SetString("RememberEmailAddress", email);

                return;
            }

            PlayerPrefs.DeleteKey("RememberEmailAddress");
        }
    }
}