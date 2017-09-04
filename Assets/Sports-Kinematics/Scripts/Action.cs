using System;
using System.Collections.Generic;

namespace SportsKinematics
{
    /// <summary>
    /// Defines the data relating to an action.
    /// </summary>
    /// <author>James Howson</author>
    /// <date>13/04/2017</date>
    [Serializable]
    public struct ActionData//FR1 - Virtual opponent modelling from captured data. 
    {
        public List<Dictionary<Windows.Kinect.JointType, float[]>> m_positionData;
        public List<Dictionary<Windows.Kinect.JointType, float[]>> m_orientationData;
    }

    /// <summary>
    /// Defines an action and provides accessor and mutator functions for its data.
    /// </summary>
    /// <author>James Howson</author>
    /// <date>13/04/2017</date>
    [Serializable]
    public class Action//FR1 - Virtual opponent modelling from captured data. 
    {
        /// <summary>
        /// Name used to identify the action.
        /// </summary>
        private string m_actionName;
        /// <summary>
        /// Action data for the action.
        /// </summary>
        private ActionData m_currentAction;

        /// <summary>
        /// Default constructor.
        /// </summary>
        public Action()
        {
            m_actionName = null;
            m_currentAction.m_positionData = new List<Dictionary<Windows.Kinect.JointType, float[]>>();
            m_currentAction.m_orientationData = new List<Dictionary<Windows.Kinect.JointType, float[]>>();
        }

        /// <summary>
        /// Instance constructor to declare an action and all its action data.
        /// </summary>
        /// <param name="name">Name of the action.</param>
        /// <param name="positionData">Position data relating to the action being created.</param>
        /// <param name="orientationData">Orientation data relating to the action being created.</param>
        public Action(string name, List<Dictionary<Windows.Kinect.JointType, float[]>> positionData, List<Dictionary<Windows.Kinect.JointType, float[]>> orientationData)
        {
            m_actionName = name;
            m_currentAction.m_positionData = positionData;
            m_currentAction.m_orientationData = orientationData;
        }

        /// <summary>
        /// Gets or sets the Action Data struct of an action.
        /// </summary>
        public ActionData CurrentAction
        {
            get { return m_currentAction; }
            set { m_currentAction = value; }
        }

        /// <summary>
        /// Gets or sets the name of an aciton.
        /// </summary>
        public string Name
        {
            get { return m_actionName; }
            set { m_actionName = value; }
        }

        /// <summary>
        /// Gets or sets the position data of an action in the action data struct.
        /// </summary>
        public List<Dictionary<Windows.Kinect.JointType, float[]>> CurrentActionPositionData
        {
            get { return m_currentAction.m_positionData; }
            set { m_currentAction.m_positionData = value; }
        }

        /// <summary>
        /// Gets or sets the orientation data of an action in the action data struct.
        /// </summary>
        public List<Dictionary<Windows.Kinect.JointType, float[]>> CurrentActionOrientationData
        {
            get { return m_currentAction.m_orientationData; }
            set { m_currentAction.m_orientationData = value; }
        }

        /// <summary>
        /// Returns the number of entries in the position data list.
        /// </summary>
        public int Count
        {
            get { return m_currentAction.m_positionData.Count; }
        }
    }
}