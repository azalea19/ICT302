using System.Collections.Generic;
using UnityEngine;
using Windows.Kinect;
using System;
using System.IO;
using System.Threading;
//using UnityEditor;

/// <summary>
/// Used to record and store KinectData to external files.
/// Three public variables are to be used as toggles for the states
/// of the object, dictating it's bahaviour and a forth for where the data is stored.
/// This includes whether it's logging, saving or discarding data.
/// </summary>
/// <author>Justin Johnston</author>
/// <date>25/03/2017</date>
///
namespace SportsKinematics
{
    public class RecordKinectData : MonoBehaviour
    {
        /// <summary>
        /// Used for samplerate
        /// </summary>
        private uint m_frameCount = 0;

        /// <summary>
        /// Contains logged position data
        /// </summary>
        private List<Dictionary<JointType, float[]>> m_pointPosition;
        /// <summary>
        /// Contains logged orientation data
        /// </summary>
        private List<Dictionary<JointType, float[]>> m_pointOrientation;
        /// <summary>
        /// Contains logged frame data
        /// </summary>
        private List<Texture2D> m_frameData;
        /// <summary>
        /// Contains logged depth data
        /// </summary>
        private List<ushort[]> m_depthData;

        /// <summary>
        /// Thread to log joint position and orientation
        /// </summary>
        private Thread t_LogPointData;
        /// <summary>
        /// Thread to log frame
        /// </summary>
        private Thread t_LogFrameData;
        /// <summary>
        /// Thread to log depth
        /// </summary>
        private Thread t_LogDepthData;

        /// <summary>
        /// Facade Script
        /// </summary>
        private CaptureFacade m_KinectFacade;
        /// <summary>
        /// Facade Object
        /// </summary>
        public GameObject kinectFacade;

        /// <summary>
        /// Every 1/sampleRate frame, capture data
        /// </summary>
        public uint m_sampleRate = 30;
        /// <summary>
        /// Log data
        /// </summary>
        public bool m_logData = false;
        /// <summary>
        /// Save logged data
        /// </summary>
        public bool m_saveData = false;
        /// <summary>
        /// Discard logged data
        /// </summary>
        public bool m_discardData = false;
        /// <summary>
        /// true to log images
        /// </summary>
        public bool m_logImages = false;
        /// <summary>
        /// Log data location
        /// </summary>
        public string m_LogPath = ""; 
        /// <summary>
        /// Data type for the frame data.
        /// </summary>
        public string m_frameDataFileType = ".jpg";

        /// <summary>
        /// Initilizes StringBuilder Containing KinectData
        /// and BodySourceManager member variable.
        /// </summary>
        /// <author>Justin Johnston</author>
        /// <date>25/03/2017</date>
        void Start()
        {
            m_KinectFacade = kinectFacade.GetComponent<CaptureFacade>();

            m_pointPosition = new List<Dictionary<JointType, float[]>>();
            m_pointOrientation = new List<Dictionary<JointType, float[]>>();
            m_frameData = new List<Texture2D>();
            m_depthData = new List<ushort[]>();
        }

