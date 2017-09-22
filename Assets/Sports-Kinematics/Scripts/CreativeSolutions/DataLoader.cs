using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System.Globalization;

namespace SportsKinematics
{
    public class DataReader : MonoBehaviour
    {
        public SortedList<float, BallData> ReadBall(string fileName)
        {
            SortedList<float, BallData> actionBallData = new SortedList<float, BallData>();

            using (var reader = new StreamReader(fileName))
            {
                while (!reader.EndOfStream)
                {
                    var line = reader.ReadLine();
                    if (line != null && line != string.Empty)
                    {
                        var data = line.Split(',');

                        BallData tmpData = new BallData();
                        tmpData.position.x = float.Parse(data[0]);
                        tmpData.position.y = float.Parse(data[1], CultureInfo.InvariantCulture.NumberFormat);
                        tmpData.position.z = float.Parse(data[2], CultureInfo.InvariantCulture.NumberFormat);

                        float time = float.Parse(data[3], CultureInfo.InvariantCulture.NumberFormat);

                        actionBallData.Add(time, tmpData);
                    }
                }
            }

            return actionBallData;
        }

        public SortedList<float, StrikerData> ReadStriker(string fileName)
        {
            SortedList<float, StrikerData> actionStrikerData = new SortedList<float, StrikerData>();

            using (var reader = new StreamReader(fileName))
            {
                while (!reader.EndOfStream)
                {
                    var line = reader.ReadLine();
                    if (line != null && line != string.Empty)
                    {
                        var data = line.Split(',');

                        StrikerData tmpData = new StrikerData();
                        tmpData.position.x = float.Parse(data[0], CultureInfo.InvariantCulture.NumberFormat);
                        tmpData.position.y = float.Parse(data[1], CultureInfo.InvariantCulture.NumberFormat);
                        tmpData.position.z = float.Parse(data[2], CultureInfo.InvariantCulture.NumberFormat);

                        tmpData.rotation.x = float.Parse(data[3], CultureInfo.InvariantCulture.NumberFormat);
                        tmpData.rotation.y = float.Parse(data[4], CultureInfo.InvariantCulture.NumberFormat);
                        tmpData.rotation.z = float.Parse(data[5], CultureInfo.InvariantCulture.NumberFormat);

                        float time = float.Parse(data[6], CultureInfo.InvariantCulture.NumberFormat);

                        actionStrikerData.Add(time, tmpData);
                    }
                }
            }

            return actionStrikerData;
        }

        public SortedList<float, SkeletonData> ReadSkeleton(string fileName)
        {
            return (new SortedList<float, SkeletonData>());
        }

        public ConfigData ReadConfig(string fileName)
        {
            return (new ConfigData());
        }

        public FileNames ReadActionHeader(string fileName)
        {
            FileNames headerData = new FileNames();

            using (var reader = new StreamReader(fileName))
            {
                var line = reader.ReadLine();
                var data = line.Split(',');

                headerData.actionName = data[0];
                headerData.ballDataFileName = data[1];
                headerData.strikerDataFileName = data[2];
                headerData.skeletonDataFileName = data[3];
                headerData.configDataFileName = data[4];
            }

            return headerData;
        }

        public SortedList<int, string> ReadPlaylist(string fileName)
        {
            SortedList<int, string> playlist = new SortedList<int, string>();
            int i = 0;
            using (var reader = new StreamReader(fileName))
            {
                while (!reader.EndOfStream)
                {
                    var line = reader.ReadLine();
                    var data = line.Split(',');

                    string headerPath = data[0];
                    playlist.Add(i, headerPath);
                    i++;
                }
            }

            return playlist;
        }
    }
}
