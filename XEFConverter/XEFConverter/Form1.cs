using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.IO;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Media.Media3D;
using Microsoft.Kinect;
using Microsoft.Kinect.Tools;
using System.Threading;
using System.Collections;


namespace XEFConverter
{
    public partial class Form1 : Form
    {
        private Thread t_LogPointData; //Not quite sure why we need this, maybe because is pretty cool
        private Thread t_LogFrameData; //Not quite sure why we need this, maybe because is pretty cool
        private Thread t_LogDepthData; //Not quite sure why we need this, maybe because is pretty cool

        private List<Dictionary<string, float[]>> m_pointPosition;
        private List<Dictionary<JointOrientation, float[]>> m_pointOrientation;
        private new List<Dictionary<string, float[]>> m_frames;
        //private List<List<Dictionary<JointType, float[]>>> storage;

        private int count;
        private bool ready = false;
        private int m_frameCount = 0;
        private KinectSensor m_sensor = null;
        private Body[] bodies = null;
        BodyFrameReader bReader = null;

        //BVHBone spineBase;
        //BVHBone spineMid;
        //BVHBone spineShoulder;
        //BVHBone neck;
        //BVHBone head;
        //
        //
        //BVHBone shoulderLeft;
        //BVHBone shoulderRight;
        //BVHBone elbowRight;
        //BVHBone elbowLeft;
        //BVHBone wristLeft;
        //BVHBone wristRight;
        //BVHBone handRight;
        //BVHBone handLeft;
        //BVHBone handTipRight;
        //BVHBone handTipLeft;
        //BVHBone thumbLeft;
        //BVHBone thumbRight;
        //
        //BVHBone hipRight;
        //BVHBone hipLeft;
        //BVHBone kneeLeft;
        //BVHBone kneeRight;
        //BVHBone ankleRight;
        //BVHBone ankleLeft;
        //BVHBone footRight;
        //BVHBone footLeft;


        public Form1()
        {
            InitializeComponent();
            m_pointPosition = new List<Dictionary<string, float[]>>();
            m_pointOrientation = new List<Dictionary<JointOrientation, float[]>>();
            // m_frames = new List<List<Dictionary<Joint, float[]>>>();
            m_frames = new List<Dictionary<string, float[]>>();

            m_frameCount = 0;
            count = 0;
            InitializeKinect();

            //spineBase = new BVHBone(null, "SpineBase", 6, TransAxis.None, true);
            //spineMid = new BVHBone(spineBase, "SpineMid", 3, TransAxis.None, true);
            //spineShoulder = new BVHBone(spineMid, "spineShoulder", 3, TransAxis.None, true);
            //neck = new BVHBone(spineShoulder, "neck", 3, TransAxis.None, true);
            //head = new BVHBone(neck, "head", 3, TransAxis.None, true);
            //
            //shoulderLeft = new BVHBone(spineShoulder, "shoulderLeft", 3, TransAxis.None, true);
            //shoulderRight = new BVHBone(spineShoulder, "shoulderRight", 3, TransAxis.None, true);
            //elbowRight = new BVHBone(shoulderRight, "elbowRight", 3, TransAxis.None, true);
            //elbowLeft = new BVHBone(shoulderLeft, "elbowLeft", 3, TransAxis.None, true);
            //wristLeft = new BVHBone(elbowLeft, "wristLeft", 3, TransAxis.None, true);
            //wristRight = new BVHBone(elbowRight, "wristRight", 3, TransAxis.None, true);
            //handRight = new BVHBone(wristRight, "handRight", 3, TransAxis.None, true);
            //handLeft = new BVHBone(wristLeft, "handLeft", 3, TransAxis.None, true);
            //handTipRight = new BVHBone(handRight, "handTipRight", 3, TransAxis.None, true);
            //handTipLeft = new BVHBone(handLeft, "handTipLeft", 3, TransAxis.None, true);
            //thumbLeft = new BVHBone(handLeft, "thumbLeft", 3, TransAxis.None, true);
            //thumbRight = new BVHBone(handRight, "thumbRight", 3, TransAxis.None, true);
            //
            //hipRight = new BVHBone(spineBase, "hipRight", 3, TransAxis.None, true);
            //hipLeft = new BVHBone(spineBase, "hipLeft", 3, TransAxis.None, true);
            //kneeLeft = new BVHBone(hipLeft, "kneeLeft", 3, TransAxis.None, true);
            //kneeRight = new BVHBone(hipRight, "kneeRight", 3, TransAxis.None, true);
            //ankleRight = new BVHBone(kneeRight, "ankleRight", 3, TransAxis.None, true);
            //ankleLeft = new BVHBone(kneeLeft, "ankleLeft", 3, TransAxis.None, true);
            //footRight = new BVHBone(ankleRight, "footRight", 3, TransAxis.None, true);
            //footLeft = new BVHBone(ankleLeft, "footLeft", 3, TransAxis.None, true);


        }

        private void InitializeKinect()
        {
            m_sensor = KinectSensor.GetDefault();
            if (m_sensor != null)
            {
                m_sensor.Open();
            }

            bReader = m_sensor.BodyFrameSource.OpenReader();
            if (bReader != null)
            {
                bReader.FrameArrived += Reader_FrameArrived;
            }
        }

        /// <summary>
        /// Allows to convert positions from global  space to local.
        /// </summary>
        /// <param name="parent">parent local position</param>
        /// <param name="child">child global position</param>
        /// <returns>child local position</returns>
        private float[] ConvertToLocalSpace(float[] parent, float[] child)
        {
            float[] result = new float[3];
            for (int i = 0; i < 3; i++)
            {
                result[i] = child[i] - parent[i];
            }
            return result;
        }

