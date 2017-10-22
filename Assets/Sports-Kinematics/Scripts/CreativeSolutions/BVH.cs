using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System.Globalization;
using System.Linq;

namespace SportsKinematics
{
    //Based on the design by Dr Masaki OSHITA
    public class BVH : MonoBehaviour
    {
        public enum ChannelEnum
        {
            X_ROTATION,
            Y_ROTATION,
            Z_ROTATION,
            X_POSITION,
            Y_POSITION,
            Z_POSITION
        };

        public struct Channel
        {
            public Joint joint;
            public ChannelEnum type;
            public int index;
        };

        public class Joint
        {
            public string name;
            public int index;

            public Joint parent;
            public List<Joint> children = new List<Joint>();

            public Vector3 offset;
            public bool has_site;
            public Vector3 site;
            public List<Channel> channels = new List<Channel>();
        };

        bool is_load_success { get; set; }
        string file_name { get; set; }
        string motion_name { get; set; }

        public int num_channel { get; set; }
        public List<Channel> channels { get; set; }
        public List<Joint> joints { get; set; }
        public Dictionary<string, Joint> joint_index { get; set; }

        public int num_frame { get; set; }
        public double interval { get; set; }
        public double[]motion { get; set; }

        Channel GetChannel(int no)
        {
            return channels[no];
        }

        Joint GetJoint(string j)
        {
            return joint_index[j];
        }

        Joint GetJoint(char j)
        {
            return joint_index[j.ToString()];
        }

        public BVH()
        {
            Clear();
        }

        public BVH(string bvh_file_name)
        {
            Clear();
            Load(bvh_file_name);
        }

        void Clear()
        {
            channels = new List<Channel>();
            joints = new List<Joint>();
            is_load_success = false;

            file_name = "";
            motion_name = "";

            num_channel = 0;
            joint_index = new Dictionary<string, Joint>();

            num_frame = 0;
            interval = 0.0;
        }

        void Load(string bvh_file_name )
        {
            List<Joint> joint_stack = new List<Joint>();
            Joint joint = null;
            Joint new_joint = null;
            bool is_site = false;
            double x, y, z;
            int i, j;


            Clear();

            using (var reader = new StreamReader(bvh_file_name))
            {
                string line;
                string[] data;
                while (!reader.EndOfStream)
                {
                    line = reader.ReadLine();
                    line = line.Replace("\t", string.Empty);

                    data = line.Split(new char[] { ' ', ':', ',', '/', '\'' });

                    if (line != null && line != string.Empty)
                    {

                        if (data[0].Equals("{"))
                        {
                            joint_stack.Add(joint);
                            joint = new_joint;
                            continue;
                        }

                        if (data[0].Equals("}"))
                        {
                            joint = joint_stack[joint_stack.Count - 1];
                            joint_stack.RemoveAt(joint_stack.Count - 1);
                            is_site = false;
                            continue;
                        }

                        if (data[0].Equals("ROOT") || data[0].Equals("JOINT"))
                        {
                            new_joint = new Joint();
                            new_joint.index = joints.Count - 1;
                            new_joint.parent = joint;
                            new_joint.has_site = false;
                            new_joint.offset[0] = 0.0f;
                            new_joint.offset[1] = 0.0f;
                            new_joint.offset[2] = 0.0f;
                            new_joint.site[0] = 0.0f;
                            new_joint.site[1] = 0.0f;
                            new_joint.site[2] = 0.0f;
                            joints.Add(new_joint);
                            if (joint != null)
                            {
                                joint.children.Add(new_joint);
                            }
                                

                            new_joint.name = data[1];

                            joint_index[new_joint.name] = new_joint;
                            continue;
                        }

                        if (data[0].Equals("End"))
                        {
                            new_joint = joint;
                            is_site = true;
                            continue;
                        }

                        if (data[0].Equals("OFFSET"))
                        {
                            x = double.Parse(data[1]);
                            y = double.Parse(data[2]);
                            z = double.Parse(data[3]);

                            if (is_site)
                            {
                                joint.has_site = true;
                                joint.site.x = (float)x;
                                joint.site.y = (float)y;
                                joint.site.z = (float)z;
                            }
                            else
                            {
                                joint.offset.x = (float)x;
                                joint.offset.y = (float)y;
                                joint.offset.z = (float)z;
                            }
                            continue;
                        }

                        if (data[0].Equals("CHANNELS"))
                        {
                            for (i = 0; i < int.Parse(data[1]); i++)
                            {
                                Channel channel = new Channel();
                                channel.joint = joint;
                                channel.index = channels.Count;

                                if (data[2 + i].Equals("Xrotation"))
                                    channel.type = ChannelEnum.X_ROTATION;
                                else if (data[2 + i].Equals("Yrotation"))
                                    channel.type = ChannelEnum.Y_ROTATION;
                                else if (data[2 + i].Equals("Zrotation"))
                                    channel.type = ChannelEnum.Z_ROTATION;
                                else if (data[2 + i].Equals("Xposition"))
                                    channel.type = ChannelEnum.X_POSITION;
                                else if (data[2 + i].Equals("Yposition"))
                                    channel.type = ChannelEnum.Y_POSITION;
                                else if (data[2 + i].Equals("Zposition"))
                                    channel.type = ChannelEnum.Z_POSITION;

                                channels.Add(channel);
                                joint.channels.Add(channel);
                            }
                        }

                        if (data[0].Equals("MOTION"))
                            break;
                    }
                }

                line = reader.ReadLine();
                data = line.Split(new char[] {':', ',', '.', '/', 't' });
                if (!data[0].Equals("Frames"))
                    goto bvh_error;

                if (data[1].Equals(null))
                    goto bvh_error;

                num_frame = int.Parse(data[1]);

                line = reader.ReadLine();
                data = line.Split(new char[] {':'});

                if (!data[0].Equals("Frame Time"))
                    goto bvh_error;

                if (data[1].Equals(null))
                    goto bvh_error;

                interval = double.Parse(data[1]);
                num_channel = channels.Count;
                motion = new double[num_frame * num_channel];

                for (i = 0; i < num_frame; i++)
                {
                    line = reader.ReadLine();
                    data = line.Split(new char[] { ' ', ':', ',', '/', 't' });

                    for (j = 0; j < num_channel; j++)
                    {
                        if (data[j].Equals(null))
                            goto bvh_error;
                        motion[i * num_channel + j] = double.Parse(data[j]);
                    }
                }

                is_load_success = true;
            }

            return;

            bvh_error:
                Debug.Log("BVH load error");
        }

