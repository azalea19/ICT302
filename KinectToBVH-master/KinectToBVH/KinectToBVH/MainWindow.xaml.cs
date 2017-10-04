using Microsoft.Kinect;
using System;
using System.IO;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Media3D;
using System.Windows.Shapes;

namespace KinectToBVH
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        // connected kinect sensor
        private KinectSensor sensor;

        // under recording state or not
        private bool underRecording = false;

        // edge brush
        private Brush BRUSH_EDGE = Brushes.Red;
        private const double DOUBLE_THICKNESS_EDGE = 10;

        // bone brush
        private Brush BRUSH_BONE = Brushes.Green;
        private const double DOUBLE_THICKNESS_BONE = 8;
        private Brush BRUSH_JOINT = Brushes.White;
        private const double DOUBLE_RADIUS_JOINT = 10;
        private Brush BRUSH_BACKGROUND = Brushes.Black;
        private Brush BRUSH_BACKGROUNDCLIBRATED = Brushes.DeepSkyBlue;
        BodyFrameReader bReader = null;
        // skeleton structure
        private KinectSkeleton _kinectSkeleton;

        // for debug
        private Brush BTUSH_BONE_DEBUG = Brushes.Black;
        private const double DOUBLE_THICKNESS_BONE_DEBUG = 6;

        //BVH writer;
        private BVHFile bvhWriter;

        // skeleton smooth parameter
        //private TransformSmoothParameters smoothParam;

        public MainWindow()
        {
            InitializeComponent();
            _kinectSkeleton = new KinectSkeleton();
            bvhWriter = new BVHFile(_kinectSkeleton,"kinect.bvh");
        }
        

        /// <summary>
        /// Execute startup tasks
        /// </summary>
        /// <param name="sender">object sending the event</param>
        /// <param name="e">event arguments</param>
        private void WindowLoaded(object sender, RoutedEventArgs e)
        {
            // Look through all sensors and start the first connected one.

            this.sensor = KinectSensor.GetDefault();
              
            

            if (null != this.sensor)
            {
                // Turn on the skeleton stream to receive skeleton frames
                setKinectSmoothParameter();
                //this.sensor.SkeletonStream.Enable(smoothParam);

                //// Add an event handler to be called whenever there is new frame data
                //this.sensor.SkeletonFrameReady += SensorSkeletonFrameReady;
                bReader = sensor.BodyFrameSource.OpenReader();
                if (bReader != null)
                {
                    bReader.FrameArrived += SensorSkeletonFrameReady;
                }
                // Start the sensor!
                try
                {
                    this.sensor.Open();
                }
                catch (IOException)
                {
                    this.sensor = null;
                }
            }

            if (null == this.sensor)
            {
                // todo: error log
            }

        }

        /// <summary>
        /// Execute shutdown tasks
        /// </summary>
        /// <param name="sender">object sending the event</param>
        /// <param name="e">event arguments</param>
        private void WindowClosing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if (null != this.sensor)
            {
                this.sensor.Close();
            }

           // bvhWriter.OutputBVHToFile();
        }

        private void setKinectSmoothParameter()
        {
            //this.smoothParam = new TransformSmoothParameters();
            //smoothParam.Smoothing = 0.5f; // 0-1, bigger - smoother
            //smoothParam.Correction = 0.5f; // 0-1, smaller - smoother
            //smoothParam.Prediction = 1.0f;
            //smoothParam.JitterRadius = 1.0f;
            //smoothParam.MaxDeviationRadius = 1.0f;
        }

        private void SensorSkeletonFrameReady(object sender, BodyFrameArrivedEventArgs e)
        {
            Body[] skeletons = new Body[0];
            long frameTimeStamp = 0;

            using (BodyFrame skeletonFrame = e.FrameReference.AcquireFrame())
            {
                if (skeletonFrame != null)
                {
                    skeletons = new Body[skeletonFrame.BodyCount];
                    skeletonFrame.GetAndRefreshBodyData(skeletons);
                    //frameTimeStamp = skeletonFrame.Timestamp;
                }
            }

            if (skeletons.Length != 0)
            {
                canvas.Children.Clear();
                foreach (Body skel in skeletons)
                {
                    // render clipped edges
                    renderClippedEdges(skel);

                    if (skel.IsTracked)
                    {
                        // clibrate the bones if not done yet
                        _kinectSkeleton.CliberateSkeleton(skel);

                        // render the skeleton on the canvas
                        addBones(skel);

                        if (!_kinectSkeleton.NeedBoneClibrated())
                        {
                            canvas.Background = BRUSH_BACKGROUNDCLIBRATED;
                            _kinectSkeleton.UpdateJoints(skel);

                            // record motion data to bvh file
                            if (underRecording)
                            {
                                bvhWriter.AppendOneMotionFrame();
                            }

                            addBVHBones_Debug();
                        }
                    }
                }
            }
        }

        /// <summary>
        /// add extra edge to viewport if the related edge is clipped by kinect
        /// </summary>
        /// <param name="skeleton"></param>
        private void renderClippedEdges(Body skeleton)
        {
            double halfThickness = DOUBLE_THICKNESS_EDGE / 2;
            if (skeleton.ClippedEdges.HasFlag(FrameEdges.Left))
                addLineToWindow(halfThickness, 0, halfThickness, canvas.Height, BRUSH_EDGE, DOUBLE_THICKNESS_EDGE);
            if (skeleton.ClippedEdges.HasFlag(FrameEdges.Top))
                addLineToWindow(0, halfThickness, canvas.Width, halfThickness, BRUSH_EDGE, DOUBLE_THICKNESS_EDGE);
            if (skeleton.ClippedEdges.HasFlag(FrameEdges.Right))
                addLineToWindow(canvas.Width - halfThickness, 0, canvas.Width - halfThickness, canvas.Height, BRUSH_EDGE, DOUBLE_THICKNESS_EDGE);
            if (skeleton.ClippedEdges.HasFlag(FrameEdges.Bottom))
                addLineToWindow(halfThickness, canvas.Height - halfThickness, canvas.Width, canvas.Height - halfThickness, BRUSH_EDGE, DOUBLE_THICKNESS_EDGE);
        }

        /// <summary>
        /// Add one bone to screen
        /// </summary>
        /// <param name="skeleton"></param>
        /// <param name="joint1"></param>
        /// <param name="joint2"></param>
        private void addBone(Body skeleton, JointType joint1, JointType joint2)
        {
            if (TrackingState.Tracked == skeleton.Joints[joint1].TrackingState
                && TrackingState.Tracked == skeleton.Joints[joint2].TrackingState)
            {
                DepthSpacePoint[] temp = new DepthSpacePoint[2];
                
                CameraSpacePoint[] points = { skeleton.Joints[joint1].Position, skeleton.Joints[joint2].Position };
               this.sensor.CoordinateMapper.MapCameraPointsToDepthSpace(points,
                    temp);
                addLineToWindow(temp[0].X, temp[0].Y, temp[1].X, temp[1].Y,
                    BRUSH_BONE, DOUBLE_THICKNESS_BONE);
            }
        }

        /// <summary>
        /// Add line to window
        /// </summary>
        /// <param name="x1"></param>
        /// <param name="y1"></param>
        /// <param name="x2"></param>
        /// <param name="y2"></param>
        /// <param name="brush"></param>
        /// <param name="thickness"></param>
        private void addLineToWindow(double x1, double y1, double x2, double y2, Brush brush, double thickness)
        {
            // add bone
            Line line = new Line();
            line.Stroke = brush;
            line.StrokeThickness = thickness;
            line.X1 = x1;
            line.Y1 = y1;
            line.X2 = x2;
            line.Y2 = y2;
            canvas.Children.Add(line);

            // add joint
            Ellipse jointPot = new Ellipse();
            jointPot.Fill = BRUSH_JOINT;
            jointPot.Width = DOUBLE_RADIUS_JOINT;
            jointPot.Height = DOUBLE_RADIUS_JOINT;
            jointPot.Margin = new Thickness(x1 - DOUBLE_RADIUS_JOINT / 2, y1 - DOUBLE_RADIUS_JOINT / 2, 0, 0);
            canvas.Children.Add(jointPot);
        }

        /// <summary>
        /// Add all bones to screen
        /// </summary>
        /// <param name="skeleton"></param>
        private void addBones(Body skeleton)
        {
            // add torso
            addBone(skeleton, JointType.SpineBase, JointType.HipLeft);
            addBone(skeleton, JointType.SpineBase, JointType.HipRight);
            addBone(skeleton, JointType.SpineBase, JointType.SpineMid);
            addBone(skeleton, JointType.SpineMid, JointType.SpineShoulder);
            addBone(skeleton, JointType.SpineShoulder, JointType.Neck);
            addBone(skeleton, JointType.SpineShoulder, JointType.ShoulderLeft);
            addBone(skeleton, JointType.SpineShoulder, JointType.ShoulderRight);
            addBone(skeleton, JointType.Neck, JointType.Head);

            // add left arm
            addBone(skeleton, JointType.ShoulderLeft, JointType.ElbowLeft);
            addBone(skeleton, JointType.ElbowLeft, JointType.WristLeft);
            addBone(skeleton, JointType.WristLeft, JointType.HandLeft);
            addBone(skeleton, JointType.WristLeft, JointType.ThumbLeft);
            addBone(skeleton, JointType.HandLeft, JointType.HandTipLeft);

            // add right arm
            addBone(skeleton, JointType.ShoulderRight, JointType.ElbowRight);
            addBone(skeleton, JointType.ElbowRight, JointType.WristRight);
            addBone(skeleton, JointType.WristRight, JointType.HandRight);
            addBone(skeleton, JointType.WristRight, JointType.ThumbRight);
            addBone(skeleton, JointType.HandRight, JointType.HandTipRight);

            // add left leg
            addBone(skeleton, JointType.HipLeft, JointType.KneeLeft);
            addBone(skeleton, JointType.KneeLeft, JointType.AnkleLeft);
            addBone(skeleton, JointType.AnkleLeft, JointType.FootLeft);

            // add right leg
            addBone(skeleton, JointType.HipRight, JointType.KneeRight);
            addBone(skeleton, JointType.KneeRight, JointType.AnkleRight);
            addBone(skeleton, JointType.AnkleRight, JointType.FootRight);
        }

        private void addBVHBones_Debug()
        {
            updateDebugBone(_kinectSkeleton.m_spineBase);
            foreach (JointNode node in _kinectSkeleton.JointNodes.Values)
            {
                if (node.Parent == null) continue;
                AddBVHBone_Debug(node.Parent, node);
            }
        }

        private void AddBVHBone_Debug(JointNode node1, JointNode node2)
        {
            addLineToWindow(node1.Offset.X, node1.Offset.Y,
                node2.Offset.X, node2.Offset.Y,
                BTUSH_BONE_DEBUG, DOUBLE_THICKNESS_BONE_DEBUG);
        }

        private void updateDebugBone(JointNode node)
        {
            if (node.Type == NodeTypeEnum.ROOT)
            {
                node.Rotation = new Vector3D(0, 0, 180);
                node.Offset.X += node.OriginalOffset.X + canvas.Width / 2;
                node.Offset.Y += node.OriginalOffset.Y + canvas.Height / 2;
                node.Offset.Z += node.OriginalOffset.Z;
            }
            else
            {
                Quaternion parentRotation = MathHelper.ConvertToQuaternion(node.Parent.Rotation);
                MathHelper.RotatePoint(node.OriginalOffset, parentRotation, out node.Offset);
                node.Offset.X += node.Parent.Offset.X;
                node.Offset.Y += node.Parent.Offset.Y;
                node.Offset.Z += node.Parent.Offset.Z;
                node.Rotation = MathHelper.QuaternionToAxisAngles(parentRotation * MathHelper.ConvertToQuaternion(node.Rotation));
            }

            if (node.Type == NodeTypeEnum.END)
                return;
            foreach (JointNode child in node.Children)
            {
                updateDebugBone(child);
            }
        }

        private void button_Click(object sender, RoutedEventArgs e)
        {
            if (null != this.sensor)
            {
                this.sensor.Close();
            }

            bvhWriter.OutputBVHToFile();
        }

        private void button1_Click(object sender, RoutedEventArgs e)
        {
            underRecording = true;
        }
    }
}
