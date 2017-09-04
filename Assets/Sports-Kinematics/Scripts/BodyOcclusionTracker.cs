using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SportsKinematics.UI
{
    /// <summary>
    /// Tracks GUI member for occlusion configuration
    /// </summary>
    public class BodyOcclusionTracker : MonoBehaviour
    {
        /// <summary>
        /// Array of booleans used to determine if [joint] is being occluded
        /// </summary>
        private bool[] m_boolArray;

        /// <summary>
        /// GameObjects array for occlusion body in config scene
        /// </summary>
        public GameObject[] m_childArray;

        /// <summary>
        /// Awake method for GameObject. Instantiate data.
        /// </summary>
        // Use this for initialization
        void Awake()
        {
            m_childArray = new GameObject[25];

            m_boolArray = new bool[m_childArray.Length];

            for (int i = 0; i < m_boolArray.Length; i++)
            {
                m_childArray[i] = GameObject.Find(((Windows.Kinect.JointType)i).ToString());
                m_boolArray[i] = true;
            }
        }

        /// <summary>
        /// Accessor methods for the body occlusion
        /// </summary>
        public bool[] Occlusions
        {
            get
            {
                if (m_boolArray != null && m_childArray.Length != 0)
                {
                    for (int i = 0; i < m_boolArray.Length; i++)
                    {
                        m_boolArray[i] = m_childArray[i].GetComponent<JointToggle>().isOn;
                    }

                    return m_boolArray;
                }

                m_boolArray = new bool[25];

                for (int i = 0; i < 25; i++)
                {
                    m_boolArray[i] = true;
                }

                return m_boolArray;
            }

            set
            {
                for (int i = 0; i < m_boolArray.Length; i++)
                {
                    m_boolArray[i] = value[i];
                    m_childArray[i].GetComponent<JointToggle>().isOn = value[i];
                }
            }
        }
    }
}