        List<SJoint> GetJointPoints(Joint joint, double[] data, int startIndex, float scale, Vector3 parentBonePos, Vector3 parentBoneRot)
        {
            List<SJoint> allJoints = new List<SJoint>();
            SJoint currentJoint = new SJoint();
            currentJoint.name = joint.name;

            if (joint.parent == null )
	        {
                currentJoint.position = new Vector3((float)data[startIndex] * scale, (float)data[startIndex + 1] * scale, (float)data[startIndex + 2] * scale) + parentBonePos;
                parentBonePos = currentJoint.position;
            }
	        else
	        {
                currentJoint.position = new Vector3(joint.offset[0] * scale, joint.offset[1] * scale, joint.offset[2] * scale) + parentBonePos;
            }

            float x = 0;
            float y = 0;
            float z = 0;

            for (int i = 0; i < joint.channels.Count; i++)
	        {

                Channel channel = joint.channels[i];
                if (channel.type == ChannelEnum.X_ROTATION)
                {
                    z = (float)data[channel.index + startIndex] + parentBoneRot.z;
                }
                else if (channel.type == ChannelEnum.Y_ROTATION)
                {
                    y = (float)data[channel.index + startIndex] + parentBoneRot.y;
                }
                else if (channel.type == ChannelEnum.Z_ROTATION)
                {
                    x = (float)data[channel.index + startIndex] + parentBoneRot.x;
                }

	        }
            if (joint.parent == null)
            {
                x = 0;
                y = 0;
                z = 0;
            }

                currentJoint.position = RotateAroundPoint(currentJoint.position, parentBonePos, new Vector3(x, y, z));


            //parentBoneRot += new Vector3(x, y, z);

            allJoints.Add(currentJoint);

            Vector3 tmp = new Vector3();
            SJoint stmp = new SJoint();

            for (int i = 0; i < joint.children.Count; i++ )
	        {
                List<SJoint> returnedJoints = GetJointPoints(joint.children[i], data, startIndex, scale, currentJoint.position, parentBoneRot);
                for(int k = 0; k < returnedJoints.Count; k++)
                {
                    
                    tmp = RotateAroundPoint(returnedJoints[k].position, parentBonePos, new Vector3(x, y, z));
                    stmp = returnedJoints[k];
                    stmp.position = tmp;
                    returnedJoints[k] = stmp;
                    allJoints.Add(returnedJoints[k]);
                }
	        }

            return allJoints;
        }

        public SortedList<float, SkeletonData> GetActionSkeletonData(float scale)
        {
            SortedList<float, SkeletonData> skelData = new SortedList<float, SkeletonData>();
            for (int i = 0; i <= 2000; i++)
            {
                List<SJoint> tmp = GetJointPoints(joints[0], motion, i * num_channel, scale, new Vector3(0,0,0), new Vector3(0,0,0));
                SkeletonData tmpSkel = new SkeletonData();
                tmpSkel.joints = tmp;
                skelData.Add(i * (float)interval, tmpSkel);
            }

            return skelData;
        }

        Vector3 RotateAroundPoint(Vector3 pointToRotate, Vector3 pivot, Vector3 angle)
        {
            Vector3 direction = pointToRotate - pivot;
            direction = Quaternion.Euler(angle) * direction;
            pointToRotate = direction + pivot;
            return pointToRotate;
        }
    }
}