using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SportsKinematics
{
    public class VisFacade : MonoBehaviour//NFR1 - Scalability.
    {
        public GameObject m_mainCam;
        public GameObject m_VRCam;
        public GameObject m_VRManager;
        public bool m_VRMode;

        private int m_frame = 0;
        private int m_frameMax = 5;
        /// <summary>
        /// Checks connection of the camera after each frame.
        /// </summary>
        /// <author>Aiden Triffitt</author>
        /// <date>28/04/2017</date>
        private void SetCameras()
        {
            if (!UnityEngine.VR.VRDevice.isPresent)
                m_VRMode = false;

            int layerMask;

            if (m_VRMode)
            {
                layerMask = 1 << 0;
                for (int i = 1; i <= LayerMask.NameToLayer("Occluded"); i++)
                {
                    //Bit shift the index of the layer i to get a bit mask
                    layerMask += 1 << i;
                }

                m_mainCam.GetComponent<Camera>().cullingMask = layerMask;

                //m_mainCam.SetActive(true);
                //sets m_mainCam to occupy only the top half of the screen
                m_mainCam.GetComponent<Camera>().rect = new Rect(0, 0.5f, 1, 0.5f);

                m_VRCam.SetActive(true);
                m_VRManager.SetActive(true);
            }
            else
            {
                //Bit shift the index of the layer (8[Occluded]) to get a bit mask
                layerMask = 1 << 8;
                //This would show the camera only occluded layer
                //We want to see everything but the occluded layer. The ~ operator inverts the bitmask.
                layerMask = ~layerMask;
                m_mainCam.GetComponent<Camera>().cullingMask = layerMask;

                //sets m_mainCam to occupy the full screen
                m_mainCam.GetComponent<Camera>().rect = new Rect(0, 0, 1, 1);

                //m_mainCam.SetActive(true);                
                m_VRCam.SetActive(false);
                m_VRManager.SetActive(false);

                try
                {
                    //turns kinect off if no VR found
                    GameObject.Find("SportsKinematics/CaptureFacade").GetComponent<CaptureFacade>().MultiManager.KinectSensor.Close();
                }
                catch
                {

                }
            }
        }

        /// <summary>
        /// Calls the Set Cameras function to ensure first frame has visuals.
        /// </summary>    
        /// <author>Aiden Triffitt</author>
        /// <date>28/04/2017</date>
        // Use this for initialization
        void Start()
        {
            SetCameras();
        }

        /// <summary>
        /// Operates after every frame, so the Cameras connection can be checked constantly.
        /// Using Fixed Update lowers load on Update
        /// </summary>
        /// <author>Aiden Triffitt</author>
        /// <date>28/04/2017</date>
        private void FixedUpdate()
        {
            m_frame++;

            if (m_frame == m_frameMax)
            {
                SetCameras();
                m_frame = 0;
            }
        }
    }
}