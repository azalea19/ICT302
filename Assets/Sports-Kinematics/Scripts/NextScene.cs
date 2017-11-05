using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

namespace SportsKinematics.UI
{
    /// <summary>
    /// Gets the next action/scenario from the playlist next.
    /// If at the last action/scenario displays the option to send report.
    /// </summary>
    public class NextScene : MonoBehaviour
    {
        /// <summary>
        /// Index of action/scenario
        /// </summary>
        private int m_index;
        /// <summary>
        /// Bool logging if the action is the last in the playlist.
        /// </summary>
        private bool m_finalSceneBool;
        /// <summary>
        /// Text to display, either 'next scenario' or 'send report'
        /// </summary>
        public Text m_text;

        /// <summary>
        /// Initializes button depending on the playlists index count
        /// </summary>
        void Start()
        {
            m_finalSceneBool = false;

            m_index = PlayerPrefs.GetInt("ScenarioIndex");

            if (m_index == GameObject.Find("SimulationManager").GetComponent<SimulationManager>().GetPlaylistCount() - 1)
            {
                m_text.text = "Send Report";
                m_finalSceneBool = true;
            }else
            {
                Debug.Log("pLAYLIST: "+GameObject.Find("SimulationManager").GetComponent<SimulationManager>().GetPlaylistCount());
            }
        }

        /// <summary>
        /// Callback which updates the 'next scenario' button.
        /// Loads the next scenario when clicked, sends report if at the end of
        /// the playlist list.
        /// </summary>
        public void TaskOnClick()
        {
            SimulationManager simMan = GameObject.Find("SimulationManager").GetComponent<SimulationManager>();
            if (m_finalSceneBool)
            {
                simMan.GenerateReport(m_finalSceneBool);
                SceneManager.LoadScene("MainMenu");
            }
            else
            {
                m_index++;

                PlayerPrefs.SetInt("ScenarioIndex", m_index);

                simMan.PlayNext();

                GetComponent<ButtonManager>().LoadLevel();
            }
        }
    }
}