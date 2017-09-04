using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Loads Actions into the system.
/// </summary>
/// <author>James Howson</author>
/// <date>13/04/2017</date>
namespace SportsKinematics
{
    public class ActionLoader
    {
        /// <summary>
        /// Path to the position data file for the current action being loaded.
        /// </summary>
        private string m_positionFolderName;
        /// <summary>
        /// Path to the orientation data file for the current action being loaded.
        /// </summary>
        private string m_orientationFolderName;

        /// <summary>
        /// Instance constructor. Sets up the location of the position and orientation files of the action.
        /// being loaded.
        /// </summary>
        /// <param name="positionFolder">Path to the position file.</param>
        /// <param name="orientationFolder">Path to the orientation file.</param>
        public ActionLoader(string positionFolder, string orientationFolder)
        {
            m_positionFolderName = positionFolder;
            m_orientationFolderName = orientationFolder;
        }

        /// <summary>
        /// Used to validate fields when Load Data is pressed.
        /// <returns>true if all fields have values</returns>
        /// <author>James Howson</author>
        /// <date>16/04/2017</date>
        public bool ValidateFields()
        {
            return (
                (!string.IsNullOrEmpty(m_positionFolderName)) &&
                (!string.IsNullOrEmpty(m_orientationFolderName)) &&
                (!string.IsNullOrEmpty(PlayerPrefs.GetString("OrientationExtension"))) &&
                (!string.IsNullOrEmpty(PlayerPrefs.GetString("PositionExtension"))));
        }

        /// <summary>
        /// Returns the deserialised orientaiton and position data relating to an action.
        /// </summary>
        /// <param name="actionName">Name of the action to be loaded.</param>
        /// <returns>The loaded action.</returns>
        /// <author>James Howson</author>
        /// <date>10/04/2017</date>
        public Action LoadActionFromFile(string actionName)
        {
            try
            {
                List<Dictionary<Windows.Kinect.JointType, float[]>> positionData = DeserializeData(m_positionFolderName, actionName, PlayerPrefs.GetString("PositionExtension"));
                List<Dictionary<Windows.Kinect.JointType, float[]>> orientationData = DeserializeData(m_orientationFolderName, actionName, PlayerPrefs.GetString("OrientationExtension"));

                return new Action(actionName, positionData, orientationData);
            }
            catch (System.IO.DirectoryNotFoundException)
            { 
                Debug.Log("Directory not found: " + m_positionFolderName);
                Debug.Log("Directory not found: " + m_orientationFolderName);
            }
            catch (System.IO.FileNotFoundException)
            {
                Debug.Log("File Not Found: " + actionName);
            }

            return null;
        }  

        /// <summary>
        /// Deserializes and returns the orientation and position data with the specified details.
        /// </summary>
        /// <param name="dataPath">Path of the serialised data.</param>
        /// <param name="fileName">File name of the serialised data.</param>
        /// <param name="dataType">File extension of the serialised data.</param>
        /// <returns></returns>
        /// <author>James Howson</author>
        /// <date>10/04/2017</date>
        private List<Dictionary<Windows.Kinect.JointType, float[]>> DeserializeData(string dataPath, string fileName, string dataType)
        {
            return Serial<List<Dictionary<Windows.Kinect.JointType, float[]>>>.Load(fileName + dataType, dataPath);
        }
    }
}