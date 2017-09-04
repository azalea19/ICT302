using System.Drawing;
using UnityEngine;
//using Emgu.CV.Structure;
//using Emgu.CV;
using Windows.Kinect;
using System.Drawing.Imaging;
using System.Runtime.InteropServices;

/*
 * MANY LINES OF CODE IN THIS SECTION ARE COMMENTED OUT, AS THEY ARE A PART OF THE ATTEMPT KRS MADE TOWARDS COLOUR TRACKING
 */

namespace SportsKinematics
{
    /// <summary>
    /// Tracks Player position.
    /// </summary>
    public class PlayerTracker : MonoBehaviour
    {
        /// <summary>
        /// Kinect Facade gameobject.
        /// </summary>
        public GameObject kinectFacade;
        /// <summary>
        /// Kinect Facade object.
        /// </summary>
        private CaptureFacade m_KinectFacade;

        /// <summary>
        /// Striker gameobject.
        /// </summary>
        public GameObject m_striker;
        /// <summary>
        /// True if the user is right handed.
        /// </summary>
        public bool m_handRight;
        /// <summary>
        /// Scales the color tracking detection box.
        /// </summary>
        private float m_scalingFactor = 10.0f;
        /// <summary>
        /// Data pertaining to the incoming frame.
        /// </summary>
        private FrameDescription m_colourFrameDescription;
        /// <summary>
        /// Reference to ColorFrameReader
        /// </summary>
        private ColorFrameReader m_colourReader;
        /// <summary>
        /// The body being tracked.
        /// </summary>
        private GameObject trackedBody;
        /// <summary>
        /// The collision wall attached to the user hand.
        /// </summary>
        private GameObject m_wall;
        /// <summary>
        /// True if hit has occured.
        /// </summary>
        private bool m_hitOccured;

        //private KinectSensor m_sensor;
        //private int m_imgWidthHalf, m_imgHeightHalf;
        //private List<Point> m_pixLocations;
        //private Transform m_handTran;
        //private System.Drawing.Color m_colourSysDraw;
        //private Image<Bgr, byte> imageFrame;
        //private float m_xFocal, m_yFocal, m_xFocalInv, m_yFocalInv;

        /// <summary>
        /// Getter and Setter for the hit occured boolean.
        /// </summary>
        public bool HitOccured
        {
            get { return m_hitOccured; }
            set { m_hitOccured = value; }
        }

        /// <summary>
        /// Initialise PlayerTracker member variables.
        /// </summary>
        void Start()
        {
            m_KinectFacade = kinectFacade.GetComponent<CaptureFacade>();
            //m_sensor = m_KinectFacade.MultiManager.KinectSensor;

            //m_colTrack = new ColourTracker();

            //m_colourSysDraw = System.Drawing.Color.FromArgb((int)(m_colourUE.r * 255), (int)(m_colourUE.g * 255), (int)(m_colourUE.b * 255));

            //m_colTrack.m_colourTrackRGB = new Bgr(m_colourSysDraw.B, m_colourSysDraw.G, m_colourSysDraw.R);

            //m_xFocal = m_sensor.CoordinateMapper.GetDepthCameraIntrinsics().FocalLengthX;
            //m_yFocal = m_sensor.CoordinateMapper.GetDepthCameraIntrinsics().FocalLengthY;
            //m_xFocalInv = 1.0f / m_xFocal;
            //m_yFocalInv = 1.0f / m_yFocal;

            m_KinectFacade.MultiManager.KinectSensor.Open();
            m_colourReader = m_KinectFacade.MultiManager.KinectSensor.ColorFrameSource.OpenReader();

            //m_handTran = new GameObject().transform;
            //m_colourReader.FrameArrived += colourReader_ColorFrameArrived;            
            //m_KinectFacade.MultiManager.KinectSensor.ColorFrameSource.OpenReader().FrameArrived += colourReader_ColorFrameArrived;
        }

