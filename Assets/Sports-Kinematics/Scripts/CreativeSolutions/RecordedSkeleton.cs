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
        BVH actionSkeletonData;
        List<float> actionSkeletonKeys = new List<float>();
        List<GameObject> jointObjs = new List<GameObject>();
        public GameObject jointPrefab;

        private void Start()
        {

        }

        void Update()
        {
            Vector3 tran = new Vector3(0, -5, 0);

            DrawSkeletonAtTime(GetCurrentTime(), actionSkeletonData.joints[0]);

            /*for(int i = 0; i < actionSkeletonData[0].joints.Count; i++)
            {
                if(!(GetPosition(GetCurrentTime(), actionSkeletonData[0].joints[i].name) == Vector3.zero))
                {
                    jointObjs[i].transform.position = GetPosition(GetCurrentTime(), actionSkeletonData[0].joints[i].name) + tran;
                    if(i + 1 < actionSkeletonData[0].joints.Count)
                        Debug.DrawLine(jointObjs[i].transform.position, jointObjs[i + 1].transform.position, Color.red, 1.0f, false);
                }
            }*/
        }

        private void DrawSkeletonAtTime(float time, BVH.Joint currentJoint)
        {
            float x = 0;
            float y = 0;
            float z = 0;

            int lowerFrame = Mathf.CeilToInt(time / ((float)actionSkeletonData.interval * 1));
            int frameMotionIndex = lowerFrame * (actionSkeletonData.num_channel);
            GameObject currentObj = new GameObject();
            List<GameObject> childrenObjs = new List<GameObject>();

            for (int i = 0; i < jointObjs.Count; i++)
            {
                if (string.Equals(jointObjs[i].name, currentJoint.name))
                {
                    currentObj = jointObjs[i];
                }
            }

            for(int k = 0; k < currentJoint.children.Count; k ++)
            {
                for (int i = 0; i < jointObjs.Count; i++)
                {
                    if (string.Equals(jointObjs[i].name, currentJoint.children[k].name))
                    {
                        childrenObjs.Add(jointObjs[i]);
                    }
                }
            }

            if (string.Equals(currentJoint.name, "Hips"))
            {
                currentObj.transform.localPosition = new Vector3((float)actionSkeletonData.motion[frameMotionIndex],
                    (float)actionSkeletonData.motion[frameMotionIndex + 1],
                    (float)actionSkeletonData.motion[frameMotionIndex + 2]);
            }

            for (int i = 0; i < currentJoint.channels.Count; i++)
            {
                BVH.Channel channel = currentJoint.channels[i];

                if (channel.type == BVH.ChannelEnum.X_ROTATION)
                {
                    x = (float)actionSkeletonData.motion[channel.index + frameMotionIndex];                 
                }
                else if (channel.type == BVH.ChannelEnum.Y_ROTATION)
                {
                    y = (float)actionSkeletonData.motion[channel.index + frameMotionIndex];
                }
                else if (channel.type == BVH.ChannelEnum.Z_ROTATION)
                {
                    z = (float)actionSkeletonData.motion[channel.index + frameMotionIndex];
                }
            }


            Quaternion tmpQuaternion = Quaternion.Euler(new Vector3(x, y, z));
            currentObj.transform.localRotation = tmpQuaternion;


            for(int i = 0; i < currentJoint.children.Count; i++)
            {
                DrawBone(currentObj, childrenObjs[i]);
                DrawSkeletonAtTime(time, currentJoint.children[i]);
            }

        }

        public void DrawBone(GameObject startJoint, GameObject endJoint)
        {
            startJoint.AddComponent<LineRenderer>();
            LineRenderer lr = startJoint.GetComponent<LineRenderer>();
            lr.material = new Material(Shader.Find("Particles/Alpha Blended Premultiply"));
            lr.startColor = Color.red;
            lr.startWidth = 0.1f;
            lr.SetPosition(0, startJoint.transform.position);
            lr.SetPosition(1, endJoint.transform.position);
        }

        public void SetData(BVH newData)
        {
            actionSkeletonData = newData;
            //actionSkeletonKeys = new List<float>(actionSkeletonData.Keys);
            //actionSkeletonKeys.Sort();
            DeleteSkeleton(jointObjs);
            jointObjs = new List<GameObject>();

            GameObject root = Instantiate(jointPrefab, this.transform);
            root.name = newData.joints[0].name;
            root.transform.localPosition = newData.joints[0].offset;
            jointObjs.Add(root);
            CreateSkeleton(newData.joints[0], root.transform);

        }

        private void DeleteSkeleton(List<GameObject> allJoints)
        {
            for(int i = 0; i < allJoints.Count; i++)
            {
                GameObject.Destroy(allJoints[i]);
            }
        }

        private void CreateSkeleton(BVH.Joint joint, Transform parentJoint)
        {

            for (int i = 0; i < joint.children.Count; i++)
            {
                GameObject tmp = Instantiate(jointPrefab, parentJoint);
                tmp.transform.localPosition = joint.children[i].offset;
                tmp.name = joint.children[i].name;

                tmp.AddComponent<LineRenderer>();
                LineRenderer lr = tmp.GetComponent<LineRenderer>();
                lr.material = new Material(Shader.Find("Particles/Alpha Blended Premultiply"));
                lr.startColor = Color.red;
                lr.startWidth = 0.1f;
                lr.SetPosition(0, tmp.transform.position);
                lr.SetPosition(1, parentJoint.position);

                jointObjs.Add(tmp);
                CreateSkeleton(joint.children[i], tmp.transform);
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

     /*   Vector3 GetPosition(float currentTime, string jointName)   //Gets the balls position at a specified frame from the data files, and interpolates value if the exact data cant be found
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
            return pos;
        }*/
    }
}