        /// <summary>
        /// Updates the stringbuilder containing the kinect data if
        /// in store mode, otherwise will save data if in saved mode
        /// or discard data if in discard mode. Also updates frameData[]
        /// with associated frameData for each pointData set.
        /// </summary>
        /// <author>Justin Johnston</author>
        /// <date>25/03/2017</date>
        void Update()
        {
            if (m_KinectFacade.ColourView.ColourManager.GetKinect().IsAvailable)
            {
                if (m_logData && (m_frameCount != 0 || m_pointPosition.Count > 0))//FR5 - Data logging facilities.
                {
                    if (m_frameCount >= m_sampleRate)
                    {
                        //Get all kinect body data
                        Body[] bodyData = m_KinectFacade.BodyView.BodyManager.GetData();

                        //checks if any bodies are active
                        //if they are, track the first body that's valid
                        Body firstBody = null;

                        foreach (Body b in bodyData)
                        {
                            if (b.TrackingId != 0)
                            {
                                Console.WriteLine("Body identified: " + b);
                                firstBody = b;
                                break;
                            }
                            else
                            {
                                Console.WriteLine("body is null");
                            }
                        }

                        //if we have at least one active body
                        if (firstBody != null)
                        {
                            //Log joint data
                            {
                                t_LogPointData = new Thread(() => LogJointData(firstBody));
                                if (!t_LogPointData.IsAlive)
                                    t_LogPointData.Start();
                            }

                            //Log texture data
                            {
                                if (m_logImages)
                                {
                                    Texture2D tex = m_KinectFacade.ColourView.ColourManager.GetColorTexture();
                                    Texture2D temp = new Texture2D(tex.width, tex.height);
                                    temp.SetPixels(tex.GetPixels());

                                    t_LogFrameData = new Thread(() => LogFrameTexture(temp));
                                    if (!t_LogFrameData.IsAlive)
                                        t_LogFrameData.Start();
                                }
                            }

                            //Log depth data
                            {
                                ushort[] depth = m_KinectFacade.InfraredView.InfraredManager.GetData();
                                t_LogDepthData = new Thread(() => LogDepthData(depth));
                                if (!t_LogDepthData.IsAlive)
                                    t_LogDepthData.Start();
                            }
                        }
                    }
                }

                //if in saved state and not logging state, not in saved state.
                if (m_pointPosition.Count == 0 && m_saveData)
                {
                    m_saveData = false;
                }

                if (m_saveData)
                {
                    SaveLoggedData("Unedited");
                }

                //if discard data state discard data.
                if (m_discardData)
                    DiscardLoggedData();
            }
            else
            {
                m_logData = false;
                m_logImages = false;
                m_saveData = false;
                m_discardData = false;
            }

            m_frameCount++;
        }

        /// <summary>
        /// Saves logged data to the logPath variable, if logPath is null, the data will not be set.
        /// Discards data after it's saved. Toggles all states to false. Also saves png byte data to 
        /// multiple files and depth data.
        /// </summary>
        /// <author>Justin Johnston</author>
        /// <date>25/03/2017</date>
        private void SaveLoggedData(string mode)//FR5 - Data logging facilities.
        {
            m_saveData = false;
            SaveJointPosition(mode);
            SaveJointOrientation(mode);
            SaveTextureData(mode);
            SaveDepthData(mode);
            SaveActionData(mode);
            DiscardLoggedData();
        }

        /// <summary>
        /// Creates a text file containing all the paths to data files related to an action.
        /// </summary>
        /// <author>James Howson</author>
        /// <date>30/04/2017</date>
        private void SaveActionData(string mode)//FR5 - Data logging facilities.
        {
            string username = PlayerPrefs.GetString("CurrentUsername");
            string ext = "";
            if (mode == "Unedited")
                ext = PlayerPrefs.GetString("UneditedExtension");
            else
                ext = PlayerPrefs.GetString("EditedExtension");

            using (var w = new StreamWriter(PlayerPrefs.GetString("CurrentUserDataPath") + "/Actions/" + mode + "/" + m_LogPath + ext))
            {
                string path = "/../Users/" + username + "/ActionData/" + mode + "/";
                var actionName = m_LogPath;
                var positionEntry = path + "/Position/";
                var orientationEntry = path + "/Orientation/";
                var depthEntry = path + "/Depth/";
                var actionEntry = string.Format("{0},{1},{2},{3},{4}", actionName, positionEntry, orientationEntry, depthEntry, mode);
                Debug.Log(actionEntry);
                w.WriteLine(actionEntry);
                w.Flush();
                w.Close();
            }
        }

        /// <summary>
        /// Discards data in StringBuilder by creating new StringBuilder.
        /// Toggles logging and discard data states to false.
        /// </summary>
        /// <author>Justin Johnston</author>
        /// <date>25/03/2017</date>
        private void DiscardLoggedData()//FR5 - Data logging facilities.
        {
            m_frameCount = 1;
            m_logData = false;
            m_discardData = false;

            if(m_pointPosition != null)
                m_pointPosition.Clear();
            if (m_pointOrientation != null)
                m_pointOrientation.Clear();
            if (m_frameData != null)
                m_frameData.Clear();
            if (m_depthData != null)
                m_depthData.Clear();
        }

