using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System.Globalization;

namespace SportsKinematics
{
    public class RecordedBall : MonoBehaviour
    {
        public GameObject m_playerPaddle;       //The player's paddle, used to detect the distance between the ball and paddle
        public GameObject m_results;

        private SortedList<float, BallData> m_actionBallData = new SortedList<float, BallData>();   //The recorded ball data for this recording
        private List<float> m_actionBallKeys = new List<float>();   //The time keys, used to get data and interpolate

        public AudioClip m_hitSound;
        public GameObject m_simManager;


        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {
            this.transform.position = GetPosition(GetCurrentTime());

            if (GetOcclusion(GetCurrentTime()) == true)     //If the file says the ball should be occluded this frame
            {
                this.GetComponent<Renderer>().enabled = false;      //Then make the object invisible
            }
            else
            {
                this.GetComponent<Renderer>().enabled = true;   //make the object visible
            }

        }

        void OnCollisionEnter(Collision collision)
        {
            if (collision.gameObject.transform.parent.gameObject.name == "vive_Striker")
            {
                if (!m_results.GetComponent<SimulationResults>().m_isHitByPlayer)
                {
                    m_results.GetComponent<SimulationResults>().m_isHitByPlayer = true;
                    m_results.GetComponent<SimulationResults>().m_hitTime = GetCurrentTime();
                }
            }

            AudioSource.PlayClipAtPoint(m_hitSound, this.transform.position);
        }

        float GetCurrentTime()       //Gets the current total time for this recording
        {
            return m_simManager.GetComponent<SimulationManager>().m_time;
        }

        bool GetOcclusion(float currentTime)     //Returns whether the ball is occluded at this frame in the config file
        {
            bool isOcc = false;
            BallData tmp = new BallData();
            if (m_actionBallData.ContainsKey(currentTime)) //if the list contains exactly this time
            {

                m_actionBallData.TryGetValue(currentTime, out tmp);
                isOcc = tmp.isOccluded;
            }
            else
            {
                if (!(~m_actionBallKeys.BinarySearch(currentTime) >= m_actionBallKeys.Count))    //If there is a key that exists above the current time
                {
                    int occIndex = ~m_actionBallKeys.BinarySearch(currentTime);
                    m_actionBallData.TryGetValue(m_actionBallKeys[occIndex], out tmp);
                    isOcc = tmp.isOccluded;
                }
            }
            return isOcc;
        }

        public void SetData(SortedList<float, BallData> newBallData)
        {
            m_actionBallData = newBallData;
            m_actionBallKeys = new List<float>(newBallData.Keys);
            
            m_actionBallKeys.Sort();
        }

        Vector3 GetPosition(float currentTime)   //Gets the balls position at a specified frame from the data files, and interpolates value if the exact data cant be found
        {
            BallData ball = new BallData();
            
            if (m_actionBallData.ContainsKey(currentTime)) //if the list contains exactly this time
            {
                m_actionBallData.TryGetValue(currentTime, out ball);
            }
            else
            {
                if (!(~m_actionBallKeys.BinarySearch(currentTime) >= m_actionBallKeys.Count))    //If there is a key that exists above the current time
                {
                    
                    int higherTimeIndex = ~m_actionBallKeys.BinarySearch(currentTime);  //finds the key time above the current time
                    int lowerTimeIndex;
                    if (higherTimeIndex > 0)  //If the higher time is larger than 0, (if it is 0, then a lower time of -1 will return an error)
                    {
                        lowerTimeIndex = higherTimeIndex - 1;    //get the index below the current one
                    }
                    else
                    {
                        lowerTimeIndex = higherTimeIndex;
                    }

                    //Get data for these index's
                    BallData tmp = new BallData();

                    float smallerTime = m_actionBallKeys[lowerTimeIndex];
                    m_actionBallData.TryGetValue(smallerTime, out tmp);
                    Vector3 smallerPos = tmp.position;

                    float higherTime = m_actionBallKeys[higherTimeIndex];
                    m_actionBallData.TryGetValue(higherTime, out tmp);
                    Vector3 higherPos = tmp.position;


                    //find the percentage between the 2 times
                    float lerpVal = (currentTime - smallerTime) / (higherTime - smallerTime);
                    //lerp between the 2 points based on the time percentage
                    ball.position = Vector3.Lerp(smallerPos, higherPos, lerpVal);
                }
                else
                {
                    ball.position = this.transform.position; //if no data could be found (potentially out of recording data)
                }
            }

            return ball.position;
        }
    }
}
