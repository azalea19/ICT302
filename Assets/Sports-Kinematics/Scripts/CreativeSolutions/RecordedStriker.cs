using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System.Globalization;

namespace SportsKinematics
{
    public class RecordedStriker : Striker
    {
        private SortedList<float, StrikerData> m_actionStrikerData;   //The recorded ball data for this recording
        private List<float> m_actionStrikerKeys;   //The time keys, used to get data and interpolate

        void Start()
        {
            m_actionStrikerData = new SortedList<float, StrikerData>();
            m_actionStrikerKeys = new List<float>();

            switch (m_StrikerSelection)
            {
                case StrikerSelection.TableTennis:
                    m_currentStriker = Object.Instantiate(m_tableTennisBat, this.transform);
                    break;
                case StrikerSelection.Catch:
                    m_currentStriker = Object.Instantiate(m_catch, this.transform);
                    break;
            }

        }

        // Update is called once per frame
        void Update()
        {
            this.transform.position = GetPosition(GetCurrentTime());
            this.transform.rotation = Quaternion.Euler(GetRotation(GetCurrentTime()));

            if (GetOcclusion(GetCurrentTime()) == true)     //If the file says the ball should be occluded this frame
            {
                m_currentStriker.GetComponent<Renderer>().enabled = false;      //Then make the object invisible
            }
            else
            {
                m_currentStriker.GetComponent<Renderer>().enabled = true;   //make the object visible
            }
        }

        void OnCollisionEnter(Collision collision)
        {
            //May need to add code in here which detects collision with opponents paddle only
            //This could be used for finding the time the ball is hit, and potentially occluding it when
            //this occurs
        }

        float GetCurrentTime()       //Gets the current total time for this recording
        {
            return m_simManager.GetComponent<SimulationManager>().m_time;
        }

        bool GetOcclusion(float currentTime)     //Returns whether the ball is occluded at this frame in the config file
        {
            bool isOcc = false;
            StrikerData tmp;
            if (m_actionStrikerData.ContainsKey(currentTime)) //if the list contains exactly this time
            {

                m_actionStrikerData.TryGetValue(currentTime, out tmp);
                isOcc = tmp.isOccluded;
            }
            else
            {
                if (!(~m_actionStrikerKeys.BinarySearch(currentTime) >= m_actionStrikerKeys.Count))    //If there is a key that exists above the current time
                {
                    int occIndex = ~m_actionStrikerKeys.BinarySearch(currentTime);
                    m_actionStrikerData.TryGetValue(m_actionStrikerKeys[occIndex], out tmp);
                    isOcc = tmp.isOccluded;
                }
            }
            return isOcc;
        }

        public void SetData(SortedList<float, StrikerData> newStrikerData)
        {
            m_actionStrikerData = newStrikerData;
            m_actionStrikerKeys = new List<float>(newStrikerData.Keys);
            m_actionStrikerKeys.Sort();
        }

        Vector3 GetPosition(float currentTime)   //Gets the balls position at a specified frame from the data files, and interpolates value if the exact data cant be found
        {
            StrikerData ball = new StrikerData();

            if (m_actionStrikerData.ContainsKey(currentTime)) //if the list contains exactly this time
            {
                m_actionStrikerData.TryGetValue(currentTime, out ball);
            }
            else
            {
                if (!(~m_actionStrikerKeys.BinarySearch(currentTime) >= m_actionStrikerKeys.Count))    //If there is a key that exists above the current time
                {
                    int higherTimeIndex = ~m_actionStrikerKeys.BinarySearch(currentTime);  //finds the key time above the current time
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
                    StrikerData tmp = new StrikerData();

                    float smallerTime = m_actionStrikerKeys[lowerTimeIndex];
                    m_actionStrikerData.TryGetValue(smallerTime, out tmp);
                    Vector3 smallerPos = tmp.position;

                    float higherTime = m_actionStrikerKeys[higherTimeIndex];
                    m_actionStrikerData.TryGetValue(higherTime, out tmp);
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

        Vector3 GetRotation(float currentTime)   //Gets the balls position at a specified frame from the data files, and interpolates value if the exact data cant be found
        {
            StrikerData ball = new StrikerData();

            if (m_actionStrikerData.ContainsKey(currentTime)) //if the list contains exactly this time
            {
                m_actionStrikerData.TryGetValue(currentTime, out ball);
            }
            else
            {
                if (!(~m_actionStrikerKeys.BinarySearch(currentTime) >= m_actionStrikerKeys.Count))    //If there is a key that exists above the current time
                {
                    int higherTimeIndex = ~m_actionStrikerKeys.BinarySearch(currentTime);  //finds the key time above the current time
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
                    StrikerData tmp = new StrikerData();

                    float smallerTime = m_actionStrikerKeys[lowerTimeIndex];
                    m_actionStrikerData.TryGetValue(smallerTime, out tmp);
                    Vector3 smallerRot = tmp.rotation;

                    float higherTime = m_actionStrikerKeys[higherTimeIndex];
                    m_actionStrikerData.TryGetValue(higherTime, out tmp);
                    Vector3 higherRot = tmp.rotation;

                    //find the percentage between the 2 times
                    float lerpVal = (currentTime - smallerTime) / (higherTime - smallerTime);
                    //lerp between the 2 points based on the time percentage
                    ball.rotation = Vector3.Lerp(smallerRot, higherRot, lerpVal);
                }
                else
                {
                    ball.rotation = this.transform.position; //if no data could be found (potentially out of recording data)
                }
            }

            return ball.rotation;
        }
    }
}