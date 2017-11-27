using System.IO;
using Kinect = Windows.Kinect;
using UnityEngine;

namespace SportsKinematics
{
    /// <summary>
    /// Used to generate a report for client viewing
    /// </summary>
    public static class Reporter
    {
        /// <summary>
        /// Creates the header line that dictates the format the CSV will take.
        /// </summary>
        /// <returns>The string containing the CSV header line.</returns>
        public static string ConfigCSVHeaders()
        {
            // CSV header info
            string body = "";
            body += "spatial_is_active,";
            for (Kinect.JointType jt = Kinect.JointType.SpineBase; jt <= Kinect.JointType.ThumbRight; jt++)
            {
                body += jt.ToString() + ",";
            }
            body = body.Remove(body.Length - 1, 1) + ",";

            body += "temporal_is_active,"
            + "occlusion_time,"
            + "temporal_on_hit_is_active,"
            + "opponent_right_handed,"
            + "user_right_handed,"
            + "ball_hit_time,"
            + "sport,"
            + "corrent_swing,"
            + "ball_to_paddle_distance,"
            + "ball_paddle_displacement";

            return body;
        }

        /// <summary>
        /// Creates part of a CSV entry based on a configuration.
        /// </summary>
        /// <param name="config">The configuration to be converted to a CSV entry</param>
        /// <returns>A string representing a CSV entry.</returns>
        public static string SceneToCSV(Configuration config)
        {
            // CSV data entries
            string body = "";
            bool[] m_occBoolArr = config.OcclusionArray;

            body += "\n" + config.SpatialIsActive.ToString() + ",";
            for (Kinect.JointType jt = Kinect.JointType.SpineBase; jt <= Kinect.JointType.ThumbRight; jt++)
            {
                body += m_occBoolArr[(int)jt] + ",";
            }
            body = body.Remove(body.Length - 1, 1) + ",";

            body += config.TemporalIsActive.ToString() + ","
            + config.OcclusionTime.ToString() + ","
            + config.TemporalOnHitIsActive.ToString() + ","
            + config.OpponentRightHanded.ToString() + ","
            + config.UserRightHanded.ToString() + ","
            + config.CollisionFrame.ToString() + ","
            + config.Sport.ToString();

            return body;
        }

        /// <summary>
        /// Converts all the information relating to a scenario in a playlist
        /// to a CSV entry.
        /// </summary>
        /// <param name="config">The Configuration to be converted to part of a CSV entry.</param>
        /// <param name="correctSwing">String value dictating if the player swung in the correct direction.</param>
        /// <param name="paddleBallDistance">String value representing the distance from the paddle to the ball.</param>
        /// <param name="paddleBallDisplacement">Vector representing the distance from the paddle to the ball.</param>
        /// <returns>String representing a CSV entry for a scenario.</returns>
        public static string SceneToCSV(Configuration config, string correctSwing, string paddleBallDistance, string paddleBallDisplacement)
        {
            // CSV data entries
            string body = "";
            bool[] m_occBoolArr = config.OcclusionArray;

            body += "\n" + config.SpatialIsActive.ToString() + ",";
            for (Kinect.JointType jt = Kinect.JointType.SpineBase; jt <= Kinect.JointType.ThumbRight; jt++)
            {
                body += m_occBoolArr[(int)jt] + ",";
            }
            body = body.Remove(body.Length - 1, 1) + ",";

            body += config.TemporalIsActive.ToString() + ","
            + config.OcclusionTime.ToString() + ","
            + config.TemporalOnHitIsActive.ToString() + ","
            + config.OpponentRightHanded.ToString() + ","
            + config.UserRightHanded.ToString() + ","
            + config.CollisionFrame.ToString() + ","
            + config.Sport.ToString() + ","
            + correctSwing + ","
            + paddleBallDistance + ","
            + paddleBallDisplacement;

            return body;
        }

        /// <summary>
        /// Static wrapper for the streamwriter class. Save the specified string to the specified file.
        /// </summary>
        /// <param name="reportPath"></param>
        /// <param name="body"></param>
        public static void Save(string reportPath, string body)
        {
            // Write the text asynchronously to a new file named "WriteTextAsync.txt".
            using (StreamWriter outputFile = new StreamWriter(reportPath))
            {
                outputFile.Write(body);
            }
        }
     }
}
