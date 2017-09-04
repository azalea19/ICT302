/*using System;
using Emgu.CV;
using Emgu.CV.Structure;
using System.Drawing;

namespace SportsKinematics
{
    public class ColourTracker
    {
        /// <summary>
        /// Colour to track, when using FindColour(8 params)
        /// </summary>
        public Bgr m_colourTrackRGB;

        /// <summary>
        /// Sets the range of colours to be used while searching
        /// </summary>
        /// <param name="colourField">a Red, Blue or Green value, which we are looking for.</param>
        /// <param name="accuracy">Allowable range of deviation from the aforementioned value.</param>
        /// <returns>double[2] with lowest ([0]) and highest ([1]) colour for the specified field (R/G/B)</returns>
        /// <author>Aiden Triffitt</author>
        /// <date>24/04/2017</date>
        private double[] SetColours(double colourField, int accuracy)
        {

            double[] colours = new double[2];

            colours[0] = colourField - accuracy;

            // Cannot be lower than 0
            if (colours[0] < 0)
                colours[0] = 0;

            colours[1] = colourField + accuracy;

            // Cannot be higher than 255
            if (colours[1] > 255)
                colours[1] = 255;

            return colours;
        }

        /// <summary>
        /// Sets the range of pixels to be searched. range[] returned references x OR y value, with [0] being minimum and [1] being maximum. range[] is always 2 members.
        /// </summary>
        /// <param name="locationStart">x OR y value from which we originate</param>
        /// <param name="border">Distance in positive AND negative from origin</param>
        /// <param name="max">Highest allowable index</param>
        /// <returns>int[2] with lowest([0]) and highest([1]) index to search</returns>
        /// <author>Aiden Triffitt</author>
        /// <date>26/04/2017</date>
        private int[] SetBorders(int locationStart, int border, int max)
        {
            //prevent searching values outside image

            int[] range = new int[2];

            range[0] = locationStart - border;
            range[1] = locationStart + border;

            //Cannot search indexes lower than 0
            if (range[0] < 0)
                range[0] = 0;

            //Cannot search higher than the maximum index
            if (range[1] > max)
                range[1] = max;

            return range;

        }

        /// <summary>
        /// Determines if a colour exists between a range of values
        /// </summary>
        /// <param name="imgColour">Colour to test</param>
        /// <param name="range">Range to test against</param>
        /// <returns>bool, true if colour is within range</returns>
        /// <author>Aiden Triffitt</author>
        /// <date>27/04/2017</date>
        private bool InColourRange(double imgColour, double[] range)
        {
            return imgColour >= range[0] && imgColour <= range[1];
        }

        /// <summary>
        /// Performs an incremental search, returning first found instance of colour in a range.
        /// </summary>
        /// <param name="imageFrame">Image to search</param>
        /// <param name="blue">Range of blue values to search</param>
        /// <param name="green">Range of green values to search</param>
        /// <param name="red">Range of red values to search</param>
        /// <param name="iRange">Range of i indexes to search</param>
        /// <param name="jRange">Range of i indexes to search</param>
        /// <returns>First retrieved point</returns>
        /// <author>Aiden Triffitt</author>
        /// <date>27/04/2017</date>
        private Point GetPointA(Image<Bgr, byte> imageFrame, double[] blue, double[] green, double[] red, int[] iRange, int[] jRange)
        {
            Point a = new Point(-1, 0);

            for (int i = iRange[0]; i < iRange[1]; ++i)
            {
                for (int j = jRange[0]; j < jRange[1]; ++j)
                {
                    if (InColourRange(imageFrame[i, j].Blue, blue) &&
                        InColourRange(imageFrame[i, j].Green, green) &&
                        InColourRange(imageFrame[i, j].Red, red))
                    {
                        a.X = i;
                        a.Y = j;

                        return a;
                    }
                }
            }

            return a;
        }
        /// <summary>
        /// Performs an decremental search, returning first found instance of colour in a range.
        /// </summary>
        /// <param name="imageFrame">Image to search</param>
        /// <param name="blue">Range of blue values to search</param>
        /// <param name="green">Range of green values to search</param>
        /// <param name="red">Range of red values to search</param>
        /// <param name="iRange">Range of i indexes to search</param>
        /// <param name="jRange">Range of i indexes to search</param>
        /// <param name="a">Point at which the first retrieved colour occured</param>
        /// <returns>First retrieved point</returns>        
        /// <author>Aiden Triffitt</author>
        /// <date>27/04/2017</date>
        private Point GetPointB(Image<Bgr, byte> imageFrame, double[] blue, double[] green, double[] red, int[] iRange, int[] jRange, Point a)
        {
            Point b = new Point(-1, 0);
    
            for (int i = iRange[1]; i >= a.X; --i)
            {
                for (int j = jRange[1]; j >= a.Y; --j)
                {
                    if (InColourRange(imageFrame[i, j].Blue, blue) &&
                        InColourRange(imageFrame[i, j].Green, green) &&
                        InColourRange(imageFrame[i, j].Red, red))
                    {
                        b.X = i;
                        b.Y = j;
       
                        return b;
                    }
                }
            }

            return b;
        }

        /// <summary>
        /// Calls the functions to search ranges of pixels, which return points.
        /// Returns the average of both points.
        /// </summary>
        /// <param name="imageFrame">image to search</param>
        /// <param name="blue">Blue colour range to search</param>
        /// <param name="green">Green colour range to search</param>
        /// <param name="red">Red colour range to search</param>
        /// <param name="iRange">Range of "i" indexes to search (for width)</param>
        /// <param name="jRange">Range of "j" indexes to search (for height)</param>
        /// <returns>Average position of a colour (x and y coordinates) within a range</returns>
        /// <author>Aiden Triffitt</author>
        /// <date>26/04/2017</date>
        private Point Search(Image<Bgr, byte> imageFrame, double[] blue, double[] green, double[] red, int[] iRange, int[] jRange)
        {
            //Search area for colour
            Point a = new Point();
            a = GetPointA(imageFrame, blue, green, red, iRange, jRange);
            Point b = new Point();
            Point c = new Point(-1, 0);
            if (a.X != -1)
            {
                b = GetPointB(imageFrame, blue, green, red, iRange, jRange, a);

                if (b.X != -1)
                {
                    c.X = a.X + b.X / 2;
                    c.Y = a.Y + b.Y / 2;
                }
            }

            return c;
        }

        /// <summary>
        /// Search an RGB image for specific colour, with colour as public class variable
        /// </summary>
        /// <param name="imageFrame">Image to search</param>
        /// <param name="locationStart">Point to search near</param>
        /// <param name="accMarginBlue">Margin of accuracy for Blue testing</param>
        /// <param name="accMarginGreen">Margin of accuracy for Green testing</param>
        /// <param name="accMarginRed">Margin of accuracy for Red testing</param>
        /// <param name="wBorder">Distance from locationStartW to search, both neg and pos</param>
        /// <param name="hBorder">Distance from locationStartH to search, both neg and pos</param>
        /// <returns>Point, first found location</returns>
        /// <author>Aiden Triffitt</author>
        /// <date>18/04/2017</date>
        public Point FindColour(Image<Bgr, Byte> imageFrame, Point locationStart, int accMarginBlue, int accMarginGreen, int accMarginRed, int wBorder, int hBorder)
        {
            int[] iRange = SetBorders(locationStart.X, wBorder, imageFrame.Width);
            int[] jRange = SetBorders(locationStart.Y, hBorder, imageFrame.Height);

            double[] blue = SetColours(m_colourTrackRGB.Blue, accMarginBlue);
            double[] green = SetColours(m_colourTrackRGB.Green, accMarginGreen);
            double[] red = SetColours(m_colourTrackRGB.Red, accMarginRed);

            return Search(imageFrame, blue, green, red, iRange, jRange);
        }

        /// <summary>
        /// Search an RGB image for specific colour, with colour as parameter
        /// </summary>
        /// <param name="imageFrame">Image to search</param>
        /// <param name="searchColours">Colour to find</param>
        /// <param name="locationStart">Point to search near</param>
        /// <param name="accMarginBlue">Margin of accuracy for Blue testing</param>
        /// <param name="accMarginGreen">Margin of accuracy for Green testing</param>
        /// <param name="accMarginRed">Margin of accuracy for Red testing</param>
        /// <param name="wBorder">Distance from locationStartW to search, both neg and pos</param>
        /// <param name="hBorder">Distance from locationStartH to search, both neg and pos</param>
        /// <returns>Point, first found location</returns>
        /// <author>Aiden Triffitt</author>
        /// <date>18/04/2017</date>
        public Point FindColour(Image<Bgr, Byte> imageFrame, Bgr searchColours, Point locationStart, int accMarginBlue, int accMarginGreen, int accMarginRed, int wBorder, int hBorder)
        {
            int[] iRange = SetBorders(locationStart.X, wBorder, imageFrame.Width);
            int[] jRange = SetBorders(locationStart.Y, hBorder, imageFrame.Height);

            double[] blue = SetColours(searchColours.Blue, accMarginBlue);
            double[] green = SetColours(searchColours.Green, accMarginGreen);
            double[] red = SetColours(searchColours.Red, accMarginRed);

            return Search(imageFrame, blue, green, red, iRange, jRange);
        }
    }
}*/