        /// <summary>
        /// Uses the kinect hierarchy to convert joint positions 
        /// from global to local space coordinates.
        /// </summary>
        private void ConvertJointPositions()
        {
            for (int i = 0; i < m_frames.Count(); i++)
            {
                m_frames[i]["spineMid"] = ConvertToLocalSpace(m_frames[i]["spineBase"], m_frames[i]["spineMid"]);
                m_frames[i]["spineShoulder"] = ConvertToLocalSpace(m_frames[i]["spineMid"], m_frames[i]["spineShoulder"]);
                m_frames[i]["neck"] = ConvertToLocalSpace(m_frames[i]["spineShoulder"], m_frames[i]["neck"]);
                m_frames[i]["head"] = ConvertToLocalSpace(m_frames[i]["neck"], m_frames[i]["head"]);

                m_frames[i]["shoulderLeft"] = ConvertToLocalSpace(m_frames[i]["spineShoulder"], m_frames[i]["shoulderLeft"]);
                m_frames[i]["elbowLeft"] = ConvertToLocalSpace(m_frames[i]["shoulderLeft"], m_frames[i]["elbowLeft"]);
                m_frames[i]["wristLeft"] = ConvertToLocalSpace(m_frames[i]["elbowLeft"], m_frames[i]["wristLeft"]);
                m_frames[i]["thumbLeft"] = ConvertToLocalSpace(m_frames[i]["wristLeft"], m_frames[i]["thumbLeft"]);
                m_frames[i]["handLeft"] = ConvertToLocalSpace(m_frames[i]["wristLeft"], m_frames[i]["handLeft"]);
                m_frames[i]["handTipLeft"] = ConvertToLocalSpace(m_frames[i]["handLeft"], m_frames[i]["handTipLeft"]);

                m_frames[i]["shoulderRight"] = ConvertToLocalSpace(m_frames[i]["spineShoulder"], m_frames[i]["shoulderRight"]);
                m_frames[i]["elbowRight"] = ConvertToLocalSpace(m_frames[i]["shoulderRight"], m_frames[i]["elbowRight"]);
                m_frames[i]["wristRight"] = ConvertToLocalSpace(m_frames[i]["elbowRight"], m_frames[i]["wristRight"]);
                m_frames[i]["thumbRight"] = ConvertToLocalSpace(m_frames[i]["wristRight"], m_frames[i]["thumbRight"]);
                m_frames[i]["handRight"] = ConvertToLocalSpace(m_frames[i]["wristRight"], m_frames[i]["handRight"]);
                m_frames[i]["handTipRight"] = ConvertToLocalSpace(m_frames[i]["handRight"], m_frames[i]["handTipRight"]);

                m_frames[i]["hipLeft"] = ConvertToLocalSpace(m_frames[i]["spineBase"], m_frames[i]["hipLeft"]);
                m_frames[i]["kneeLeft"] = ConvertToLocalSpace(m_frames[i]["hipLeft"], m_frames[i]["kneeLeft"]);
                m_frames[i]["ankleLeft"] = ConvertToLocalSpace(m_frames[i]["kneeLeft"], m_frames[i]["ankleLeft"]);
                m_frames[i]["footLeft"] = ConvertToLocalSpace(m_frames[i]["footLeft"], m_frames[i]["footLeft"]);

                m_frames[i]["hipRight"] = ConvertToLocalSpace(m_frames[i]["spineBase"], m_frames[i]["hipRight"]);
                m_frames[i]["kneeRight"] = ConvertToLocalSpace(m_frames[i]["hipRight"], m_frames[i]["kneeRight"]);
                m_frames[i]["ankleRight"] = ConvertToLocalSpace(m_frames[i]["kneeRight"], m_frames[i]["ankleRight"]);
                m_frames[i]["footRight"] = ConvertToLocalSpace(m_frames[i]["footRight"], m_frames[i]["footRight"]);
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {

            m_sensor.Close();
            Console.WriteLine("Data: " + m_frames[0]["spineBase"][3] + " " + m_frames[0]["spineBase"][4] + " " + m_frames[0]["spineBase"][5] + " " + m_frames[0]["spineBase"][6]);
            //Console.WriteLine(m_frames[0]["spineBase"][0] + " " + m_frames[0]["spineBase"][1] + " " + m_frames[0]["spineBase"][2]);
            //Console.WriteLine(m_frames[0]["spineMid"][0] + " " + m_frames[0]["spineMid"][1] + " " + m_frames[0]["spineMid"][2]);
            //Console.WriteLine(m_frames[0]["spineShoulder"][0] + " " + m_frames[0]["spineShoulder"][1] + " " + m_frames[0]["spineShoulder"][2]);
            //Console.WriteLine(m_frames[0]["neck"][0] + " " + m_frames[0]["neck"][1] + " " + m_frames[0]["neck"][2]);
            //Console.WriteLine(m_frames[0]["head"][0] + " " + m_frames[0]["head"][1] + " " + m_frames[0]["head"][2]);
            for (int i = 0; i < m_frames.Count(); i++)
            {
                //toEulerAngles(i);
            }

            ConvertJointPositions();

            Console.WriteLine("Data1: " + m_frames[0]["spineBase"][3] + " " + m_frames[0]["spineBase"][4] + " " + m_frames[4]["spineBase"][5]);
            StreamWriter file;
            string fileName = "test.bvh";
            if (File.Exists(fileName))
                File.Delete(fileName);
            file = File.CreateText(fileName);

            int x = 0;
            int y = 1;
            int z = 2;
            file.WriteLine("HIERARCHY");
            file.WriteLine("ROOT spineBase");
            file.WriteLine("{");
            file.WriteLine("\tOFFSET " + m_frames[0]["spineBase"][x] + " " + m_frames[0]["spineBase"][y] + " " + m_frames[0]["spineBase"][z]);
            file.WriteLine("\tCHANNELS  6 Xposition Yposition Zposition Zrotation Xrotation Yrotation");
            file.WriteLine("\tJOINT spineMid");
            file.WriteLine("\t{");
            file.WriteLine("\t\tOFFSET " + m_frames[0]["spineMid"][x] + " " + m_frames[0]["spineMid"][y] + " " + m_frames[0]["spineMid"][z]);
            file.WriteLine("\t\tCHANNELS 3 Zrotation Xrotation Yrotation");
            file.WriteLine("\t\tJOINT spineShoulder");
            file.WriteLine("\t\t{");
            file.WriteLine("\t\t\tOFFSET " + m_frames[0]["spineShoulder"][x] + " " + m_frames[0]["spineShoulder"][y] + " " + m_frames[0]["spineShoulder"][z]);
            file.WriteLine("\t\t\tCHANNELS 3 Zrotation Xrotation Yrotation");
            file.WriteLine("\t\t\tJOINT neck");
            file.WriteLine("\t\t\t{");
            file.WriteLine("\t\t\t\tOFFSET " + m_frames[0]["neck"][x] + " " + m_frames[0]["neck"][y] + " " + m_frames[0]["neck"][z]);
            file.WriteLine("\t\t\t\tCHANNELS 3 Zrotation Xrotation Yrotation");
            file.WriteLine("\t\t\t\tJOINT head");
            file.WriteLine("\t\t\t\t{");
            file.WriteLine("\t\t\t\t\tOFFSET " + m_frames[0]["head"][x] + " " + m_frames[0]["head"][y] + " " + m_frames[0]["head"][z]);
            file.WriteLine("\t\t\t\t\tCHANNELS 3 Zrotation Xrotation Yrotation");
            file.WriteLine("\t\t\t\t\tEnd Site");
            file.WriteLine("\t\t\t\t\t{");
            file.WriteLine("\t\t\t\t\t\tOFFSET 0 1 0"); /// not sure what this is
            file.WriteLine("\t\t\t\t\t}");
            file.WriteLine("\t\t\t\t}");
            file.WriteLine("\t\t\t}");
            file.WriteLine("\t\t\tJOINT shoulderLeft");
            file.WriteLine("\t\t\t{");
            file.WriteLine("\t\t\t\tOFFSET " + m_frames[0]["shoulderLeft"][x] + " " + m_frames[0]["shoulderLeft"][y] + " " + m_frames[0]["shoulderLeft"][z]);
            file.WriteLine("\t\t\t\tCHANNELS 3 Zrotation Xrotation Yrotation");
            file.WriteLine("\t\t\t\tJOINT elbowLeft");
            file.WriteLine("\t\t\t\t{");
            file.WriteLine("\t\t\t\t\tOFFSET " + m_frames[0]["elbowLeft"][x] + " " + m_frames[0]["elbowLeft"][y] + " " + m_frames[0]["elbowLeft"][z]);
            file.WriteLine("\t\t\t\t\tCHANNELS 3 Zrotation Xrotation Yrotation");
            file.WriteLine("\t\t\t\t\tJOINT wristLeft");
            file.WriteLine("\t\t\t\t\t{");
            file.WriteLine("\t\t\t\t\t\tOFFSET " + m_frames[0]["wristLeft"][x] + " " + m_frames[0]["wristLeft"][y] + " " + m_frames[0]["wristLeft"][z]);
            file.WriteLine("\t\t\t\t\t\tCHANNELS 3 Zrotation Xrotation Yrotation");
            file.WriteLine("\t\t\t\t\t\tJOINT handLeft");
            file.WriteLine("\t\t\t\t\t\t{");
            file.WriteLine("\t\t\t\t\t\t\tOFFSET " + m_frames[0]["handLeft"][x] + " " + m_frames[0]["handLeft"][y] + " " + m_frames[0]["handLeft"][z]);
            file.WriteLine("\t\t\t\t\t\t\tCHANNELS 3 Zrotation Xrotation Yrotation");
            file.WriteLine("\t\t\t\t\t\t\tJOINT handTipLeft");
            file.WriteLine("\t\t\t\t\t\t\t{");
            file.WriteLine("\t\t\t\t\t\t\t\tOFFSET " + m_frames[0]["handTipLeft"][x] + " " + m_frames[0]["handTipLeft"][y] + " " + m_frames[0]["handTipLeft"][z]);
            file.WriteLine("\t\t\t\t\t\t\t\tCHANNELS 3 Zrotation Xrotation Yrotation");
            file.WriteLine("\t\t\t\t\t\t\t\tEnd Site");
            file.WriteLine("\t\t\t\t\t\t\t\t{");
            file.WriteLine("\t\t\t\t\t\t\t\t\t OFFSET 0 0 0");
            file.WriteLine("\t\t\t\t\t\t\t\t}");
            file.WriteLine("\t\t\t\t\t\t\t}");
            file.WriteLine("\t\t\t\t\t\t}");
            file.WriteLine("\t\t\t\t\t}");
            file.WriteLine("\t\t\t\t}");
            file.WriteLine("\t\t\t}");
            file.WriteLine("\t\t\tJOINT shoulderRight");
            file.WriteLine("\t\t\t{");
            file.WriteLine("\t\t\t\tOFFSET " + m_frames[0]["shoulderRight"][x] + " " + m_frames[0]["shoulderRight"][y] + " " + m_frames[0]["shoulderRight"][z]);
            file.WriteLine("\t\t\t\tCHANNELS 3 Zrotation Xrotation Yrotation");
            file.WriteLine("\t\t\t\tJOINT elbowRight");
            file.WriteLine("\t\t\t\t{");
            file.WriteLine("\t\t\t\t\tOFFSET " + m_frames[0]["elbowRight"][x] + " " + m_frames[0]["elbowRight"][y] + " " + m_frames[0]["elbowRight"][z]);
            file.WriteLine("\t\t\t\t\tCHANNELS 3 Zrotation Xrotation Yrotation");
            file.WriteLine("\t\t\t\t\tJOINT wristRight");
            file.WriteLine("\t\t\t\t\t{");
            file.WriteLine("\t\t\t\t\t\tOFFSET " + m_frames[0]["wristRight"][x] + " " + m_frames[0]["wristRight"][y] + " " + m_frames[0]["wristRight"][z]);
            file.WriteLine("\t\t\t\t\t\tCHANNELS 3 Zrotation Xrotation Yrotation");
            file.WriteLine("\t\t\t\t\t\tJOINT handRight");
            file.WriteLine("\t\t\t\t\t\t{");
            file.WriteLine("\t\t\t\t\t\t\tOFFSET " + m_frames[0]["handRight"][x] + " " + m_frames[0]["handRight"][y] + " " + m_frames[0]["handRight"][z]);
            file.WriteLine("\t\t\t\t\t\t\tCHANNELS 3 Zrotation Xrotation Yrotation");
            file.WriteLine("\t\t\t\t\t\t\tJOINT handTipRight");
            file.WriteLine("\t\t\t\t\t\t\t{");
            file.WriteLine("\t\t\t\t\t\t\t\tOFFSET " + m_frames[0]["handTipRight"][x] + " " + m_frames[0]["handTipRight"][y] + " " + m_frames[0]["handTipRight"][z]);
            file.WriteLine("\t\t\t\t\t\t\t\tCHANNELS 3 Zrotation Xrotation Yrotation");
            file.WriteLine("\t\t\t\t\t\t\t\tEnd Site");
            file.WriteLine("\t\t\t\t\t\t\t\t{");
            file.WriteLine("\t\t\t\t\t\t\t\t\t OFFSET 0 0 0");
            file.WriteLine("\t\t\t\t\t\t\t\t}");
            file.WriteLine("\t\t\t\t\t\t\t}");
            file.WriteLine("\t\t\t\t\t\t}");
            file.WriteLine("\t\t\t\t\t}");
            file.WriteLine("\t\t\t\t}");
            file.WriteLine("\t\t\t}");
            file.WriteLine("\t\t}");
            file.WriteLine("\t}");
            file.WriteLine("\tJOINT hipLeft");
            file.WriteLine("\t{");
            file.WriteLine("\t\tOFFSET " + m_frames[0]["hipLeft"][x] + " " + m_frames[0]["hipLeft"][1] + " " + m_frames[0]["hipLeft"][2]);
            file.WriteLine("\t\tCHANNELS 3 Zrotation Xrotation Yrotation");
            file.WriteLine("\t\tJOINT kneeLeft");
            file.WriteLine("\t\t{");
            file.WriteLine("\t\t\tOFFSET " + m_frames[0]["kneeLeft"][x] + " " + m_frames[0]["kneeLeft"][1] + " " + m_frames[0]["kneeLeft"][2]);
            file.WriteLine("\t\t\tCHANNELS 3 Zrotation Xrotation Yrotation");
            file.WriteLine("\t\t\tJOINT ankleLeft");
            file.WriteLine("\t\t\t{");
            file.WriteLine("\t\t\t\tOFFSET " + m_frames[0]["ankleLeft"][x] + " " + m_frames[0]["ankleLeft"][1] + " " + m_frames[0]["ankleLeft"][2]);
            file.WriteLine("\t\t\t\tCHANNELS 3 Zrotation Xrotation Yrotation");
            file.WriteLine("\t\t\t\tJOINT footLeft");
            file.WriteLine("\t\t\t\t{");
            file.WriteLine("\t\t\t\t\tOFFSET " + m_frames[0]["footLeft"][x] + " " + m_frames[0]["footLeft"][1] + " " + m_frames[0]["footLeft"][2]);
            file.WriteLine("\t\t\t\t\tCHANNELS 3 Zrotation Xrotation Yrotation");
            file.WriteLine("\t\t\t\t\tEnd Site");
            file.WriteLine("\t\t\t\t\t{");
            file.WriteLine("\t\t\t\t\t\tOFFSET 0 0 0");
            file.WriteLine("\t\t\t\t\t}");
            file.WriteLine("\t\t\t\t}");
            file.WriteLine("\t\t\t}");
            file.WriteLine("\t\t}");
            file.WriteLine("\t}");
            file.WriteLine("\tJOINT hipRight");
            file.WriteLine("\t{");
            file.WriteLine("\t\tOFFSET " + m_frames[0]["hipRight"][x] + " " + m_frames[0]["hipRight"][1] + " " + m_frames[0]["hipRight"][2]);
            file.WriteLine("\t\tCHANNELS 3 Zrotation Xrotation Yrotation");
            file.WriteLine("\t\tJOINT kneeRight");
            file.WriteLine("\t\t{");
            file.WriteLine("\t\t\tOFFSET " + m_frames[0]["kneeRight"][x] + " " + m_frames[0]["kneeRight"][1] + " " + m_frames[0]["kneeRight"][2]);
            file.WriteLine("\t\t\tCHANNELS 3 Zrotation Xrotation Yrotation");
            file.WriteLine("\t\t\tJOINT ankleRight");
            file.WriteLine("\t\t\t{");
            file.WriteLine("\t\t\t\tOFFSET " + m_frames[0]["ankleRight"][x] + " " + m_frames[0]["ankleRight"][1] + " " + m_frames[0]["ankleRight"][2]);
            file.WriteLine("\t\t\t\tCHANNELS 3 Zrotation Xrotation Yrotation");
            file.WriteLine("\t\t\t\tJOINT footRight");
            file.WriteLine("\t\t\t\t{");
            file.WriteLine("\t\t\t\t\tOFFSET " + m_frames[0]["footRight"][x] + " " + m_frames[0]["footRight"][1] + " " + m_frames[0]["footRight"][2]);
            file.WriteLine("\t\t\t\t\tCHANNELS 3 Zrotation Xrotation Yrotation");
            file.WriteLine("\t\t\t\t\tEnd Site");
            file.WriteLine("\t\t\t\t\t{");
            file.WriteLine("\t\t\t\t\t\tOFFSET 0 0 0");
            file.WriteLine("\t\t\t\t\t}");
            file.WriteLine("\t\t\t\t}");
            file.WriteLine("\t\t\t}");
            file.WriteLine("\t\t}");
            file.WriteLine("\t}");
            file.WriteLine("}");

            file.WriteLine("MOTION");
            file.WriteLine("Frames: " + m_frames.Count);
            file.WriteLine("Frame Time: 0.0333333");
            
            float scale = 1;
            for (int i = 1; i < m_frames.Count; i++)
            {
                //file.Write(m_frames[i]["spineBase"][0] + " " + m_frames[i]["spineBase"][1] + " " + m_frames[i]["spineBase"][2]+" ");
                //Console.WriteLine("Bones count: " + m_frames[i].Count);
                //for (int j = 0; j < m_frames[i].Count; j++)
                // {
                //file.Write(j + " ");
                foreach (var item in m_frames[i])
                {
                    string key = item.Key;
                    //Console.WriteLine(key);
                    float valsX = m_frames[i][key][0] * scale;
                    float valsZ = m_frames[i][key][2] * scale;
                    float valsY = m_frames[i][key][1] * scale;
                    float valsXR = 1;// m_frames[i][key][3] * scale;
                    float valsYR = 1;// m_frames[i][key][4] * scale;
                    float valsZR = 1;// m_frames[i][key][5] * scale;

                    //Console.WriteLine(vals);
                    file.Write(valsX + " " + valsY + " " + valsZ + " " + valsXR + " " + valsYR + " " + valsZR + " ");
                }
                file.WriteLine();

            }


            file.Close();

            //MessageBox.Show(spineBase.Name);
            //MessageBox.Show(tmp.ContainsKey("ankleLeft");
            // if (m_sensor != null)
            // {
            //     m_sensor.Open();
            // }

            // bReader = m_sensor.BodyFrameSource.OpenReader();
            // if( bReader != null)
            //{
            // bReader.FrameArrived += Reader_FrameArrived;
            //}

        }

        private void MakeTPose()
        {
            //for(int jt = (int)JointType.SpineBase; jt <= (int)JointType.ThumbRight; jt++){


            //}
            Dictionary<string, float[]> tmp;
            tmp = new Dictionary<string, float[]>();


            tmp.Add("spineBase",new float[] {0,0,0});
            tmp.Add("spineMid", new float[] { 0, 1, 0 });
            tmp.Add("spineShoulder", new float[] { 0, 0, 0 });
            tmp.Add("neck", new float[] { 0, 0, 0 });
            tmp.Add("head", new float[] { 0, 0, 0 });

            tmp.Add("shoulderLeft", new float[] { 1, 0, 0 });
            tmp.Add("elbowLeft", new float[] { 1, 0, 0 });
            tmp.Add("wristLeft", new float[] { 1, 0, 0 });
            tmp.Add("handLeft", new float[] { 0, 0, 0 });
            tmp.Add("handTipLeft", new float[] { 0, 0, 0 });
            tmp.Add("thumbLeft", new float[] { 0, 0, 0 });

        }

        private void AddPosition(int count, Joint joint, float x, float y, float z)
        {
            //m_pointPosition[count].Add(joint, new float[] { x, y, z });
        }

        private void AddOrientation(int count, JointOrientation joint, float w, float x, float y, float z)
        {
            m_pointOrientation[count].Add(joint, new float[] { w, x, y, z });
        }

        private void Reader_FrameArrived(object sender, BodyFrameArrivedEventArgs e)
        {
            bool dataReceived = false;
            using (BodyFrame bodyFrame = e.FrameReference.AcquireFrame())
            {
                if (bodyFrame != null)
                {

                    if (bodies == null)
                    {
                        bodies = new Body[bodyFrame.BodyCount];
                    }
                }
                bodyFrame.GetAndRefreshBodyData(bodies);
                dataReceived = true;
            }
            if (dataReceived && ready)
            {
                foreach (Body body in bodies)
                {
                    if (body.IsTracked)
                    {
                        Dictionary<string, float[]> tmp;
                        tmp = new Dictionary<string, float[]>();
                        BVHBone bone;
                        IReadOnlyDictionary<JointType, Joint> joints = body.Joints;
                        IReadOnlyDictionary<JointType, JointOrientation> orientations = body.JointOrientations;

                        Dictionary<JointType, Point> jointPoints = new Dictionary<JointType, Point>();


                        Joint ankleLeft = joints[JointType.AnkleLeft];
                        JointOrientation ankleLeftOri = orientations[JointType.AnkleLeft];
                        Joint ankleRight = joints[JointType.AnkleRight];
                        JointOrientation ankleRightOri = orientations[JointType.AnkleRight];
                        Joint elbowLeft = joints[JointType.ElbowLeft];
                        JointOrientation elbowLeftOri = orientations[JointType.ElbowLeft];
                        Joint elbowRight = joints[JointType.ElbowRight];
                        JointOrientation elbowRightOri = orientations[JointType.ElbowRight];
                        Joint footLeft = joints[JointType.FootLeft];
                        JointOrientation footLeftOri = orientations[JointType.FootLeft];
                        Joint footRight = joints[JointType.FootRight];
                        JointOrientation footRightOri = orientations[JointType.FootRight];
                        Joint handLeft = joints[JointType.HandLeft];
                        JointOrientation handLeftOri = orientations[JointType.HandLeft];
                        Joint handRight = joints[JointType.HandRight];
                        JointOrientation handRightOri = orientations[JointType.HandRight];
                        Joint handTipLeft = joints[JointType.HandTipLeft];
                        JointOrientation handTipLeftOri = orientations[JointType.HandTipLeft];
                        Joint handTipRight = joints[JointType.HandTipRight];
                        JointOrientation handTipRightOri = orientations[JointType.HandTipRight];
                        Joint head = joints[JointType.Head];
                        JointOrientation headOri = orientations[JointType.Head];
                        Joint hipLeft = joints[JointType.HipLeft];
                        JointOrientation hipLeftOri = orientations[JointType.HipLeft];
                        Joint hipRight = joints[JointType.HipRight];
                        JointOrientation hipRightOri = orientations[JointType.HipRight];
                        Joint kneeRight = joints[JointType.KneeLeft];
                        JointOrientation kneeRightOri = orientations[JointType.KneeLeft];
                        Joint kneeLeft = joints[JointType.KneeRight];
                        JointOrientation kneeLeftOri = orientations[JointType.KneeRight];
                        Joint neck = joints[JointType.Neck];
                        JointOrientation neckOri = orientations[JointType.Neck];
                        Joint shoulderLeft = joints[JointType.ShoulderLeft];
                        JointOrientation shoulderLeftOri = orientations[JointType.ShoulderLeft];
                        Joint shoulderRight = joints[JointType.ShoulderRight];
                        JointOrientation shoulderRightOri = orientations[JointType.ShoulderRight];
                        Joint spineBase = joints[JointType.SpineBase];
                        JointOrientation spineBaseOri = orientations[JointType.SpineBase];
                        Joint spineMid = joints[JointType.SpineMid];
                        JointOrientation spineMidOri = orientations[JointType.SpineMid];
                        Joint spineShoulder = joints[JointType.SpineShoulder];
                        JointOrientation spineShoulderOri = orientations[JointType.SpineShoulder];
                        Joint thumbLeft = joints[JointType.ThumbLeft];
                        JointOrientation thumbLeftOri = orientations[JointType.ThumbLeft];
                        Joint thumbRight = joints[JointType.ThumbRight];
                        JointOrientation thumbRightOri = orientations[JointType.ThumbRight];
                        Joint wristLeft = joints[JointType.WristLeft];
                        JointOrientation wristLeftOri = orientations[JointType.WristLeft];
                        Joint wristRight = joints[JointType.WristRight];
                        JointOrientation wristRightOri = orientations[JointType.WristRight];
                        //spineBase.translOffset[0] = 
                        //m_pointPosition.Add(new string[] { "spineBase", spineBase.Position.X.ToString(),  spineBase.Position.Y * 10.ToString(), spineBase.Position.Z * 10.ToString() });
                        //tmp.Add();
                        float oMulti = 180.0f / 3.14f;
                        int mult = 10;
                        tmp.Add("spineBase", new float[] { spineBase.Position.X * mult, spineBase.Position.Y * mult, spineBase.Position.Z * mult, spineBaseOri.Orientation.X * oMulti, spineBaseOri.Orientation.Y * oMulti, spineBaseOri.Orientation.Z * oMulti, spineBaseOri.Orientation.W * oMulti });
                        tmp.Add("spineMid", new float[] { spineMid.Position.X * mult, spineMid.Position.Y * mult, spineMid.Position.Z * mult, spineMidOri.Orientation.X * oMulti, spineMidOri.Orientation.Y * oMulti, spineMidOri.Orientation.Z * oMulti, spineMidOri.Orientation.W * oMulti });
                        tmp.Add("spineShoulder", new float[] { spineShoulder.Position.X * mult, spineShoulder.Position.Y * mult, spineShoulder.Position.Z * mult, spineShoulderOri.Orientation.X * oMulti, spineShoulderOri.Orientation.Y * oMulti, spineShoulderOri.Orientation.Z * oMulti, spineShoulderOri.Orientation.W * oMulti });
                        tmp.Add("neck", new float[] { neck.Position.X * mult, neck.Position.Y * mult, neck.Position.Z * mult, neckOri.Orientation.X * oMulti, neckOri.Orientation.Y * oMulti, neckOri.Orientation.Z * oMulti, neckOri.Orientation.W * oMulti });
                        tmp.Add("head", new float[] { head.Position.X * mult, head.Position.Y * mult, head.Position.Z * mult, headOri.Orientation.X * oMulti, headOri.Orientation.Y * oMulti, headOri.Orientation.Z * oMulti, headOri.Orientation.W * oMulti });

                        tmp.Add("shoulderLeft", new float[] { shoulderLeft.Position.X * mult, shoulderLeft.Position.Y * mult, shoulderLeft.Position.Z * mult, shoulderLeftOri.Orientation.X * oMulti, shoulderLeftOri.Orientation.Y * oMulti, shoulderLeftOri.Orientation.Z * oMulti, shoulderLeftOri.Orientation.W * oMulti });
                        tmp.Add("elbowLeft", new float[] { elbowLeft.Position.X * mult, elbowLeft.Position.Y * mult, elbowLeft.Position.Z * mult, elbowLeftOri.Orientation.X * oMulti, elbowLeftOri.Orientation.Y * oMulti, elbowLeftOri.Orientation.Z * oMulti, elbowLeftOri.Orientation.W * oMulti });
                        tmp.Add("wristLeft", new float[] { wristLeft.Position.X * mult, wristLeft.Position.Y * mult, wristLeft.Position.Z * mult, wristLeftOri.Orientation.X * oMulti, wristLeftOri.Orientation.Y * oMulti, wristLeftOri.Orientation.Z * oMulti, wristLeftOri.Orientation.W * oMulti });
                        tmp.Add("handLeft", new float[] { handLeft.Position.X * mult, handLeft.Position.Y * mult, handLeft.Position.Z * mult, handLeftOri.Orientation.X * oMulti, handLeftOri.Orientation.Y * oMulti, handLeftOri.Orientation.Z * oMulti, handLeftOri.Orientation.W * oMulti });
                        tmp.Add("handTipLeft", new float[] { handTipLeft.Position.X * mult, handTipLeft.Position.Y * mult, handTipLeft.Position.Z * mult, handTipLeftOri.Orientation.X * oMulti, handTipLeftOri.Orientation.Y * oMulti, handTipLeftOri.Orientation.Z * oMulti, handTipLeftOri.Orientation.W * oMulti });
                        tmp.Add("thumbLeft", new float[] { thumbLeft.Position.X * mult, thumbLeft.Position.Y * mult, thumbLeft.Position.Z * mult, thumbLeftOri.Orientation.X * oMulti, thumbLeftOri.Orientation.Y * oMulti, thumbLeftOri.Orientation.Z * oMulti, thumbLeftOri.Orientation.W * oMulti });


                        tmp.Add("shoulderRight", new float[] { shoulderRight.Position.X * mult, shoulderRight.Position.Y * mult, shoulderRight.Position.Z * mult, shoulderRightOri.Orientation.X * oMulti, shoulderRightOri.Orientation.Y * oMulti, shoulderRightOri.Orientation.Z * oMulti, shoulderRightOri.Orientation.W * oMulti });
                        tmp.Add("elbowRight", new float[] { elbowRight.Position.X * mult, elbowRight.Position.Y * mult, elbowRight.Position.Z * mult, elbowRightOri.Orientation.X * oMulti, elbowRightOri.Orientation.Y * oMulti, elbowRightOri.Orientation.Z * oMulti, elbowRightOri.Orientation.W * oMulti });
                        tmp.Add("wristRight", new float[] { wristRight.Position.X * mult, wristRight.Position.Y * mult, wristRight.Position.Z * mult, wristRightOri.Orientation.X * oMulti, wristRightOri.Orientation.Y * oMulti, wristRightOri.Orientation.Z * oMulti, wristRightOri.Orientation.W * oMulti });
                        tmp.Add("handRight", new float[] { handRight.Position.X * mult, handRight.Position.Y * mult, handRight.Position.Z * mult, handRightOri.Orientation.X * oMulti, handRightOri.Orientation.Y * oMulti, handRightOri.Orientation.Z * oMulti, handRightOri.Orientation.W * oMulti });
                        tmp.Add("handTipRight", new float[] { handTipRight.Position.X * mult, handTipRight.Position.Y * mult, handTipRight.Position.Z * mult, handTipRightOri.Orientation.X * oMulti, handTipRightOri.Orientation.Y * oMulti, handTipRightOri.Orientation.Z * oMulti, handTipRightOri.Orientation.W * oMulti });
                        tmp.Add("thumbRight", new float[] { thumbRight.Position.X * mult, thumbRight.Position.Y * mult, thumbRight.Position.Z * mult, thumbRightOri.Orientation.X * oMulti, thumbRightOri.Orientation.Y * oMulti, thumbRightOri.Orientation.Z * oMulti, thumbRightOri.Orientation.W * oMulti });

                        tmp.Add("hipLeft", new float[] { hipLeft.Position.X * mult, hipLeft.Position.Y * mult, hipLeft.Position.Z * mult, hipLeftOri.Orientation.X * oMulti, hipLeftOri.Orientation.Y * oMulti, hipLeftOri.Orientation.Z * oMulti, hipLeftOri.Orientation.W * oMulti });
                        tmp.Add("kneeLeft", new float[] { kneeLeft.Position.X * mult, kneeLeft.Position.Y * mult, kneeLeft.Position.Z * mult, kneeLeftOri.Orientation.X * oMulti, kneeLeftOri.Orientation.Y * oMulti, kneeLeftOri.Orientation.Z * oMulti, kneeLeftOri.Orientation.W * oMulti });
                        tmp.Add("ankleLeft", new float[] { ankleLeft.Position.X * mult, ankleLeft.Position.Y * mult, ankleLeft.Position.Z * mult, ankleLeftOri.Orientation.X * oMulti, ankleLeftOri.Orientation.Y * oMulti, ankleLeftOri.Orientation.Z * oMulti, ankleLeftOri.Orientation.W * oMulti });
                        tmp.Add("footLeft", new float[] { footLeft.Position.X * mult, footLeft.Position.Y * mult, footLeft.Position.Z * mult, footLeftOri.Orientation.X * oMulti, footLeftOri.Orientation.Y * oMulti, footLeftOri.Orientation.Z * oMulti, footLeftOri.Orientation.W * oMulti });

                        tmp.Add("hipRight", new float[] { hipRight.Position.X * mult, hipRight.Position.Y * mult, hipRight.Position.Z * mult, hipRightOri.Orientation.X * oMulti, hipRightOri.Orientation.Y * oMulti, hipRightOri.Orientation.Z * oMulti, hipRightOri.Orientation.W * oMulti });
                        tmp.Add("kneeRight", new float[] { kneeRight.Position.X * mult, kneeRight.Position.Y * mult, kneeRight.Position.Z * mult, kneeRightOri.Orientation.X * oMulti, kneeRightOri.Orientation.Y * oMulti, kneeRightOri.Orientation.Z * oMulti, kneeRightOri.Orientation.W * oMulti });
                        tmp.Add("ankleRight", new float[] { ankleRight.Position.X * mult, ankleRight.Position.Y * mult, ankleRight.Position.Z * mult, ankleRightOri.Orientation.X * oMulti, ankleRightOri.Orientation.Y * oMulti, ankleRightOri.Orientation.Z * oMulti, ankleRightOri.Orientation.W * oMulti });
                        tmp.Add("footRight", new float[] { footRight.Position.X * mult, footRight.Position.Y * mult, footRight.Position.Z * mult, footRightOri.Orientation.X * oMulti, footRightOri.Orientation.Y * oMulti, footRightOri.Orientation.Z * oMulti, footRightOri.Orientation.W * oMulti });

                        //tmp.Add("thumbLeft", new float[] { thumbLeft.Position.X * mult, thumbLeft.Position.Y * mult, thumbLeft.Position.Z * mult, thumbLeftOri.Orientation.X  * oMulti, thumbLeftOri.Orientation.Y  * oMulti, thumbLeftOri.Orientation.Z * oMulti });
                        //tmp.Add("thumbRight", new float[] { thumbRight.Position.X * mult, thumbRight.Position.Y * mult, thumbRight.Position.Z * mult, thumbRightOri.Orientation.X  * oMulti, thumbRightOri.Orientation.Y  * oMulti, thumbRightOri.Orientation.Z * oMulti });

                        //tmp.Add("wristLeft", new float[] { wristLeft.Position.X, wristLeft.Position.Y * 10, wristLeft.Position.Z * 10, wristLeftOri.Orientation.X  * oMulti  * oMulti * oMulti* 10, wristLeftOri.Orientation.Y * 10, wristLeftOri.Orientation.Z * 10 });
                        //tmp.Add("wristRight", new float[] { wristRight.Position.X, wristRight.Position.Y * 10, wristRight.Position.Z * 10, wristRightOri.Orientation.X  * oMulti  * oMulti* 10, wristRightOri.Orientation.Y * 10, wristRightOri.Orientation.Z * 10 });


                        // m_pointPosition.Add(tmp);
                        // m_frames.Add(m_pointPosition);
                        // tmp.Clear();

                        //m_pointPosition.Add(tmp);

                        // m_pointPosition.Clear();
                        //m_pointOrientation.Add( (ankleLeftOri,  { ankleLeftOri.Orientation.W, ankleLeftOri.Orientation.X  * oMulti* 10, ankleLeftOri.Orientation.Y * 10, ankleLeftOri.Orientation.Z * 10 }));
                        //count++;
                        //
                        ////m_pointPosition.Add(new Dictionary<Joint, float[]> (ankleRight, new float[] { ankleRight.Position.X, ankleRight.Position.Y * 10, ankleRight.Position.Z * 10 }));
                        //m_pointOrientation[count].Add(ankleRightOri, new float[] { ankleRightOri.Orientation.W, ankleRightOri.Orientation.X * 10, ankleRightOri.Orientation.Y * 10, ankleRightOri.Orientation.Z * 10 });
                        //count++;
                        //
                        //m_pointPosition[count].Add(elbowLeft, new float[] { elbowLeft.Position.X, elbowLeft.Position.Y * 10, elbowLeft.Position.Z * 10 });
                        //m_pointOrientation[count].Add(elbowLeftOri, new float[] { elbowLeftOri.Orientation.W, elbowLeftOri.Orientation.X * 10, elbowLeftOri.Orientation.Y * 10, elbowLeftOri.Orientation.Z * 10 });
                        //count++;
                        //
                        //m_pointPosition[count].Add(elbowRight, new float[] { elbowRight.Position.X, elbowRight.Position.Y * 10, elbowRight.Position.Z * 10 });
                        //m_pointOrientation[count].Add(elbowRightOri, new float[] { elbowRightOri.Orientation.W, elbowRightOri.Orientation.X * 10, elbowRightOri.Orientation.Y * 10, elbowRightOri.Orientation.Z * 10 });
                        //count++;
                        //
                        //m_pointPosition[count].Add(footLeft, new float[] { footLeft.Position.X, footLeft.Position.Y * 10, footLeft.Position.Z * 10 });
                        //m_pointOrientation[count].Add(footLeftOri, new float[] { footLeftOri.Orientation.W, footLeftOri.Orientation.X * 10, footLeftOri.Orientation.Y * 10, footLeftOri.Orientation.Z * 10 });
                        //count++;
                        //
                        //m_pointPosition[count].Add(footRight, new float[] { footRight.Position.X, footRight.Position.Y * 10, footRight.Position.Z * 10 });
                        //m_pointOrientation[count].Add(footRightOri, new float[] { footRightOri.Orientation.W, footRightOri.Orientation.X * 10, footRightOri.Orientation.Y * 10, footRightOri.Orientation.Z * 10 });
                        //count++;
                        //
                        //m_pointPosition[count].Add(handLeft, new float[] { handLeft.Position.X, handLeft.Position.Y * 10, handLeft.Position.Z * 10 });
                        //m_pointOrientation[count].Add(handLeftOri, new float[] { handLeftOri.Orientation.W, handLeftOri.Orientation.X * 10, handLeftOri.Orientation.Y * 10, handLeftOri.Orientation.Z * 10 });
                        //count++;
                        //
                        //m_pointPosition[count].Add(handRight, new float[] { handRight.Position.X, handRight.Position.Y * 10, handRight.Position.Z * 10 });
                        //m_pointOrientation[count].Add(handRightOri, new float[] { handRightOri.Orientation.W, handRightOri.Orientation.X * 10, handRightOri.Orientation.Y * 10, handRightOri.Orientation.Z * 10 });
                        //count++;
                        //
                        //m_pointPosition[count].Add(handTipLeft, new float[] { handTipLeft.Position.X, handTipLeft.Position.Y * 10, handTipLeft.Position.Z * 10 });
                        //m_pointOrientation[count].Add(handTipLeftOri, new float[] { handTipLeftOri.Orientation.W, handTipLeftOri.Orientation.X * 10, handTipLeftOri.Orientation.Y * 10, handTipLeftOri.Orientation.Z * 10 });
                        //count++;
                        //
                        //m_pointPosition[count].Add(handTipRight, new float[] { handTipRight.Position.X, handTipRight.Position.Y * 10, handTipRight.Position.Z * 10 });
                        //m_pointOrientation[count].Add(handTipRightOri, new float[] { handTipRightOri.Orientation.W, handTipRightOri.Orientation.X * 10, handTipRightOri.Orientation.Y * 10, handTipRightOri.Orientation.Z * 10 });
                        //count++;
                        //
                        //m_pointPosition[count].Add(head, new float[] { head.Position.X, head.Position.Y * 10, head.Position.Z * 10 });
                        //m_pointOrientation[count].Add(headOri, new float[] { headOri.Orientation.W, headOri.Orientation.X * 10, headOri.Orientation.Y * 10, headOri.Orientation.Z * 10 });
                        //count++;
                        //
                        //m_pointPosition[count].Add(hipLeft, new float[] { hipLeft.Position.X, hipLeft.Position.Y * 10, hipLeft.Position.Z * 10 });
                        //m_pointOrientation[count].Add(hipLeftOri, new float[] { hipLeftOri.Orientation.W, hipLeftOri.Orientation.X * 10, hipLeftOri.Orientation.Y * 10, hipLeftOri.Orientation.Z * 10 });
                        //count++;
                        //
                        //m_pointPosition[count].Add(hipRight, new float[] { hipRight.Position.X, hipRight.Position.Y * 10, hipRight.Position.Z * 10 });
                        //m_pointOrientation[count].Add(hipRightOri, new float[] { hipRightOri.Orientation.W, hipRightOri.Orientation.X * 10, hipRightOri.Orientation.Y * 10, hipRightOri.Orientation.Z * 10 });
                        //count++;
                        //
                        //m_pointPosition[count].Add(kneeRight, new float[] { kneeRight.Position.X, kneeRight.Position.Y * 10, kneeRight.Position.Z * 10 });
                        //m_pointOrientation[count].Add(kneeRightOri, new float[] { kneeRightOri.Orientation.W, kneeRightOri.Orientation.X * 10, kneeRightOri.Orientation.Y * 10, kneeRightOri.Orientation.Z * 10 });
                        //count++;
                        //
                        //m_pointPosition[count].Add(kneeLeft, new float[] { kneeLeft.Position.X, kneeLeft.Position.Y * 10, kneeLeft.Position.Z * 10 });
                        //m_pointOrientation[count].Add(kneeLeftOri, new float[] { kneeLeftOri.Orientation.W, kneeLeftOri.Orientation.X * 10, kneeLeftOri.Orientation.Y * 10, kneeLeftOri.Orientation.Z * 10 });
                        //count++;
                        //
                        //m_pointPosition[count].Add(neck, new float[] { neck.Position.X, neck.Position.Y * 10, neck.Position.Z * 10 });
                        //m_pointOrientation[count].Add(neckOri, new float[] { neckOri.Orientation.W, neckOri.Orientation.X * 10, neckOri.Orientation.Y * 10, neckOri.Orientation.Z * 10 });
                        //count++;
                        //
                        //m_pointPosition[count].Add(spineBase, new float[] { spineBase.Position.X, spineBase.Position.Y * 10, spineBase.Position.Z * 10 });
                        //m_pointOrientation[count].Add(spineBaseOri, new float[] { spineBaseOri.Orientation.W, spineBaseOri.Orientation.X * 10, spineBaseOri.Orientation.Y * 10, spineBaseOri.Orientation.Z * 10 });
                        //count++;
                        //
                        //m_pointPosition[count].Add(spineMid, new float[] { spineMid.Position.X, spineMid.Position.Y * 10, spineMid.Position.Z * 10 });
                        //m_pointOrientation[count].Add(spineMidOri, new float[] { spineMidOri.Orientation.W, spineMidOri.Orientation.X * 10, spineMidOri.Orientation.Y * 10, spineMidOri.Orientation.Z * 10 });
                        //count++;
                        //
                        //m_pointPosition[count].Add(spineShoulder, new float[] { spineShoulder.Position.X, spineShoulder.Position.Y * 10, spineShoulder.Position.Z * 10 });
                        //m_pointOrientation[count].Add(spineShoulderOri, new float[] { spineShoulderOri.Orientation.W, spineShoulderOri.Orientation.X * 10, spineShoulderOri.Orientation.Y * 10, spineShoulderOri.Orientation.Z * 10 });
                        //count++;
                        //
                        //m_pointPosition[count].Add(thumbLeft, new float[] { thumbLeft.Position.X, thumbLeft.Position.Y * 10, thumbLeft.Position.Z * 10 });
                        //m_pointOrientation[count].Add(thumbLeftOri, new float[] { thumbLeftOri.Orientation.W, thumbLeftOri.Orientation.X * 10, thumbLeftOri.Orientation.Y * 10, thumbLeftOri.Orientation.Z * 10 });
                        //count++;
                        //
                        //m_pointPosition[count].Add(thumbRight, new float[] { thumbRight.Position.X, thumbRight.Position.Y * 10, thumbRight.Position.Z * 10 });
                        //m_pointOrientation[count].Add(thumbRightOri, new float[] { thumbRightOri.Orientation.W, thumbRightOri.Orientation.X * 10, thumbRightOri.Orientation.Y * 10, thumbRightOri.Orientation.Z * 10 });
                        //count++;
                        //
                        //m_pointPosition[count].Add(wristLeft, new float[] { wristLeft.Position.X, wristLeft.Position.Y * 10, wristLeft.Position.Z * 10 });
                        //m_pointOrientation[count].Add(wristLeftOri, new float[] { wristLeftOri.Orientation.W, wristLeftOri.Orientation.X * 10, wristLeftOri.Orientation.Y * 10, wristLeftOri.Orientation.Z * 10 });
                        //count++;
                        //
                        //m_pointPosition[count].Add(wristRight, new float[] { wristRight.Position.X, wristRight.Position.Y * 10, wristRight.Position.Z * 10 });
                        //m_pointOrientation[count].Add(wristRightOri, new float[] { wristRightOri.Orientation.W, wristRightOri.Orientation.X * 10, wristRightOri.Orientation.Y * 10, wristRightOri.Orientation.Z * 10 });
                        //count++;

                        //foreach (var item in tmp)
                        //{                 
                        //    string key = item.Key;
                        //    float vals = tmp[key][0];
                        //    Console.Write(key + ": " + vals + " ");
                        //   // file.Write(vals + " ");
                        //}

                        //Console.WriteLine();


                        float ms_distance_x = tmp["head"][3];
                        float ms_distance_y = tmp["head"][4];
                        float ms_distance_z = tmp["head"][5];

                        textBox1.Text = ms_distance_x.ToString();
                        textBox2.Text = ms_distance_y.ToString();
                        textBox3.Text = ms_distance_z.ToString();

                        m_frames.Add(tmp);
                        //tmp.Clear();
                    }
                }
                //m_frameCount++;
            }
        }
        private void getRotations(int i)
        {
            foreach (var keys in m_frames[i])
            {
                string key = keys.Key;
                Vector3D a1 = new Vector3D((double)m_frames[i][key][0], (double)m_frames[i][key][1], (double)m_frames[i][key][2]);
                Vector3D a2 = new Vector3D((double)m_frames[i][key + 1][0], (double)m_frames[i][key + 1][1], (double)m_frames[i][key + 1][2]);
            }
        }

        private void toEulerAngles(int i)
        {
            foreach (var keys in m_frames[i])
            {
                string key = keys.Key;
                //float y = 0;
                ////int a = 0;
                //float sinr = 2.0f * (m_frames[i][key][6] * m_frames[i][key][3] + m_frames[i][key][4] * m_frames[i][key][5]);
                //float cosr = 1.0f- 2.0f * (m_frames[i][key][3] * m_frames[i][key][3] + m_frames[i][key][4] * m_frames[i][key][4]);
                //float x = (float)Math.Atan2(sinr, cosr);
                //
                ////x = x * (180f / (float)Math.PI);
                //
                //// pitch (y-axis rotation)
                //float sinp = 2.0f * (m_frames[i][key][6] * m_frames[i][key][4] - m_frames[i][key][5] * m_frames[i][key][3]);
                //y = (float)Math.Asin(sinp);
                ////if (Math.Abs(sinp) >= 1)
                ////    //Console.WriteLine("Needed This right now");
                ////    y = Math.Sign(sinp);
                //////float y =  copysign(Math.PI / 2, sinp); // use 90 degrees if out of range
                ////else
                // 
                /// y = y * (180f / (float)Math.PI);
                ////pitch = Math.Asin(sinp);
                //
                //// yaw (z-axis rotation)
                //float siny = 2.0f * (m_frames[i][key][6] * m_frames[i][key][5] + m_frames[i][key][3] * m_frames[i][key][4]);
                //float  cosy = 1.0f - 2.0f * (m_frames[i][key][4] * m_frames[i][key][4] + m_frames[i][key][5] * m_frames[i][key][5]);
                //float z = (float)Math.Atan2(siny, cosy);
                ////z = z * (180f / (float)Math.PI);
                float[] q = m_frames[i][key];
                for (int k = 0; k < q.Length; k++)
                {
                    // Console.Write(q[k]+" ");
                }
                // Console.WriteLine();
                float w = q[6];
                float x = q[3];
                float y = q[4];
                float z = q[5];

                double sqw = w * w;
                double sqx = x * x;
                double sqy = y * y;
                double sqz = z * z;

                double unit = sqx + sqy + sqz + sqw;
                double test = x + y + z + w;

                float angle = (float)Math.Acos(w);
                float sa = (float)Math.Sin(angle);
                float ooScale = 0f;
                if (sa != 0)
                    ooScale = 1.0f / sa;

                angle *= 2.0f;

                m_frames[i][key][4] = y * ooScale;
                m_frames[i][key][3] = x * ooScale;
                m_frames[i][key][5] = z * ooScale;

                Vector3D vec = new Vector3D();
                vec.X = m_frames[i][key][3];
                vec.Y = m_frames[i][key][4];
                vec.Z = m_frames[i][key][5];

                Matrix3D rot = GetRotationMatrix(vec.X, vec.Y, vec.Z);
                Vector3D vec2 = Vector3D.Multiply(vec, rot);
                double[] tmp = new double[3];

                tmp[0] = vec.X;
                tmp[1] = vec.Y;
                tmp[2] = vec.Z;
                tmp = rotMatrix2Deg(rot);

                m_frames[i][key][3] = (float)tmp[0];
                m_frames[i][key][4] = (float)tmp[1];
                m_frames[i][key][5] = (float)tmp[2];
                //if (bone.Axis == TransAxis.nY)
                //{
                //    m_frames[i][key][3] = -m_frames[i][key][3];
                //    m_frames[i][key][4] = -m_frames[i][key][4];
                //    m_frames[i][key][5] = m_frames[i][key][5];
                //
                //}
                //
                ////Rechter Arm
                //if (bone.Axis == TransAxis.nX && bone.Name != "ShoulderRight")
                //{
                //    double[] tempDecVec = new double[3] { degVec[0], degVec[1], degVec[2] };
                //    degVec[0] = -tempDecVec[2];
                //    degVec[1] = -tempDecVec[1];
                //    degVec[2] = -tempDecVec[0];
                //
                //}

                //Console.WriteLine(key);
                //float fl = (x * z - w * y);
                //m_frames[i][key][4] = (float)Math.Atan2(2f * x * w + 2f * y * z, 1 - 2f * (sqz + sqw));
                //m_frames[i][key][3] = (float)Math.Asin(2f * (x * z - w * y));
                //m_frames[i][key][5] = (float)Math.Atan2(2f * x * y + 2f * z * w, 1 - 2f * (sqy + sqw));
            }
        }

        public static Matrix3D GetRotationMatrixZ(double angle)
        {
            if (angle == 0.0)
            {
                return Matrix3D.Identity;
            }
            double sin = (double)Math.Sin(angle);
            double cos = (double)Math.Cos(angle);
            return new Matrix3D(
         cos, -sin, 0, 0,
         sin, cos, 0, 0,
         0, 0, 1, 0,
         0, 0, 0, 1);
        }

        public static Matrix3D GetRotationMatrixX(double angle)
        {
            if (angle == 0.0)
            {
                return Matrix3D.Identity;
            }
            double sin = (double)Math.Sin(angle);
            double cos = (double)Math.Cos(angle);
            return new Matrix3D(
         1, 0, 0, 0,
         0, cos, -sin, 0,
         0, sin, cos, 0,
         0, 0, 0, 1);

        }


        public static Matrix3D GetRotationMatrixY(double angle)
        {
            if (angle == 0.0)
            {
                return Matrix3D.Identity;
            }
            double sin = (double)Math.Sin(angle);
            double cos = (double)Math.Cos(angle);
            return new Matrix3D(
        cos, 0, sin, 0,
        0, 1, 0, 0,
        -sin, 0, cos, 0,
        0, 0, 0, 1);
        }

        public static double[] rotMatrix2Deg(Matrix3D mat)
        {
            double[] value = new double[3];
            //Quelle: http://social.msdn.microsoft.com/Forums/en-US/b644698d-bdec-47a2-867e-574cf84e5db7/what-is-the-default-sequence-of-hierarchical-rotation-matrix-eg-xyz-#b3946d0d-9658-4c2b-b14b-69e79070c7d2
            // https://en.wikipedia.org/wiki/Euler_angles#Rotation_matrix
            // Kinect Matrix hat die Tait-Bryan Convention mit Y1 X2 Z3 
            // Problem: Es gibt immer 2 Möglichkeiten für Drehung im 3D Raum, weshalb die Gradwerte nicht immer den Quaternionenwerte entsprechen müssen!
            // Drehung um y- Achse 

            value[0] = Math.Asin(-mat.M23);
            // Drehung um x- Achse
            value[1] = Math.Atan2(mat.M13 / Math.Cos(value[0]), mat.M33 / Math.Cos(value[0]));
            // Drehung um z- Achse
            value[2] = Math.Atan2(mat.M21 / Math.Cos(value[0]), mat.M22 / Math.Cos(value[0]));


            // Um auf die gleichen Winkel wie bei den Quaternionen zu kommen muss man die Winkel negieren
            value[0] = value[0] * -(180 / Math.PI);
            value[1] = value[1] * -(180 / Math.PI);
            value[2] = value[2] * -(180 / Math.PI);
            return value;
        }

        public static Matrix3D GetRotationMatrix(double ax, double ay, double az)
        {
            Matrix3D my = Matrix3D.Identity;
            Matrix3D mz = Matrix3D.Identity;
            Matrix3D result = Matrix3D.Identity;
            if (ax != 0.0)
            {
                result = GetRotationMatrixX(ax);
            }
            if (ay != 0.0)
            {
                my = GetRotationMatrixY(ay);
            }
            if (az != 0.0)
            {
                mz = GetRotationMatrixZ(az);
            }
            if (my != null)
            {
                if (result != null)
                {
                    result *= my;
                }
                else
                {
                    result = my;
                }
            }
            if (mz != null)
            {
                if (result != null)
                {
                    result *= mz;
                }
                else
                {
                    result = mz;
                }
            }
            if (result != null)
            {
                return result;
            }
            else
            {
                return Matrix3D.Identity;
            }
        }

        private void convertAboToRel()
        {

        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }

        private void button2_Click(object sender, EventArgs e)
        {
            ready = true;
        }

        private void label1_Click(object sender, EventArgs e)
        {

        }
    }
}
