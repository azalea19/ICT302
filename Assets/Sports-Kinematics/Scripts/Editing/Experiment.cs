using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using System.IO;
namespace SportsKinematics
{
    public class Experiment 
    {


        private int m_start = 0;
        private int m_end = 0;
        private int m_ballFrame = -1;

        /// <summary>
        /// speed being rendered at
        /// </summary>
        public float m_speed = 50;
        /// <summary>
        /// Offset till the first joint; 0 - paddle; 1 - ball; 2 - skeleton
        /// </summary>
        public const int m_jointOffset = 3;

        public int FrameBall
        {
            get { return m_ballFrame; }

            set { m_ballFrame = value; }
        }

        public int FrameEnd
        {
            get { return m_end; }

            set { m_end = value; }
        }

        public int FrameStart
        {
            get { return m_start; }

            set { m_start = value; }
        }

        /// <summary>
        /// occlusion array for spatial occlusion. True = shown.
        /// 0 - paddle; 1 - ball; 2...26 - skeleton
        /// </summary>
        public bool[] m_occBoolArr;

        public int[] m_occFrameMin;
        public int[] m_occFrameMax;

        public void Start()
        {
            m_occBoolArr = new bool[25 + m_jointOffset];
            m_occFrameMin = new int[25 + m_jointOffset];
            m_occFrameMax = new int[25 + m_jointOffset];
            for (int i = 0; i < 25 + m_jointOffset; i++)
            {
                m_occBoolArr[i] = false;
                m_occFrameMin[i] = m_start;
                m_occFrameMax[i] = m_end;
            }
        }

        public void ReadConfig(bool isEditor, string expName)
        {
            if (!isEditor)
            {
                string username = PlayerPrefs.GetString("CurrentUsername");
                //string ext = "";
                //if (mode == "Unedited")
                //    ext = PlayerPrefs.GetString("UneditedExtension");
                //else
                //    ext = PlayerPrefs.GetString("EditedExtension");

                using (var w = new StreamReader(PlayerPrefs.GetString("CurrentUserDataPath") + "/Actions/" + "Edited" + "/" + expName))
                {
                    string path = "/../Users/" + username + "/ActionData/" + "Edited" + "/" + expName;

                    string row = w.ReadLine();
                    {
                        string[] value = row.Split(',');
                        float.TryParse(value[0], out m_speed);
                    }
                    row = w.ReadLine();
                    {
                        string[] value = row.Split(',');
                        for (int i = 0; i < value.Length; i++)
                        {
                            bool.TryParse(value[i], out m_occBoolArr[i]);
                            m_occBoolArr[i] = true;
                        }
                    }

                    row = w.ReadLine();
                    {
                        string[] value = row.Split(',');
                        for (int i = 0; i < value.Length; i++)
                            int.TryParse(value[i], out m_occFrameMin[i]);
                    }

                    row = w.ReadLine();
                    {
                        string[] value = row.Split(',');
                        for (int i = 0; i < value.Length; i++)
                            int.TryParse(value[i], out m_occFrameMax[i]);
                    }
                    w.Close();
                    // w.Flush();
                    //w.Close();
                }

            }
        }

        /// <summary>
        /// Saves spatial occlusion data
        /// </summary>
        /// <param name="filename">name of the file</param>
        public void SaveSpatialOcclusion(string filename)
        {
            string path = (PlayerPrefs.GetString("CurrentUserDataPath") + "/Actions/Edited/" + filename);
            // m_conf.SpatialIsActive = true;
            string body = "";// PlayerPrefs.GetString("EditorSettings");
            Debug.Log(body);
            body += m_speed + "," + m_start + "," + m_end + "\n";

            for (int i = 0; i < m_occBoolArr.Length; i++)
            {
                body += m_occBoolArr[i] + ",";
            }
            body = body.Substring(0, body.Length - 1);
            body += "\n";
            for (int i = 0; i < m_occFrameMax.Length; i++)
            {
                body += m_occFrameMin[i] + ",";
            }
            body = body.Substring(0, body.Length - 1);
            body += "\n";
            for (int i = 0; i < m_occFrameMin.Length; i++)
            {
                body += m_occFrameMax[i] + ",";
            }
            body = body.Substring(0, body.Length - 1);

            PlayerPrefs.SetString("EditorSettings", body);
            Debug.Log(body);
            Reporter.Save(path, body);
        }


    }
}