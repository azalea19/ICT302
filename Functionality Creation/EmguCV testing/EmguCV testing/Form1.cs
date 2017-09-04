using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Emgu.CV;
using Emgu.CV.UI;
using Emgu.CV.Structure;
using System.Drawing;
using System.Windows.Forms;

namespace EmguCV_testing
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            ImageViewer viewer = new ImageViewer(); //create an image viewer
            //KinectCapture capture = new KinectCapture(KinectCapture.DeviceType.Kinect, KinectCapture.ImageGeneratorOutputMode.Vga30Hz); //create a camera captue
            VideoCapture capture = null;
            capture = new VideoCapture();
            Application.Idle += new EventHandler(delegate(object sender, EventArgs e)
            {  //run this until application closed (close button click on image viewer)
               //capture.Retrieve(viewer.Image); //draw the image obtained from camera
            });
            viewer.ShowDialog(); //show the image viewer
        }
    }
}