        /// <summary>
        /// Threaded function to log joint position and orientation
        /// </summary>
        /// <author>Justin Johnston</author>
        /// <date>10/04/2017</date>
        private void LogJointData(Body bodyData)//FR5 - Data logging facilities.
        {
            LogJointPositions(bodyData);
            LogJointOrientations(bodyData);
        }

        /// <summary>
        /// Saves the position data for the current recording session.
        /// </summary>
        /// <param name="mode">Type of data being saved, Edited or Unedited.</param>
        private void SaveJointPosition(string mode)//FR5 - Data logging facilities.
        {
            if (m_LogPath != "" && m_pointPosition.Count != 0)
            {
                Serial<List<Dictionary<JointType, float[]>>>.Save(m_pointPosition, m_LogPath + PlayerPrefs.GetString("PositionExtension"), PlayerPrefs.GetString("CurrentUserDataPath") + "/ActionData/" + mode + "/Position/");
            }
        }

        /// <summary>
        /// Saves the orientation data for the current recording session.
        /// </summary>
        /// <param name="mode">Type of data being saved, Edited or Unedited.</param>
        private void SaveJointOrientation(string mode)//FR5 - Data logging facilities.
        {
            if (m_LogPath != "" && m_pointOrientation.Count != 0)
            {
                Serial<List<Dictionary<JointType, float[]>>>.Save(m_pointOrientation, m_LogPath + PlayerPrefs.GetString("OrientationExtension"), PlayerPrefs.GetString("CurrentUserDataPath") + "/ActionData/" + mode + "/Orientation/");
            }
        }

        /// <summary>
        /// Saves the depth data for the current recording session as jpg images.
        /// </summary>
        /// <param name="mode">Type of data being saved, Edited or Unedited.</param>
        private void SaveTextureData(string mode)//FR5 - Data logging facilities.
        {
            //check if we have frame data before making a folder for it
            if (m_LogPath != "" && m_frameData != null && m_frameData.Count != 0)
            {
                 Directory.CreateDirectory(PlayerPrefs.GetString("CurrentUserDataPath") + "/ActionData/Images/" + m_LogPath + "/");

                for (int i = 0; i < m_frameData.Count; i++)
                {
                    TextureToFile(PlayerPrefs.GetString("CurrentUserDataPath") + "/ActionData/Images/" + m_LogPath + "/" + i + m_frameDataFileType, m_frameData[i]);
                }
            }
        }

        /// <summary>
        /// Saves the depth data for the current recording session.
        /// </summary>
        /// <param name="mode">Type of data being saved, Edited or Unedited.</param>
        private void SaveDepthData(string mode)//FR5 - Data logging facilities.
        {
            if (m_LogPath != "" && m_depthData != null && m_depthData.Count != 0)
            {
                Serial<List<ushort[]>>.Save(m_depthData, m_LogPath + PlayerPrefs.GetString("DepthExtension"), PlayerPrefs.GetString("CurrentUserDataPath") + "/ActionData/" + mode + "/Depth/");
            }
        }

        /// <summary>
        /// Logs all joint positions and appends their data to the StringBuilder,
        /// given the body is currently active (TrackingId  != 0).
        /// </summary>
        /// <author>Justin Johnston</author>
        /// <date>25/03/2017</date>
        /// <param name="bodyData">Kinect Body Data</param>
        private void LogJointPositions(Body b)//FR5 - Data logging facilities.
        {
            Dictionary<JointType, float[]> tempDic = new Dictionary<JointType, float[]>();
            foreach (KeyValuePair<JointType, Windows.Kinect.Joint> j in b.Joints)
            {
                Vector3 unityJoint = GetVector3FromJoint(j.Value);
                float[] vec = { unityJoint.x, unityJoint.y, unityJoint.z };
                tempDic.Add(j.Value.JointType, vec);
            }

            m_pointPosition.Add(tempDic);
        }

