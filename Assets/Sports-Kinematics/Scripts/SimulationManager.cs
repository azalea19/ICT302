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
        private SortedList<int, string> m_playlist;
        private int m_actionIndex;
        private SimulationResults m_results;
        private DataReader m_dataReader;
        private float m_actionEndTime;
        private string m_actionName;
        private bool m_isPaused;
        private bool m_displayResult;
        private GameObject m_recordedPlayerStriker;
        private float m_countdownTimer;

        public GameObject m_ball;
        public GameObject m_opponentSkeleton;
        public GameObject m_opponentStriker;
        public GameObject m_playerStriker;
        public float m_time;
        public GameObject m_VRText;
        void Start()
        {
            m_results = this.GetComponent<SimulationResults>();
            m_dataReader = new DataReader();
            string playListPath = PlayerPrefs.GetString("PlaylistPath");
            //m_playlist = m_dataReader.ReadPlaylist(playListPath);
            //m_actionIndex = PlayerPrefs.GetInt("ScenarioIndex");

            LoadAndSetAction("recordings/testPlayList.txt");
            Countdown();
            m_isPaused = true;
            m_displayResult = false;
        }

        private void LoadAndSetAction(string actionHeaderPath)
        {
            FileNames actionFilePaths = m_dataReader.ReadActionHeader(actionHeaderPath);
            SortedList<float, BallData> actionBallData = m_dataReader.ReadBall(actionFilePaths.ballDataFileName);
            SortedList<float, StrikerData> actionStrikerData = m_dataReader.ReadStriker(actionFilePaths.strikerDataFileName);
            SortedList<float, SkeletonData> actionSkeletonData = m_dataReader.ReadSkeleton(actionFilePaths.skeletonDataFileName);
            /*ConfigData actionConfigData = m_dataReader.ReadConfig(actionFilePaths.configDataFileName);

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
            m_opponentStriker.GetComponent<RecordedStriker>().SetData(actionStrikerData);
            m_opponentSkeleton.GetComponent<RecordedSkeleton>().SetData(actionSkeletonData);
            m_actionEndTime = 15;
            m_actionName = actionFilePaths.actionName;
        }

        void Update()
        {
            if (!(m_isPaused == true || m_displayResult == true))
            {
                if (m_countdownTimer > 0)
                {
                    m_countdownTimer -= Time.deltaTime;
                    m_VRText.GetComponent<TextMesh>().text ="Starting in: " + m_countdownTimer.ToString("F1");
                }
                else
                {
                    m_VRText.GetComponent<TextMesh>().text = "Elapsed Time: " + m_time.ToString("F2");
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
            if(m_displayResult)
            {
                m_VRText.GetComponent<TextMesh>().text = "Showing Result, Time: " + m_time.ToString("F2");
            }
        }

        /// <summary>
        /// Generates the individual report for the scenario and creates the next scenario.
        /// </summary>
        public void PlayNext()
        {
            GenerateReport(false);

            m_actionIndex = PlayerPrefs.GetInt("ScenarioIndex");
            LoadAndSetAction(m_playlist[m_actionIndex]);
            Countdown();
        }

        public void Countdown()
        {
            m_countdownTimer = 3;
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

            File.WriteAllText("Results/" + m_actionName + "_" + System.DateTime.Now.ToString("yyyy-MMM-dd HH-mm-ss") + ".txt", report);

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

        private void PlaySim()
        {
            m_isPaused = false;
            Countdown();
        }

        public void PauseOrPlaySim()
        {
            if (m_displayResult == true)
            {
                HideResult();
            }
            if (m_isPaused == true)
            {
                PlaySim();
            }
            else
            {
                PauseSim();
            }
        }

        public void ShowOrHideResult()
        {
            if(m_displayResult == true)
            {
                HideResult();
            }
            else
            {
                ShowResult();
            }
        }

        private void ShowResult()
        {
            if(m_results.m_isHitByPlayer)
            {
                m_time = m_results.m_hitTime;
            }
            else
            {
                m_time = m_results.m_smallestDistTime;
            }

            if(m_playerStriker.GetComponent<PlayerStriker>().GetData() != null)
            {
                m_recordedPlayerStriker = Object.Instantiate(m_opponentStriker);
                m_recordedPlayerStriker.GetComponent<RecordedStriker>().SetData(m_playerStriker.GetComponent<PlayerStriker>().GetData());
                m_playerStriker.GetComponent<PlayerStriker>().SetOccluded(true);
            }

            m_isPaused = true;
            m_displayResult = true;
        }

        private void HideResult()
        {
            Object.Destroy(m_recordedPlayerStriker);
            m_playerStriker.GetComponent<PlayerStriker>().SetOccluded(false);
            m_displayResult = false;
        }

        private void PauseSim()
        {
            m_isPaused = true;
        }

        public void RestartSim()
        {
            m_playerStriker.GetComponent<PlayerStriker>().ClearData();
            m_results.GetComponent<SimulationResults>().ClearResults();
            m_time = 0;
            if (m_displayResult == true)
            {
                HideResult();
            }
            PlaySim();
        }
    }
}

