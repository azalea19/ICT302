using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Emgu.CV;
using Emgu.CV.UI;
using Emgu.CV.Structure;
using Emgu.CV.CvEnum;
using Emgu.Util;
using Emgu.CV.ML;
using System.Drawing;
using System.Windows.Forms;
using Microsoft.Kinect;
using System.Drawing.Imaging;
using System.Runtime.InteropServices;

namespace EmguCV_testing
{
    static class Program
    {
        static ColorFrameReader colourReader;
        static FrameDescription colourFrameDescription;
        static KinectSensor sensor;
        static ImageViewer viewer1;

        /// <summary>
        /// Search an image for a specified colour, and draw a rectangle around that colour.
        /// </summary>
        /// <param name="imageFrame">Image to search</param>
        /// <param name="sampleColour">Hsv Colour to find in image</param>
        /// <param name="locationStartW">Start point for search, width</param>
        /// <param name="locationStartL">Start point for search, length</param>
        /// <param name="accMarginHue">accuracy margin for testing Hue</param>
        /// <param name="accMarginSat">accuracy margin for testing Saturation</param>
        /// <param name="accMarginVal">accuracy margin for testing Value</param>
        /// <author>Aiden Triffitt</author>
        /// <date>15/04/2017</date>
        static void FindColour(Image<Bgr, Byte> imageFrame, Hsv sampleColour, int locationStartW, int locationStartH, int accMarginHue, int accMarginSat, int accMarginVal)
        {
            Image<Hsv, Byte> colDetect = imageFrame.Convert<Hsv, Byte>();
            List<Point> pixLocations = new List<Point>();

            int iMin = locationStartW - 200;
            int jMin = locationStartH - 200;
            int iMax = locationStartW + 200;
            int jMax = locationStartH + 200;

            double hueMin, hueMax;
            double satMin, satMax;
            double valMin, valMax;

            hueMin = sampleColour.Hue - accMarginHue;

            if (hueMin < 0)
                hueMin = 360 + hueMin;

            hueMax = sampleColour.Hue + accMarginHue;

            if (hueMax > 360)
                hueMax = (0 + hueMax) - 360;

            satMin = sampleColour.Satuation - accMarginSat;
            if (satMin < 0)
                satMin = 0;

            satMax = sampleColour.Satuation + accMarginSat;
            if (satMax > 255)
                satMax = 255;

            valMin = sampleColour.Value - accMarginVal;
            if (valMin < 0)
                valMin = 0;

            valMax = sampleColour.Value + accMarginVal;
            if (valMax > 255)
                valMax = 255;

            //prevent searching values outside image
            if (iMin < 0)
                iMin = 0;
            if (jMin < 0)
                jMin = 0;

            if (iMax > imageFrame.Width)
                iMax = imageFrame.Width;
            if (jMax > imageFrame.Height)
                jMax = imageFrame.Height;

            //Search area for colour
            for (int i = iMin; i < iMax; i++)
            {
                for (int j = jMin; j < jMax; j++)
                {
                    if (colDetect[i, j].Hue >= hueMin && colDetect[i, j].Hue <= hueMax
                        && colDetect[i, j].Satuation >= satMin && colDetect[i, j].Satuation <= satMax
                        && colDetect[i, j].Value >= valMin - accMarginVal && colDetect[i, j].Value <= valMax)
                    {

                        //change to add only a couple points
                        pixLocations.Add(new Point(i, j));
                    }
                    else
                        if (sampleColour.Value == 0) //Looking for Black
                        {
                            if (colDetect[i, j].Value >= sampleColour.Value - accMarginVal && colDetect[i, j].Value <= sampleColour.Value + accMarginVal)
                            {
                                pixLocations.Add(new Point(i, j));
                            }
                        }
                        else
                            if (sampleColour.Satuation == 0) //Looking for White
                            {
                                if (colDetect[i, j].Satuation >= sampleColour.Satuation - accMarginSat && colDetect[i, j].Satuation <= sampleColour.Satuation + accMarginSat)
                                {
                                    pixLocations.Add(new Point(i, j));
                                }
                            }
                            else //red
                                if((colDetect[i, j].Hue >= hueMin && hueMin > 300) ||  (colDetect[i, j].Hue <= hueMax && hueMax < 60)
                                   && colDetect[i, j].Satuation >= satMin && colDetect[i, j].Satuation <= satMax
                                   && colDetect[i, j].Value >= valMin - accMarginVal && colDetect[i, j].Value <= valMax)
                                {
                                    pixLocations.Add(new Point(i, j));
                                }
                }
            }

            Point[] corners = new Point[2];

            if (pixLocations.Count > 0)
            {
                corners[0] = pixLocations.ElementAt<Point>(0);
                corners[1] = pixLocations.Last<Point>();

                Size rectSize = new Size();

                rectSize.Width = corners[1].X - corners[0].X;
                rectSize.Height = corners[1].Y - corners[0].Y;

                colDetect.Draw(new Rectangle(corners[0], rectSize), new Hsv(0, 100, 100), 10);
            }
             
            viewer1.Image = colDetect;
        }

        /// <summary>
        /// Fires when a frame is captured. Initiates search for colours
        /// </summary>
        /// <param name="sender">object sending event</param>
        /// <param name="e">Event that is fired. Features all frame details</param>
        /// <author>Aiden Triffitt</author>
        /// <date>14/04/2017</date>
        static void colourReader_ColorFrameArrived(object sender, ColorFrameArrivedEventArgs e)
        {
            Image<Bgr, byte> imageFrame;

            using (ColorFrame frame = e.FrameReference.AcquireFrame())
            {
                if (frame == null)
                {
                    return;
                }

                int width = frame.FrameDescription.Width;
                int height = frame.FrameDescription.Height;
                byte[] data = new byte[width * height * Image.GetPixelFormatSize(PixelFormat.Format32bppRgb)/8];

                frame.CopyConvertedFrameDataToArray(data, ColorImageFormat.Bgra);

                Bitmap bitmap = new System.Drawing.Bitmap(width, height, PixelFormat.Format32bppRgb);

                BitmapData bitmapData = bitmap.LockBits(
                    new System.Drawing.Rectangle(0, 0, bitmap.Width, bitmap.Height),
                    ImageLockMode.WriteOnly,
                    bitmap.PixelFormat);

                Marshal.Copy(data, 0, bitmapData.Scan0, data.Length);
                bitmap.UnlockBits(bitmapData);

                imageFrame = new Image<Bgr, byte>(bitmap);

                FindColour(imageFrame, new Hsv(210, 73.15, 84.71), 500, 500, 40, 20, 20);
            }
        }

        static void sensor_IsAvailableChanged(object sender, IsAvailableChangedEventArgs e)
        {
           
        }

        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        /// <author>Aiden Triffitt</author>
        /// <date>14/04/2017</date>
        [STAThread]
        static void Main()
        {
            sensor = KinectSensor.GetDefault();

            if (sensor == null)
                return;

            colourFrameDescription = sensor.ColorFrameSource.FrameDescription;

            colourReader = sensor.ColorFrameSource.OpenReader();
            colourReader.ColorFrameSource.CreateFrameDescription(ColorImageFormat.Rgba);

            sensor.Open();
            colourReader.FrameArrived += colourReader_ColorFrameArrived;
            sensor.IsAvailableChanged += sensor_IsAvailableChanged;

            viewer1 = new ImageViewer(); //create an image viewer
            viewer1.ShowDialog(); //show the image viewer
        }
    }
}