        /// <summary>
        /// Creates the wall collision object.
        /// </summary>
        /// <param name="bodyJoint">Transform to place the wall.</param>
        private void CreateCollisionPlane(Transform bodyJoint)
        {
            m_wall = GameObject.CreatePrimitive(PrimitiveType.Cube);
            m_wall.name = "BallDetector";
            m_wall.GetComponent<MeshRenderer>().enabled = false;
            m_wall.AddComponent<BoxCollider>();
            m_wall.GetComponent<BoxCollider>().isTrigger = true;

            m_wall.AddComponent<BallPassPlayer>();
            m_wall.GetComponent<BallPassPlayer>().SpineShoulder = bodyJoint.parent.FindChild((JointType.SpineShoulder).ToString()).gameObject;

            m_wall.transform.position = bodyJoint.position;
            m_wall.transform.localScale = new Vector3(100, 100, 1);
        }

        /// <summary>
        /// Attaches the collision wall to the joint specified to the
        /// bodyJoint transform.
        /// </summary>
        /// <param name="bodyJoint">Transform of the joint to attach the wall to.</param>
        private void UpdateCollisionPlane(Transform bodyJoint)
        {
            m_wall.GetComponent<BallPassPlayer>().SpineShoulder = bodyJoint.parent.FindChild((JointType.SpineShoulder).ToString()).gameObject;
            m_wall.transform.position = bodyJoint.position;
        }

        /// <summary>
        /// Ensures the collision wall is attached to the players left or right hand, depending
        /// on the configuration settings.
        /// </summary>
        private void Update()
        {
            trackedBody = m_KinectFacade.BodyView.TrackedBody;

            if (trackedBody)
            {
                if (m_handRight)
                {
                    //m_striker.GetComponent<StrikerRenderer>().CreateStriker(trackedBody.transform.FindChild("HandRight"), true);
                    if (!m_wall)
                        CreateCollisionPlane(trackedBody.transform.FindChild("HandRight"));
                    else
                        UpdateCollisionPlane(trackedBody.transform.FindChild("HandRight"));
                }
                else
                {
                    //m_striker.GetComponent<StrikerRenderer>().CreateStriker(trackedBody.transform.FindChild("HandLeft"), true);
                    if (!m_wall)
                        CreateCollisionPlane(trackedBody.transform.FindChild("HandLeft"));
                    else
                        UpdateCollisionPlane(trackedBody.transform.FindChild("HandLeft"));
                }
            }
        }

        /// <summary>
        /// Retrieves user hand and sport type from the specified configuration.
        /// </summary>
        /// <param name="conf">Configuration to retrieve data from.</param>
        public void LoadConf(Configuration conf)
        {
            m_handRight = conf.UserRightHanded;
            m_striker.GetComponent<StrikerRenderer>().m_StrikerSelection = conf.Sport;
        }

        /// <summary>
        /// Converts a colour coordinate to a depth value.
        /// </summary>
        /// <param name="colourCord">Colour values.</param>
        /// <param name="colourWidth">Colour width.</param>
        /// <returns>Depth value.</returns>
        private ushort ColourCordToDepthVal(Point colourCord, int colourWidth)
        {
            ushort[] depthData = m_KinectFacade.DepthView.DepthManager.GetComponentInChildren<DepthSourceManager>().GetData();
            return depthData[(colourWidth + 1) * colourCord.Y + colourCord.X + 1];
        }

        /// <summary>
        /// Fires when a frame is captured. Draws paddle to hand
        /// </summary>
        /// <param name="sender">object sending event</param>
        /// <param name="e">Event that is fired. Features all frame details</param>
        /// <author>Aiden Triffitt</author>
        /// <date>14/04/2017</date>
        //private void colourReader_ColorFrameArrived(object sender, ColorFrameArrivedEventArgs e)//FR1 - Virtual opponent modelling from captured data. 
        //{
        //    using (ColorFrame frame = e.FrameReference.AcquireFrame())
        //    {
        //        if (frame == null)
        //        {
        //            return;
        //        }

