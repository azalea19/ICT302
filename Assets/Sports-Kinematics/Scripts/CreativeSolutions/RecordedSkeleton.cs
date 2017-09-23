using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System.Globalization;

namespace SportsKinematics
{
    public class RecordedSkeleton : MonoBehaviour
    {
        public GameObject m_simManager;
        SortedList<float, SkeletonData> actionSkeletonData = new SortedList<float, SkeletonData>();
        List<float> actionSkeletonKeys = new List<float>();
        List<GameObject> jointObjs = new List<GameObject>();
        public GameObject jointPrefab;

        private void Start()
        {

        }

        void Update()
        {
            for(int i = 0; i < actionSkeletonData[0].joints.Count; i++)
            {
                if(!(GetPosition(GetCurrentTime(), actionSkeletonData[0].joints[i].name) == Vector3.zero))
                    jointObjs[i].transform.position = GetPosition(GetCurrentTime(), actionSkeletonData[0].joints[i].name);
            }
        }

        public void SetData(SortedList<float, SkeletonData> newData)
        {
            actionSkeletonData = newData;
            actionSkeletonKeys = new List<float>(actionSkeletonData.Keys);
            actionSkeletonKeys.Sort();
            jointObjs = new List<GameObject>();
            for (int i = 0; i < actionSkeletonData[0].joints.Count; i++)
            {
                GameObject tmp = Instantiate(jointPrefab, this.transform);
                jointObjs.Add(tmp);
            }
        }

        private float GetCurrentTime()
        {
            return m_simManager.GetComponent<SimulationManager>().m_time;
        }

        SJoint FindJoint(string jointName, SkeletonData data)
        {
            for(int i = 0; i < data.joints.Count; i++)
            {
                if(string.Equals(data.joints[i].name, jointName))
                {
                    return data.joints[i];
                }
            }

            Debug.Log("No Joint Found");
            return new SJoint();
        }

        Vector3 GetPosition(float currentTime, string jointName)   //Gets the balls position at a specified frame from the data files, and interpolates value if the exact data cant be found
        {
            Vector3 pos = new Vector3();
            SkeletonData tmp = new SkeletonData();

            if (actionSkeletonData.ContainsKey(currentTime)) //if the list contains exactly this time
            {
                actionSkeletonData.TryGetValue(currentTime, out tmp);
                SJoint tmpJoint = FindJoint(jointName, tmp);
                pos = tmpJoint.position;
            }
            else
            {
                if (!(~actionSkeletonKeys.BinarySearch(currentTime) >= actionSkeletonKeys.Count))    //If there is a key that exists above the current time
                {

                    int higherTimeIndex = ~actionSkeletonKeys.BinarySearch(currentTime);  //finds the key time above the current time
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
                    float smallerTime = actionSkeletonKeys[lowerTimeIndex];
                    actionSkeletonData.TryGetValue(smallerTime, out tmp);
                    SJoint tmpJoint = FindJoint(jointName, tmp);
                    Vector3 smallerPos = tmpJoint.position;

                    float higherTime = actionSkeletonKeys[higherTimeIndex];
                    actionSkeletonData.TryGetValue(higherTime, out tmp);
                    tmpJoint = FindJoint(jointName, tmp);
                    Vector3 higherPos = tmpJoint.position;


                    //find the percentage between the 2 times
                    float lerpVal = (currentTime - smallerTime) / (higherTime - smallerTime);
                    //lerp between the 2 points based on the time percentage
                    pos = Vector3.Lerp(smallerPos, higherPos, lerpVal);
                }
                else
                {
                    //pos = this.transform.position; //if no data could be found (potentially out of recording data)
                }
            }
            Debug.Log(pos);
            return pos;
        }
    }
}