        /// <summary>
        /// Logs all joint orentations and appends their data to the StringBuilder,
        /// given the body is currently active (TrackingId  != 0).
        /// </summary>
        /// <author>Justin Johnston</author>
        /// <date>25/03/2017</date>
        /// <param name="bodyData">Kinect Body Data</param>
        private void LogJointOrientations(Body b)//FR5 - Data logging facilities.
        {
            Dictionary<JointType, float[]> tempDic = new Dictionary<JointType, float[]>();
            foreach (KeyValuePair<JointType, Windows.Kinect.JointOrientation> jo in b.JointOrientations)
            {
                Vector3 unityJoint = GetVector3FromJointOrientation(jo.Value);
                float[] vec = { unityJoint.x, unityJoint.y, unityJoint.z };
                tempDic.Add(jo.Value.JointType, vec);
            }

            m_pointOrientation.Add(tempDic);
        }

        /// <summary>
        /// Stores byte data representing a png to a List of frameData.
        /// </summary>
        /// <param name="tex">Texture data for a frame.</param>
        /// <author>Justin Johnston</author>
        /// <date>29/03/2017</date>
        private void LogFrameTexture(Texture2D tex)//FR5 - Data logging facilities.
        {
            m_frameData.Add(tex);
        }

        /// <summary>
        /// Stores the depth data to a list of depth data. Each entry in the
        /// list represents a frame.
        /// </summary>
        /// <param name="depth">Depth data for a frame.</param>
        private void LogDepthData(ushort[] depth)//FR5 - Data logging facilities.
        {
            m_depthData.Add(depth);
        }

        /// <summary>
        /// Saves byteData to a file specified by _FileName
        /// </summary>
        /// <param name="fileName">Path to save file to</param>
        /// <param name="byteArray">Frame data to be saved to file</param>
        /// <returns>True if completed correctly.</returns>
        public bool TextureToFile(string fileName, Texture2D tex)//FR5 - Data logging facilities.
        {
            Color[] pix = tex.GetPixels();
            System.Array.Reverse(pix, 0, pix.Length);
            tex.SetPixels(pix);
            byte[] byteArray = tex.EncodeToJPG();

            try
            {
                // Open file for reading
                System.IO.FileStream _FileStream = new System.IO.FileStream(fileName, System.IO.FileMode.Create, System.IO.FileAccess.Write);
                // Writes a block of bytes to this stream using data from a byte array.
                _FileStream.Write(byteArray, 0, byteArray.Length);
                _FileStream.Close();
                return true;
            }
            catch (Exception _Exception)
            {
                Console.WriteLine("Exception caught in process: {0}", _Exception.ToString());
            }

            // error occured, return false
            return false;
        }

        /// <summary>
        /// Used to standardize a joint position to local Unity Space.
        /// </summary>
        /// <author>Justin Johnston</author>
        /// <date>25/03/2017</date>
        /// <param name="j"> Joint to be standardized</param>
        /// <returns>Unity space Vector3</returns>
        private static Vector3 GetVector3FromJoint(Windows.Kinect.Joint j)//FR5 - Data logging facilities.
        {
            return new Vector3(j.Position.X * 10f, j.Position.Y * 10f, j.Position.Z * 10f);
        }

        /// <summary>
        /// Used to standardize a joint orentation rotation to local Unity Space.
        /// </summary>
        /// <author>Justin Johnston</author>
        /// <date>25/03/2017</date>
        /// <param name="jo"> JointOrientation to be standardized</param>
        /// <returns>Unity space Vector3</returns>
        private static Vector3 GetVector3FromJointOrientation(Windows.Kinect.JointOrientation jo)//FR5 - Data logging facilities.
        {
            return new Vector3(jo.Orientation.X, jo.Orientation.Y, jo.Orientation.Z);
        }

        /// <summary>
        /// Used to saved edited action data.
        /// </summary>
        /// <param name="path">Path to data.</param>
        /// <param name="pointPosition">Edited position data.</param>
        /// <param name="pointOrientation">Edited orientation data.</param>
        /// <param name="depthData">Edited depth data.</param>
        public void SaveEditedLogData(string path, List<Dictionary<JointType, float[]>> pointPosition, List<Dictionary<JointType, float[]>> pointOrientation, List<ushort[]> depthData = null)
        {
            m_LogPath = System.IO.Path.GetFileName(path);
            m_pointPosition = pointPosition;
            m_pointOrientation = pointOrientation;
            if(depthData != null)
                m_depthData = depthData;

            SaveLoggedData("Edited");
        }
    }
}