        //        CameraSpacePoint handPoint = new CameraSpacePoint();

        //        if (m_handRight)
        //            handPoint = m_KinectFacade.BodyView.GetComponentInChildren<BodySourceView>().GetJoint(Windows.Kinect.JointType.HandRight).Position;
        //        else
        //            handPoint = m_KinectFacade.BodyView.GetComponentInChildren<BodySourceView>().GetJoint(Windows.Kinect.JointType.HandLeft).Position;

        //        m_handTran.position.Set(-handPoint.X * m_scalingFactor, handPoint.Y * m_scalingFactor, handPoint.Z * m_scalingFactor);
        //        m_striker.GetComponentInChildren<StrikerRenderer>().CreateStriker(m_handTran, true);

                /**ALL ATTACHED COMMENTS ARE AN APPROACH TO cOLOUR TRACKING AND ARE HIGHLY UNSTABLE**/

                //Point m_pixLocation;
                //CameraSpacePoint handPoint = new CameraSpacePoint();
                //ColorSpacePoint colPoint = new ColorSpacePoint();

                //int width = frame.FrameDescription.Width;
                //int height = frame.FrameDescription.Height;

                //m_imgHeightHalf = height /2;
                //m_imgWidthHalf = width /2; // halve the dimensions of the frame. Frame indexes are 0 to n-1, but IR camera operates from -2 to 2. Adding half n to IR.X and IR.Y will normalise the range to 0 to 4

                //if (m_handRight)
                //    handPoint = m_KinectFacade.BodyView.GetComponentInChildren<BodySourceView>().GetJoint(Windows.Kinect.JointType.HandRight).Position;
                //else
                //    handPoint = m_KinectFacade.BodyView.GetComponentInChildren<BodySourceView>().GetJoint(Windows.Kinect.JointType.HandLeft).Position;

                //float camPixelWidth = 0.000028f;
                //float camPixelHeight = (width/height) * camPixelWidth;

                //colPoint = m_sensor.CoordinateMapper.MapCameraPointToColorSpace(handPoint);

                //byte[] data = new byte[width * height * Image.GetPixelFormatSize(PixelFormat.Format32bppRgb) / 8]; //Bits in a byte

                //frame.CopyConvertedFrameDataToArray(data, ColorImageFormat.Bgra);

                //Bitmap bitmap = new System.Drawing.Bitmap(width, height, PixelFormat.Format32bppRgb);

                //BitmapData bitmapData = bitmap.LockBits(
                //    new System.Drawing.Rectangle(0, 0, bitmap.Width, bitmap.Height),
                //    ImageLockMode.WriteOnly,
                //    bitmap.PixelFormat);
                //Marshal.Copy(data, 0, bitmapData.Scan0, data.Length);
                //bitmap.UnlockBits(bitmapData);

                //imageFrame = new Image<Bgr, byte>(bitmap);

                //m_pixLocation = m_colTrack.FindColour(imageFrame, new Point((int)colPoint.X, (int)colPoint.Y), 50, 50, 50, 100, 100);

                //if (m_pixLocation.X != -1.0f)
                //{
                //    colPoint.X = (m_pixLocation.X - m_imgWidthHalf) * ((m_xFocal * camPixelWidth) * handPoint.Z);
                //    colPoint.Y = (m_pixLocation.Y - m_imgHeightHalf) * ((m_yFocal * camPixelHeight) * handPoint.Z);

                //    m_handLocation.Set(-colPoint.X, -colPoint.Y, handPoint.Z * m_scalingFactor);
                //    m_handLocation.Set(-handPoint.X * m_scalingFactor, handPoint.Y * m_scalingFactor, handPoint.Z * m_scalingFactor);

                //    m_striker.GetComponentInChildren<Striker>().DrawPaddle(m_handLocation);
                //}
            //}
        //}
    }
}