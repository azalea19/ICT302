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
using Microsoft.Kinect;
using Microsoft.Kinect.Tools;
using System.Threading;

namespace XEFConverter
{
    public partial class Form1 : Form
    {
        private Thread t_LogPointData; //Not quite sure why we need this, maybe because is pretty cool
        private Thread t_LogFrameData; //Not quite sure why we need this, maybe because is pretty cool
        private Thread t_LogDepthData; //Not quite sure why we need this, maybe because is pretty cool

        private List<Dictionary<string, float[]>> m_pointPosition;
        private List<Dictionary<JointOrientation, float[]>> m_pointOrientation;
        private new List< Dictionary<string, float[]>> m_frames;
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
            m_frames = new List< Dictionary < string, float[]>> ();
            
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
        private void button1_Click(object sender, EventArgs e)
        {

            m_sensor.Close();
            StreamWriter file;
            string fileName = "test.bvh";
            if (File.Exists(fileName))
                File.Delete(fileName);
            file = File.CreateText(fileName);
            /*
            file.WriteLine("HIERARCHY");
            file.WriteLine("ROOT spineBase");
            file.WriteLine("{");
            file.WriteLine("\tOFFSET 0 0 0");
            file.WriteLine("\tCHANNELS 6 Xposition Zposition Yposition Xrotation Zrotation Yrotation");
            file.WriteLine("\tJOINT spineMid");
            file.WriteLine("\t{");
            file.WriteLine("\t\tOFFSET " + m_frames[0]["spineMid"][0] + " " + m_frames[0]["spineMid"][1] + " " + m_frames[0]["spineMid"][2]);
            file.WriteLine("\t\tCHANNELS 3 Xposition Zposition Yposition");
            file.WriteLine("\t\tJOINT spineShoulder");
            file.WriteLine("\t\t{");
            file.WriteLine("\t\t\tOFFSET " + m_frames[0]["spineShoulder"][0] + " " + m_frames[0]["spineShoulder"][1] + " " + m_frames[0]["spineShoulder"][2]);
            file.WriteLine("\t\t\tCHANNELS 3 Xposition Zposition Yposition");
            file.WriteLine("\t\t\tJOINT neck");
            file.WriteLine("\t\t\t{");
            file.WriteLine("\t\t\t\tOFFSET " + m_frames[0]["neck"][0] + " " + m_frames[0]["neck"][1] + " " + m_frames[0]["neck"][2]);
            file.WriteLine("\t\t\t\tCHANNELS 3 Xposition Zposition Yposition");
            file.WriteLine("\t\t\t\tJOINT head");
            file.WriteLine("\t\t\t\t{");
            file.WriteLine("\t\t\t\t\tOFFSET " + m_frames[0]["head"][0] + " " + m_frames[0]["head"][1] + " " + m_frames[0]["head"][2]);
            file.WriteLine("\t\t\t\t\tCHANNELS 3 Xposition Zposition Yposition");
            file.WriteLine("\t\t\t\t\tEnd Site");
            file.WriteLine("\t\t\t\t\t{");
            file.WriteLine("\t\t\t\t\t\tOFFSET 0 0 0"); /// not sure what this is
            file.WriteLine("\t\t\t\t\t}");
            file.WriteLine("\t\t\t\t}");
            file.WriteLine("\t\t\t}");
            file.WriteLine("\t\t\tJOINT shoulderLeft");
            file.WriteLine("\t\t\t{");
            file.WriteLine("\t\t\t\tOFFSET " + m_frames[0]["shoulderLeft"][0] + " " + m_frames[0]["shoulderLeft"][1] + " " + m_frames[0]["shoulderLeft"][2]);
            file.WriteLine("\t\t\t\tCHANNELS 3 Xposition Zposition Yposition");
            file.WriteLine("\t\t\t\tJOINT elbowLeft");
            file.WriteLine("\t\t\t\t{");
            file.WriteLine("\t\t\t\t\tOFFSET " + m_frames[0]["elbowLeft"][0] + " " + m_frames[0]["elbowLeft"][1] + " " + m_frames[0]["elbowLeft"][2]);
            file.WriteLine("\t\t\t\t\tCHANNELS 3 Xposition Zposition Yposition");
            file.WriteLine("\t\t\t\t\tJOINT wristLeft");
            file.WriteLine("\t\t\t\t\t{");
            file.WriteLine("\t\t\t\t\t\tOFFSET " + m_frames[0]["wristLeft"][0] + " " + m_frames[0]["wristLeft"][1] + " " + m_frames[0]["wristLeft"][2]);
            file.WriteLine("\t\t\t\t\t\tCHANNELS 3 Xposition Zposition Yposition");
            file.WriteLine("\t\t\t\t\t\tJOINT handLeft");
            file.WriteLine("\t\t\t\t\t\t{");
            file.WriteLine("\t\t\t\t\t\t\tOFFSET " + m_frames[0]["handLeft"][0] + " " + m_frames[0]["handLeft"][1] + " " + m_frames[0]["handLeft"][2]);
            file.WriteLine("\t\t\t\t\t\t\tCHANNELS 3 Xposition Zposition Yposition");
            file.WriteLine("\t\t\t\t\t\t\tJOINT handTipLeft");
            file.WriteLine("\t\t\t\t\t\t\t{");
            file.WriteLine("\t\t\t\t\t\t\t\tOFFSET " + m_frames[0]["handTipLeft"][0] + " " + m_frames[0]["handTipLeft"][1] + " " + m_frames[0]["handTipLeft"][2]);
            file.WriteLine("\t\t\t\t\t\t\t\tCHANNELS 3 Xposition Zposition Yposition");
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
            file.WriteLine("\t\t\t\tOFFSET " + m_frames[0]["shoulderRight"][0] + " " + m_frames[0]["shoulderRight"][1] + " " + m_frames[0]["shoulderRight"][2]);
            file.WriteLine("\t\t\t\tCHANNELS 3 Xposition Zposition Yposition");
            file.WriteLine("\t\t\t\tJOINT elbowRight");
            file.WriteLine("\t\t\t\t{");
            file.WriteLine("\t\t\t\t\tOFFSET " + m_frames[0]["elbowRight"][0] + " " + m_frames[0]["elbowRight"][1] + " " + m_frames[0]["elbowRight"][2]);
            file.WriteLine("\t\t\t\t\tCHANNELS 3 Xposition Zposition Yposition");
            file.WriteLine("\t\t\t\t\tJOINT wristRight");
            file.WriteLine("\t\t\t\t\t{");
            file.WriteLine("\t\t\t\t\t\tOFFSET " + m_frames[0]["wristRight"][0] + " " + m_frames[0]["wristRight"][1] + " " + m_frames[0]["wristRight"][2]);
            file.WriteLine("\t\t\t\t\t\tCHANNELS 3 Xposition Zposition Yposition");
            file.WriteLine("\t\t\t\t\t\tJOINT handRight");
            file.WriteLine("\t\t\t\t\t\t{");
            file.WriteLine("\t\t\t\t\t\t\tOFFSET " + m_frames[0]["handRight"][0] + " " + m_frames[0]["handRight"][1] + " " + m_frames[0]["handRight"][2]);
            file.WriteLine("\t\t\t\t\t\t\tCHANNELS 3 Xposition Zposition Yposition");
            file.WriteLine("\t\t\t\t\t\t\tJOINT handTipRight");
            file.WriteLine("\t\t\t\t\t\t\t{");
            file.WriteLine("\t\t\t\t\t\t\t\tOFFSET " + m_frames[0]["handTipRight"][0] + " " + m_frames[0]["handTipRight"][1] + " " + m_frames[0]["handTipRight"][2]);
            file.WriteLine("\t\t\t\t\t\t\t\tCHANNELS 3 Xposition Zposition Yposition");
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
            file.WriteLine("\t\tOFFSET " + m_frames[0]["hipLeft"][0] + " " + m_frames[0]["hipLeft"][1] + " " + m_frames[0]["hipLeft"][2]);
            file.WriteLine("\t\tCHANNELS 3 Xposition Zposition Yposition");
            file.WriteLine("\t\tJOINT kneeLeft");
            file.WriteLine("\t\t{");
            file.WriteLine("\t\t\tOFFSET " + m_frames[0]["kneeLeft"][0] + " " + m_frames[0]["kneeLeft"][1] + " " + m_frames[0]["kneeLeft"][2]);
            file.WriteLine("\t\t\tCHANNELS 3 Xposition Zposition Yposition");
            file.WriteLine("\t\t\tJOINT ankleLeft");
            file.WriteLine("\t\t\t{");
            file.WriteLine("\t\t\t\tOFFSET " + m_frames[0]["ankleLeft"][0] + " " + m_frames[0]["ankleLeft"][1] + " " + m_frames[0]["ankleLeft"][2]);
            file.WriteLine("\t\t\t\tCHANNELS 3 Xposition Zposition Yposition");
            file.WriteLine("\t\t\t\tJOINT footLeft");
            file.WriteLine("\t\t\t\t{");
            file.WriteLine("\t\t\t\t\tOFFSET " + m_frames[0]["footLeft"][0] + " " + m_frames[0]["footLeft"][1] + " " + m_frames[0]["footLeft"][2]);
            file.WriteLine("\t\t\t\t\tCHANNELS 3 Xposition Zposition Yposition");
            file.WriteLine("\t\t\t\t\tEnd Site");
            file.WriteLine("\t\t\t\t\t{");
            file.WriteLine("\t\t\t\t\t\tOFFSET 20 10 20");
            file.WriteLine("\t\t\t\t\t}");
            file.WriteLine("\t\t\t\t}");
            file.WriteLine("\t\t\t}");
            file.WriteLine("\t\t}");
            file.WriteLine("\t}");
            file.WriteLine("\tJOINT hipRight");
            file.WriteLine("\t{");
            file.WriteLine("\t\tOFFSET " + m_frames[0]["hipRight"][0] + " " + m_frames[0]["hipRight"][1] + " " + m_frames[0]["hipRight"][2]);
            file.WriteLine("\t\tCHANNELS 3 Xposition Zposition Yposition");
            file.WriteLine("\t\tJOINT kneeRight");
            file.WriteLine("\t\t{");
            file.WriteLine("\t\t\tOFFSET " + m_frames[0]["kneeRight"][0] + " " + m_frames[0]["kneeRight"][1] + " " + m_frames[0]["kneeRight"][2]);
            file.WriteLine("\t\t\tCHANNELS 3 Xposition Zposition Yposition");
            file.WriteLine("\t\t\tJOINT ankleRight");
            file.WriteLine("\t\t\t{");
            file.WriteLine("\t\t\t\tOFFSET " + m_frames[0]["ankleRight"][0] + " " + m_frames[0]["ankleRight"][1] + " " + m_frames[0]["ankleRight"][2]);
            file.WriteLine("\t\t\t\tCHANNELS 3 Xposition Zposition Yposition");
            file.WriteLine("\t\t\t\tJOINT footRight");
            file.WriteLine("\t\t\t\t{");
            file.WriteLine("\t\t\t\t\tOFFSET " + m_frames[0]["footRight"][0] + " " + m_frames[0]["footRight"][1] + " " + m_frames[0]["footRight"][2]);
            file.WriteLine("\t\t\t\t\tCHANNELS 3 Xposition Zposition Yposition");
            file.WriteLine("\t\t\t\t\tEnd Site");
            file.WriteLine("\t\t\t\t\t{");
            file.WriteLine("\t\t\t\t\t\tOFFSET 20 10 20");
            file.WriteLine("\t\t\t\t\t}");
            file.WriteLine("\t\t\t\t}");
            file.WriteLine("\t\t\t}");
            file.WriteLine("\t\t}");
            file.WriteLine("\t}");
            file.WriteLine("}");
             */
            
            file.WriteLine("HIERARCHY");
            file.WriteLine("ROOT spineBase");
            file.WriteLine("{");
            file.WriteLine("\tOFFSET 0 0 0");
            file.WriteLine("\tCHANNELS 6 Xposition Zposition Yposition Xrotation Zrotation Yrotation");
            file.WriteLine("\tJOINT spineMid");
            file.WriteLine("\t{");
            file.WriteLine("\t\tOFFSET " + m_frames[0]["spineMid"][3] + " " + m_frames[0]["spineMid"][4] + " " + m_frames[0]["spineMid"][5]);
            file.WriteLine("\t\tCHANNELS 3 Xrotation Zrotation Yrotation");
            file.WriteLine("\t\tJOINT spineShoulder");
            file.WriteLine("\t\t{");
            file.WriteLine("\t\t\tOFFSET " + m_frames[0]["spineShoulder"][3] + " " + m_frames[0]["spineShoulder"][4] + " " + m_frames[0]["spineShoulder"][5]);
            file.WriteLine("\t\t\tCHANNELS 3 Xrotation Zrotation Yrotation");
            file.WriteLine("\t\t\tJOINT neck");
            file.WriteLine("\t\t\t{");
            file.WriteLine("\t\t\t\tOFFSET " + m_frames[0]["neck"][3] + " " + m_frames[0]["neck"][4] + " " + m_frames[0]["neck"][5]);
            file.WriteLine("\t\t\t\tCHANNELS 3 Xrotation Zrotation Yrotation");
            file.WriteLine("\t\t\t\tJOINT head");
            file.WriteLine("\t\t\t\t{");
            file.WriteLine("\t\t\t\t\tOFFSET " + m_frames[0]["head"][3] + " " + m_frames[0]["head"][4] + " " + m_frames[0]["head"][5]);
            file.WriteLine("\t\t\t\t\tCHANNELS 3 Xrotation Zrotation Yrotation");
            file.WriteLine("\t\t\t\t\tEnd Site");
            file.WriteLine("\t\t\t\t\t{");
            file.WriteLine("\t\t\t\t\t\tOFFSET 0 0 0"); /// not sure what this is
            file.WriteLine("\t\t\t\t\t}");
            file.WriteLine("\t\t\t\t}");
            file.WriteLine("\t\t\t}");
            file.WriteLine("\t\t\tJOINT shoulderLeft");
            file.WriteLine("\t\t\t{");
            file.WriteLine("\t\t\t\tOFFSET " + m_frames[0]["shoulderLeft"][3] + " " + m_frames[0]["shoulderLeft"][4] + " " + m_frames[0]["shoulderLeft"][5]);
            file.WriteLine("\t\t\t\tCHANNELS 3 Xrotation Zrotation Yrotation");
            file.WriteLine("\t\t\t\tJOINT elbowLeft");
            file.WriteLine("\t\t\t\t{");
            file.WriteLine("\t\t\t\t\tOFFSET " + m_frames[0]["elbowLeft"][3] + " " + m_frames[0]["elbowLeft"][4] + " " + m_frames[0]["elbowLeft"][5]);
            file.WriteLine("\t\t\t\t\tCHANNELS 3 Xrotation Zrotation Yrotation");
            file.WriteLine("\t\t\t\t\tJOINT wristLeft");
            file.WriteLine("\t\t\t\t\t{");
            file.WriteLine("\t\t\t\t\t\tOFFSET " + m_frames[0]["wristLeft"][3] + " " + m_frames[0]["wristLeft"][4] + " " + m_frames[0]["wristLeft"][5]);
            file.WriteLine("\t\t\t\t\t\tCHANNELS 3 Xrotation Zrotation Yrotation");
            file.WriteLine("\t\t\t\t\t\tJOINT handLeft");
            file.WriteLine("\t\t\t\t\t\t{");
            file.WriteLine("\t\t\t\t\t\t\tOFFSET " + m_frames[0]["handLeft"][3] + " " + m_frames[0]["handLeft"][4] + " " + m_frames[0]["handLeft"][5]);
            file.WriteLine("\t\t\t\t\t\t\tCHANNELS 3 Xrotation Zrotation Yrotation");
            file.WriteLine("\t\t\t\t\t\t\tJOINT handTipLeft");
            file.WriteLine("\t\t\t\t\t\t\t{");
            file.WriteLine("\t\t\t\t\t\t\t\tOFFSET " + m_frames[0]["handTipLeft"][3] + " " + m_frames[0]["handTipLeft"][4] + " " + m_frames[0]["handTipLeft"][5]);
            file.WriteLine("\t\t\t\t\t\t\t\tCHANNELS 3 Xrotation Zrotation Yrotation");
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
            file.WriteLine("\t\t\t\tOFFSET " + m_frames[0]["shoulderRight"][3] + " " + m_frames[0]["shoulderRight"][4] + " " + m_frames[0]["shoulderRight"][5]);
            file.WriteLine("\t\t\t\tCHANNELS 3 Xrotation Zrotation Yrotation");
            file.WriteLine("\t\t\t\tJOINT elbowRight");
            file.WriteLine("\t\t\t\t{");
            file.WriteLine("\t\t\t\t\tOFFSET " + m_frames[0]["elbowRight"][3] + " " + m_frames[0]["elbowRight"][4] + " " + m_frames[0]["elbowRight"][5]);
            file.WriteLine("\t\t\t\t\tCHANNELS 3 Xrotation Zrotation Yrotation");
            file.WriteLine("\t\t\t\t\tJOINT wristRight");
            file.WriteLine("\t\t\t\t\t{");
            file.WriteLine("\t\t\t\t\t\tOFFSET " + m_frames[0]["wristRight"][3] + " " + m_frames[0]["wristRight"][4] + " " + m_frames[0]["wristRight"][5]);
            file.WriteLine("\t\t\t\t\t\tCHANNELS 3 Xrotation Zrotation Yrotation");
            file.WriteLine("\t\t\t\t\t\tJOINT handRight");
            file.WriteLine("\t\t\t\t\t\t{");
            file.WriteLine("\t\t\t\t\t\t\tOFFSET " + m_frames[0]["handRight"][3] + " " + m_frames[0]["handRight"][4] + " " + m_frames[0]["handRight"][5]);
            file.WriteLine("\t\t\t\t\t\t\tCHANNELS 3 Xrotation Zrotation Yrotation");
            file.WriteLine("\t\t\t\t\t\t\tJOINT handTipRight");
            file.WriteLine("\t\t\t\t\t\t\t{");
            file.WriteLine("\t\t\t\t\t\t\t\tOFFSET " + m_frames[0]["handTipRight"][3] + " " + m_frames[0]["handTipRight"][4] + " " + m_frames[0]["handTipRight"][5]);
            file.WriteLine("\t\t\t\t\t\t\t\tCHANNELS 3 Xrotation Zrotation Yrotation");
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
            file.WriteLine("\t\tOFFSET " + m_frames[0]["hipLeft"][3] + " " + m_frames[0]["hipLeft"][1] + " " + m_frames[0]["hipLeft"][2]);
            file.WriteLine("\t\tCHANNELS 3 Xrotation Zrotation Yrotation");
            file.WriteLine("\t\tJOINT kneeLeft");
            file.WriteLine("\t\t{");
            file.WriteLine("\t\t\tOFFSET " + m_frames[0]["kneeLeft"][3] + " " + m_frames[0]["kneeLeft"][1] + " " + m_frames[0]["kneeLeft"][2]);
            file.WriteLine("\t\t\tCHANNELS 3 Xrotation Zrotation Yrotation");
            file.WriteLine("\t\t\tJOINT ankleLeft");
            file.WriteLine("\t\t\t{");
            file.WriteLine("\t\t\t\tOFFSET " + m_frames[0]["ankleLeft"][3] + " " + m_frames[0]["ankleLeft"][1] + " " + m_frames[0]["ankleLeft"][2]);
            file.WriteLine("\t\t\t\tCHANNELS 3 Xrotation Zrotation Yrotation");
            file.WriteLine("\t\t\t\tJOINT footLeft");
            file.WriteLine("\t\t\t\t{");
            file.WriteLine("\t\t\t\t\tOFFSET " + m_frames[0]["footLeft"][3] + " " + m_frames[0]["footLeft"][1] + " " + m_frames[0]["footLeft"][2]);
            file.WriteLine("\t\t\t\t\tCHANNELS 3 Xrotation Zrotation Yrotation");
            file.WriteLine("\t\t\t\t\tEnd Site");
            file.WriteLine("\t\t\t\t\t{");
            file.WriteLine("\t\t\t\t\t\tOFFSET 20 10 20");
            file.WriteLine("\t\t\t\t\t}");
            file.WriteLine("\t\t\t\t}");
            file.WriteLine("\t\t\t}");
            file.WriteLine("\t\t}");
            file.WriteLine("\t}");
            file.WriteLine("\tJOINT hipRight");
            file.WriteLine("\t{");
            file.WriteLine("\t\tOFFSET " + m_frames[0]["hipRight"][3] + " " + m_frames[0]["hipRight"][1] + " " + m_frames[0]["hipRight"][2]);
            file.WriteLine("\t\tCHANNELS 3 Xrotation Zrotation Yrotation");
            file.WriteLine("\t\tJOINT kneeRight");
            file.WriteLine("\t\t{");
            file.WriteLine("\t\t\tOFFSET " + m_frames[0]["kneeRight"][3] + " " + m_frames[0]["kneeRight"][1] + " " + m_frames[0]["kneeRight"][2]);
            file.WriteLine("\t\t\tCHANNELS 3 Xrotation Zrotation Yrotation");
            file.WriteLine("\t\t\tJOINT ankleRight");
            file.WriteLine("\t\t\t{");
            file.WriteLine("\t\t\t\tOFFSET " + m_frames[0]["ankleRight"][3] + " " + m_frames[0]["ankleRight"][1] + " " + m_frames[0]["ankleRight"][2]);
            file.WriteLine("\t\t\t\tCHANNELS 3 Xrotation Zrotation Yrotation");
            file.WriteLine("\t\t\t\tJOINT footRight");
            file.WriteLine("\t\t\t\t{");
            file.WriteLine("\t\t\t\t\tOFFSET " + m_frames[0]["footRight"][3] + " " + m_frames[0]["footRight"][1] + " " + m_frames[0]["footRight"][2]);
            file.WriteLine("\t\t\t\t\tCHANNELS 3 Xrotation Zrotation Yrotation");
            file.WriteLine("\t\t\t\t\tEnd Site");
            file.WriteLine("\t\t\t\t\t{");
            file.WriteLine("\t\t\t\t\t\tOFFSET 20 10 20");
            file.WriteLine("\t\t\t\t\t}");
            file.WriteLine("\t\t\t\t}");
            file.WriteLine("\t\t\t}");
            file.WriteLine("\t\t}");
            file.WriteLine("\t}");
            file.WriteLine("}");
            
            file.WriteLine("MOTION");
            file.WriteLine("Frames: " + m_frames.Count);
            file.WriteLine("Frame Time: 0.0333333");

            //Console.WriteLine("Frame count: " + m_frames.Count);
            //file.WriteLine(m_frames.Count);
            
            for (int i = 1; i < m_frames.Count; i++)
            {
                //Console.WriteLine("Bones count: " + m_frames[i].Count);
                //for (int j = 0; j < m_frames[i].Count; j++)
                // {
                //file.Write(j + " ");
                foreach (var item in m_frames[i])
                {
                    string key = item.Key;
                    //Console.WriteLine(key);
                    float valsX = m_frames[i][key][0];
                    float valsY = m_frames[i][key][1];
                    float valsZ = m_frames[i][key][2];

                    //Console.WriteLine(vals);
                    file.Write(valsX + " " + valsY + " " + valsZ + " ");
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
                        int oMulti = 10;
                        int mult = 10;
                        tmp.Add("spineBase", new float[] { spineBase.Position.X * mult, spineBase.Position.Y * mult, spineBase.Position.Z * mult, spineBaseOri.Orientation.X  * oMulti , spineBaseOri.Orientation.Y  * oMulti , spineBaseOri.Orientation.Z * oMulti });
                        tmp.Add("spineMid", new float[] { spineMid.Position.X * mult, spineMid.Position.Y * mult, spineMid.Position.Z * mult, spineMidOri.Orientation.X  * oMulti, spineMidOri.Orientation.Y * oMulti, spineMidOri.Orientation.Z * oMulti });
                        tmp.Add("spineShoulder", new float[] { spineShoulder.Position.X * mult, spineShoulder.Position.Y * mult, spineShoulder.Position.Z * mult, spineShoulderOri.Orientation.X  * oMulti, spineShoulderOri.Orientation.Y * oMulti, spineShoulderOri.Orientation.Z * oMulti });
                        tmp.Add("neck", new float[] { neck.Position.X * mult, neck.Position.Y * mult, neck.Position.Z * mult, neckOri.Orientation.X  * oMulti, neckOri.Orientation.Y  * oMulti, neckOri.Orientation.Z * oMulti });
                        tmp.Add("head", new float[] { head.Position.X * mult, head.Position.Y * mult, head.Position.Z * mult, headOri.Orientation.X  * oMulti, headOri.Orientation.Y  * oMulti, headOri.Orientation.Z * oMulti });
                        tmp.Add("shoulderLeft", new float[] { shoulderLeft.Position.X * mult, shoulderLeft.Position.Y * mult, shoulderLeft.Position.Z * mult, shoulderLeftOri.Orientation.X  * oMulti, shoulderLeftOri.Orientation.Y * oMulti, shoulderLeftOri.Orientation.Z * oMulti});
                        tmp.Add("elbowLeft", new float[] { elbowLeft.Position.X * mult, elbowLeft.Position.Y * mult, elbowLeft.Position.Z * mult, elbowLeftOri.Orientation.X * oMulti, elbowLeftOri.Orientation.Y * oMulti, elbowLeftOri.Orientation.Z * oMulti });
                        tmp.Add("wristLeft", new float[] { wristLeft.Position.X * mult, wristLeft.Position.Y * mult, wristLeft.Position.Z * mult, wristLeftOri.Orientation.X * oMulti, wristLeftOri.Orientation.Y * oMulti, wristLeftOri.Orientation.Z * oMulti });
                        tmp.Add("handLeft", new float[] { handLeft.Position.X * mult, handLeft.Position.Y * mult, handLeft.Position.Z * mult, handLeftOri.Orientation.X * oMulti, handLeftOri.Orientation.Y * oMulti, handLeftOri.Orientation.Z * oMulti });
                        tmp.Add("handTipLeft", new float[] { handTipLeft.Position.X * mult, handTipLeft.Position.Y * mult, handTipLeft.Position.Z * mult, handTipLeftOri.Orientation.X * oMulti, handTipLeftOri.Orientation.Y * oMulti, handTipLeftOri.Orientation.Z * oMulti });

                        tmp.Add("shoulderRight", new float[] { shoulderRight.Position.X * mult, shoulderRight.Position.Y * mult, shoulderRight.Position.Z * mult, shoulderRightOri.Orientation.X  * oMulti, shoulderRightOri.Orientation.Y * oMulti, shoulderRightOri.Orientation.Z * oMulti });
                        tmp.Add("elbowRight", new float[] { elbowRight.Position.X * mult, elbowRight.Position.Y * mult, elbowRight.Position.Z * mult, elbowRightOri.Orientation.X  * oMulti, elbowRightOri.Orientation.Y  * oMulti, elbowRightOri.Orientation.Z * oMulti });
                        tmp.Add("wristRight", new float[] { wristRight.Position.X * mult, wristRight.Position.Y * mult, wristRight.Position.Z * mult, wristRightOri.Orientation.X  * oMulti, wristRightOri.Orientation.Y  * oMulti, wristRightOri.Orientation.Z * oMulti });                        
                        tmp.Add("handRight", new float[] { handRight.Position.X * mult, handRight.Position.Y * mult, handRight.Position.Z * mult, handRightOri.Orientation.X  * oMulti, handRightOri.Orientation.Y  * oMulti, handRightOri.Orientation.Z * oMulti });                        
                        tmp.Add("handTipRight", new float[] { handTipRight.Position.X * mult, handTipRight.Position.Y * mult, handTipRight.Position.Z * mult, handTipRightOri.Orientation.X  * oMulti, handTipRightOri.Orientation.Y  * oMulti, handTipRightOri.Orientation.Z * oMulti });
                        tmp.Add("hipLeft", new float[] { hipLeft.Position.X * mult, hipLeft.Position.Y * mult, hipLeft.Position.Z * mult, hipLeftOri.Orientation.X  * oMulti, hipLeftOri.Orientation.Y  * oMulti, hipLeftOri.Orientation.Z * oMulti });
                        tmp.Add("kneeLeft", new float[] { kneeLeft.Position.X * mult, kneeLeft.Position.Y * mult, kneeLeft.Position.Z * mult, kneeLeftOri.Orientation.X  * oMulti, kneeLeftOri.Orientation.Y  * oMulti, kneeLeftOri.Orientation.Z * oMulti });
                        tmp.Add("ankleLeft", new float[] { ankleLeft.Position.X * mult, ankleLeft.Position.Y * mult, ankleLeft.Position.Z * mult, ankleLeftOri.Orientation.X  * oMulti, ankleLeftOri.Orientation.Y  * oMulti, ankleLeftOri.Orientation.Z * oMulti });   
                        tmp.Add("footLeft", new float[] { footLeft.Position.X * mult, footLeft.Position.Y * mult, footLeft.Position.Z * mult, footLeftOri.Orientation.X  * oMulti, footLeftOri.Orientation.Y  * oMulti, footLeftOri.Orientation.Z * oMulti });
                        
                        tmp.Add("hipRight", new float[] { hipRight.Position.X * mult, hipRight.Position.Y * mult, hipRight.Position.Z * mult, hipRightOri.Orientation.X  * oMulti, hipRightOri.Orientation.Y  * oMulti, hipRightOri.Orientation.Z * oMulti });
                        tmp.Add("kneeRight", new float[] { kneeRight.Position.X * mult, kneeRight.Position.Y * mult, kneeRight.Position.Z * mult, kneeRightOri.Orientation.X  * oMulti, kneeRightOri.Orientation.Y  * oMulti, kneeRightOri.Orientation.Z * oMulti });                       
                        tmp.Add("ankleRight", new float[] { ankleRight.Position.X * mult, ankleRight.Position.Y * mult, ankleRight.Position.Z * mult, ankleRightOri.Orientation.X  * oMulti, ankleRightOri.Orientation.Y  * oMulti, ankleRightOri.Orientation.Z * oMulti });
                        tmp.Add("footRight", new float[] { footRight.Position.X * mult, footRight.Position.Y * mult, footRight.Position.Z * mult, footRightOri.Orientation.X  * oMulti, footRightOri.Orientation.Y  * oMulti, footRightOri.Orientation.Z * oMulti });                                
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

                        float ms_distance_x = tmp["wristRight"][3];
                        float ms_distance_y = tmp["wristRight"][4];
                        float ms_distance_z = tmp["wristRight"][5];
                       
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

        private void Form1_Load(object sender, EventArgs e)
        {

        }

        private void button2_Click(object sender, EventArgs e)
        {
            ready = true;
        }
    }
}
