using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Xml.Serialization;
using System.Text;
using System.IO;

namespace SportsKinematics
{
    /// <summary>
    /// Manages the current playlist and its scenarios loaded in the similation.
    /// </summary>
    /// <author>James Howson</author>
    /// <date>01/04/2017</date>
    public class SimulationManager : MonoBehaviour
    {
        /// <summary>
        /// Path to the playlist to be used in the simulation.
        /// </summary>
        private string m_fileName;
        /// <summary>
        /// Data for the playlist currently active in the simulation.
        /// </summary>
        private Playlist m_currentPlaylist;
        /// <summary>
        /// Configuration settings for the current scenario in the playlist. 
        /// </summary>
        private Configuration m_currentScenarioConfiguation;
        /// <summary>
        /// Represents the current scenario being played as an index based off its position in the list of scenarios from m_currentPlaylist.
        /// </summary>
        private int m_scenarioIndex;
        /// <summary>
        /// Used to load actions from file, into the simulation.
        /// </summary>
        private ActionLoader m_loader;
        /// <summary>
        /// Renders the simulation using the scenario data for the currently active scenario.
        /// </summary>
        private ActionRenderer m_renderer;
        /// <summary>
        /// Handles the generation, saving and emailing of the reports for each scenario and
        /// the overall report.
        /// </summary>
        private ReportManager m_reportManager;

        /// <summary>
        /// ReportManager gameobject used to retrieve the current ReportManager object.
        /// </summary>
        public GameObject m_reportManagerGameObject;
        /// <summary>
        /// ActionRenderer gameobject used to retrieve the current ActionRenerer object.
        /// </summary>
        public GameObject m_actionRenderer;

        /// <summary>
        /// Used to Initialise the ReportManager as well as creating the simulation scene based off the current playlist.
        /// </summary>
        void Start()
        {
            //m_currentPlaylist = LoadPlaylist(m_fileName, m_folderName);
            m_reportManager = m_reportManagerGameObject.GetComponent<ReportManager>();
            CreateScene();
        }

        /// <summary>
        /// Called in start. It creates the current scenario in the simulation based off m_scenarioIndex and
        /// the currently loaded playlist.
        /// </summary>
        private void CreateScene()//FR1 - Virtual opponent modelling from captured data.
        {
            m_fileName = PlayerPrefs.GetString("PlaylistPath");
            m_scenarioIndex = PlayerPrefs.GetInt("ScenarioIndex");
            //Debug.Log("Scenario index " + m_scenarioIndex);
            m_renderer = m_actionRenderer.GetComponent<ActionRenderer>();
            m_currentPlaylist = LoadPlaylist(m_fileName);
            string[] actionData = m_currentPlaylist.Scenarios[m_scenarioIndex].actionDataPaths;

            m_loader = new ActionLoader(Application.dataPath + actionData[1], Application.dataPath + actionData[2]);
            Action temp = m_loader.LoadActionFromFile(actionData[0]);
            m_currentScenarioConfiguation = Serial<Configuration>.Load(PlayerPrefs.GetString("CurrentUserDataPath") + "/Configurations/" + m_currentPlaylist.Name + "/" + m_currentPlaylist.Scenarios[m_scenarioIndex].configuration);

            m_renderer.InitConfig(m_currentScenarioConfiguation);
            m_renderer.InitiateRenderBody(temp);
        }

        /// <summary>
        /// Generates the individual report for the scenario and creates the next scenario.
        /// </summary>
        public void PlayNext()
        {
            GenerateReport(false);
            CreateScene();
        }

        /// <summary>
        /// Generates the individual or overall report based off the configuration data and simulation
        /// results from the current playlist in the simulation.
        /// </summary>
        /// <param name="overall">True if the overall report is to be generated.</param>
        public void GenerateReport(bool overall)
        {
            string correctSwing = "N/A";
            string paddleBallDistance = "N/A";
            string paddleBallDisplacement = "N/A";

            try
            {
                BallPassPlayer results = GameObject.Find("BallDetector").GetComponent<BallPassPlayer>();
                correctSwing = results.SwingCorrect.ToString();
                paddleBallDistance = results.Distance.ToString();
                paddleBallDisplacement = results.Displacement.ToString();
                Debug.Log("CorrectSwing: " + correctSwing);
                Debug.Log("Paddle To Ball Distance: " + paddleBallDistance);
                Debug.Log("Paddle Ball Distance: " + paddleBallDisplacement);
            }
            catch
            {
                Debug.Log("Cannot find BallDetector");
            }
                
            string individualName = "Individual " + m_currentPlaylist.Name + " Scenario " + m_scenarioIndex + " " + System.DateTime.Now.ToString("yyyy-MMM-dd HH-mm-ss") + ".csv"; 
            if (overall)
            {
                string overallName = "Overall " + m_currentPlaylist.Name + System.DateTime.Now.ToString("yyyy-MMM-dd HH-mm-ss") + ".csv";
                m_reportManager.CreateSceneReport(m_currentScenarioConfiguation, correctSwing, paddleBallDistance, paddleBallDisplacement);
                m_reportManager.SaveAndSend(PlayerPrefs.GetString("CurrentUserDataPath") + "/Reports/" + individualName);
                m_reportManager.SaveAndSendOverall(PlayerPrefs.GetString("CurrentUserDataPath") + "/Reports/" + overallName);
            }
            else
            {
                m_reportManager.CreateSceneReport(m_currentScenarioConfiguation, correctSwing, paddleBallDistance, paddleBallDisplacement);
                m_reportManager.SaveAndSend(PlayerPrefs.GetString("CurrentUserDataPath") + "/Reports/" + individualName);
            }
        }

        /// <summary>
        /// Returns the number of scenarios in the playlist.
        /// </summary>
        /// <returns>Number of scenarios in the playlist.</returns>
        public int GetPlaylistCount()
        {
            if (m_currentPlaylist != null)
                return m_currentPlaylist.Count;
            else
                return 0;
        }

        /// <summary>
        /// Deserializes the specified playlist form file.
        /// </summary>
        /// <param name="fileName">File name the playlist to load.</param>
        /// <param name="folderName">Path to the playlist data folder,</param>
        /// <returns>The deserialised playlist object.</returns>
        public Playlist LoadPlaylist(string fileName, string folderName)//FR1 - Virtual opponent modelling from captured data.
        {
            return Serial<Playlist>.Load(fileName, folderName);
        }

        /// <summary>
        /// Deserializes the specified playlist form file.
        /// </summary>
        /// <param name="fileName">File name the playlist to load.</param>
        /// <returns>The deserialised playlist object.</returns>
        public Playlist LoadPlaylist(string fileName)//FR1 - Virtual opponent modelling from captured data.
        {
            return Serial<Playlist>.Load(fileName);
        }
    }
}

