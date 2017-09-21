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
        /// Data for the playlist currently active in the simulation.
        /// </summary>
        private SortedList<int, string> m_playlist;
        /// <summary>
        /// Represents the current scenario being played as an index based off its position in the list of scenarios from m_currentPlaylist.
        /// </summary>
        private int m_actionIndex;


        private SimulationResults m_results;
        private DataReader m_dataReader;
        public GameObject m_ball;
        public GameObject m_opponentSkeleton;
        public GameObject m_opponentStriker;
        public float m_time;
        public float m_countdownTimer;
        private float m_actionEndTime;
        private string m_actionName;
        public GameObject m_VRText;
        /// <summary>
        /// Used to Initialise the ReportManager as well as creating the simulation scene based off the current playlist.
        /// </summary>
        void Start()
        {
            //m_currentPlaylist = LoadPlaylist(m_fileName, m_folderName);
            m_results = this.GetComponent<SimulationResults>();
            m_dataReader = new DataReader();
            string playListPath = PlayerPrefs.GetString("PlaylistPath");
            //m_playlist = m_dataReader.ReadPlaylist(playListPath);
            //m_actionIndex = PlayerPrefs.GetInt("ScenarioIndex");

            LoadAndSetAction("recordings/testPlayList.txt");
            Countdown();
            //CreateScene();
        }

        private void LoadAndSetAction(string actionHeaderPath)
        {
            FileNames actionFilePaths = m_dataReader.ReadActionHeader(actionHeaderPath);
            SortedList<float, BallData> actionBallData = m_dataReader.ReadBall(actionFilePaths.ballDataFileName);
            /*SortedList<float, StrikerData> actionStrikerData = m_dataReader.ReadStriker(actionFilePaths.strikerDataFileName);
            SortedList<float, SkeletonData> actionSkeletonData = m_dataReader.ReadSkeleton(actionFilePaths.skeletonDataFileName);
            ConfigData actionConfigData = m_dataReader.ReadConfig(actionFilePaths.configDataFileName);

            List<float> times = new List<float>(actionBallData.Keys);
            for (int i = 0; i < times.Count; i++)
            {
                if (times[i] >= actionConfigData.ballOccludeStartTime && times[i] <= actionConfigData.ballOccludeEndTime) //If this time should be occluded
                {
                    BallData tmp = new BallData();
                    tmp = actionBallData[times[i]];
                    tmp.isOccluded = true;
                    actionBallData[times[i]] = tmp;
                }
            }

            times = new List<float>(actionStrikerData.Keys);
            for (int i = 0; i < times.Count; i++)
            {
                if (times[i] >= actionConfigData.strikerOccludeStartTime && times[i] <= actionConfigData.strikerOccludeEndTime) //If this time should be occluded
                {
                    StrikerData tmp = new StrikerData();
                    tmp = actionStrikerData[times[i]];
                    tmp.isOccluded = true;
                    actionStrikerData[times[i]] = tmp;
                }
            }

            times = new List<float>(actionSkeletonData.Keys);
            for (int i = 0; i < times.Count; i++)
            {
                if (times[i] >= actionConfigData.jointOccludeStartTime && times[i] <= actionConfigData.jointOccludeEndTime) //If this time should be occluded
                {
                    SkeletonData tmp = new SkeletonData();
                    tmp = actionSkeletonData[times[i]];
                    for(int k = 0; k < tmp.joints.Count; k++)
                    {
                        for(int j = 0; j < actionConfigData.jointsToOcclude.Count; j++)
                        {
                            if(string.Equals(tmp.joints[k].name, actionConfigData.jointsToOcclude[j]))
                            {
                                Joint tmpJ = new Joint();
                                tmpJ = tmp.joints[k];
                                tmpJ.isOccluded = true;
                                tmp.joints[k] = tmpJ;
                            }
                        }
                    }
                    actionSkeletonData[times[i]] = tmp;
                }
            }
            */
            m_ball.GetComponent<RecordedBall>().SetData(actionBallData);
            //m_opponentStriker.GetComponent<RecordedStriker>().SetData(actionStrikerData);
            //m_opponentSkeleton.GetComponent<RecordedSkeleton>().SetData(actionSkeletonData);
            m_actionEndTime = 15;
            m_actionName = actionFilePaths.actionName;
        }

        void Update()
        {
            if (m_countdownTimer > 0)
            {
                m_countdownTimer -= Time.deltaTime;
                m_VRText.GetComponent<TextMesh>().text = m_countdownTimer.ToString("F2");
            }
            else
            {
                m_VRText.GetComponent<TextMesh>().text = m_time.ToString("F2");
                if (m_time <= m_actionEndTime)
                {
                    m_time += Time.deltaTime;
                }
                else
                {
                    m_VRText.GetComponent<TextMesh>().text = "Simulation Complete";
                }
            }

            
        }

        /// <summary>
        /// Generates the individual report for the scenario and creates the next scenario.
        /// </summary>
        public void PlayNext()
        {
            //GenerateReport(false);

            m_actionIndex = PlayerPrefs.GetInt("ScenarioIndex");
            LoadAndSetAction(m_playlist[m_actionIndex]);
            Countdown();
        }

        public void Countdown()
        {
            m_countdownTimer = 1;
            m_time = 0;
        }

        /// <summary>
        /// Generates the individual or overall report based off the configuration data and simulation
        /// results from the current playlist in the simulation.
        /// </summary>
        /// <param name="overall">True if the overall report is to be generated.</param>
        public void GenerateReport(bool overall)
        {
            string report = "Action Name: " +  m_actionName + "\n";
            report += "Date and Time: " + System.DateTime.Now.ToString("yyyy-MMM-dd HH-mm-ss") + "\n\n";

            report += m_results.GetResultsString();


             /*string individualName = "Individual " + m_currentPlaylist.Name + " Scenario " + m_scenarioIndex + " " + System.DateTime.Now.ToString("yyyy-MMM-dd HH-mm-ss") + ".csv"; 
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
             }*/
        }

        /// <summary>
        /// Returns the number of scenarios in the playlist.
        /// </summary>
        /// <returns>Number of scenarios in the playlist.</returns>
        public int GetPlaylistCount()
        {
            if (m_playlist != null)
                return m_playlist.Count;
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

