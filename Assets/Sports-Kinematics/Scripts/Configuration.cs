using System;
using UnityEngine;

namespace SportsKinematics
{
    /// <summary>
    /// truct that defines Temporal Occlusion settings for a scenario.
    /// </summary>
    [Serializable]
    public struct TemporalOcclusion
    {
        public bool isActive;
        public bool onHit;
        public float occlusionTime;
    }

    /// <summary>
    /// truct that defines Spatial Occlusion settings for a scenario.
    /// </summary>
    [Serializable]
    public struct SpatialOcclusion
    {
        public bool isActive;
        public bool[] joints;
    }

    /// <summary>
    /// Struct that defines Sports settings for a scenario.
    /// </summary>
    [Serializable]
    public struct SportsSettings
    {
        public bool userRightHanded;
        public bool opponentRightHanded;
        public float[] startingRotation;
        public float[] endingRotation;
        public StrikerRenderer.StrikerSelection sport;
        public int paddleCollisionFrame;
    }

    /// <summary>
    /// Configution defines all the configution settings for a scenario. This includes the settings
    /// specific to TemporalOcclusion, SpatialOcclusion and SportsSettings.
    /// </summary>
    [Serializable]
    public class Configuration
    {
        /// <summary>
        /// TemporalOcclusion settings associated with this configuration.
        /// </summary>
        private TemporalOcclusion m_temporalOcclusionSettings;
        /// <summary>
        /// SpatialOcclusion settings associated with this configuration.
        /// </summary>
        private SpatialOcclusion m_spatialOcclusionSettings;
        /// <summary>
        /// Sports settings associated with this configuration.
        /// </summary>
        private SportsSettings m_sportsSettings;

        /// <summary>
        /// Default constructor.
        /// </summary>
        public Configuration()
        {
            m_temporalOcclusionSettings.onHit = false;
            m_temporalOcclusionSettings.occlusionTime = 0.0f;
            m_spatialOcclusionSettings.joints = new bool[25];

            for (int i = 0; i < 25; i++)
            {
                m_spatialOcclusionSettings.joints[i] = true;
            }

            m_sportsSettings.userRightHanded = true;
            m_sportsSettings.opponentRightHanded = true;
            m_sportsSettings.startingRotation = new float[4];
            m_sportsSettings.endingRotation = new float[4];
            m_sportsSettings.sport = StrikerRenderer.StrikerSelection.TableTennis;
            m_sportsSettings.paddleCollisionFrame = 0;
        }

        /// <summary>
        /// Instance constructor.
        /// </summary>
        /// <param name="temporalSettings">Used to construct the temporal occlusion settings.</param>
        /// <param name="spatialSettings">Used to construct the spatial occlusion settings.</param>
        /// <param name="sportsSettings">Used to construct the sports settings.</param>
        public Configuration(TemporalOcclusion temporalSettings, SpatialOcclusion spatialSettings, SportsSettings sportsSettings)
        {
            m_temporalOcclusionSettings = temporalSettings;
            m_spatialOcclusionSettings = spatialSettings;
            m_sportsSettings = sportsSettings;
        }


        /// <summary>
        /// Gets or sets the Temporal Occlusion Settings of a configuration.
        /// </summary>
        public TemporalOcclusion TemporalOcclusionSettings
        {
            get { return m_temporalOcclusionSettings; }
            set { m_temporalOcclusionSettings = value; }
        }

        /// <summary>
        /// Gets or sets whether Temporal Occlusion is Active in a configuration.
        /// </summary>
        public bool TemporalIsActive
        {
            get { return m_temporalOcclusionSettings.isActive; }
            set { m_temporalOcclusionSettings.isActive = value; }
        }

        /// <summary>
        /// Gets or sets whether Temporal Occlusion on Hit is Active in a configuration.
        /// </summary>
        public bool TemporalOnHitIsActive
        {
            get { return m_temporalOcclusionSettings.onHit; }
            set { m_temporalOcclusionSettings.onHit = value; }
        }

        /// <summary>
        /// Gets or sets the ammount of time, after which occlusion will occur.
        /// </summary>
        public float OcclusionTime
        {
            get { return m_temporalOcclusionSettings.occlusionTime; }
            set { m_temporalOcclusionSettings.occlusionTime = value; }
        }

        /// <summary>
        /// Gets or sets whether spatial occlsion is active in a configuration.
        /// </summary>
        public bool SpatialIsActive
        {
            get { return m_spatialOcclusionSettings.isActive; }
            set { m_spatialOcclusionSettings.isActive = value; }
        }

        /// <summary>
        /// Gets or sets the spatial occlusion settings for in a configuration.
        /// </summary>
        public SpatialOcclusion SpatialOcclusionSettings
        {
            get { return m_spatialOcclusionSettings; }
            set { m_spatialOcclusionSettings = value; }
        }

        /// <summary>
        /// Gets or sets the Sports Settings for in a configuration.
        /// </summary>
        public SportsSettings SportsSettings
        {
            get { return m_sportsSettings; }
            set { m_sportsSettings = value; }
        }

        /// <summary>
        /// Gets or sets an array of booleans representing whether a joint in the body is occluded.
        /// </summary>
        public bool[] OcclusionArray
        {
            get { return m_spatialOcclusionSettings.joints; }
            set { m_spatialOcclusionSettings.joints = value; }
        }

        /// <summary>
        /// Gets or sets whether the user is right handed.
        /// </summary>
        public bool UserRightHanded
        {
            get { return m_sportsSettings.userRightHanded; }
            set { m_sportsSettings.userRightHanded = value; }
        }

        /// <summary>
        /// Gets or sets whether the opponent is right handed.
        /// </summary>
        public bool OpponentRightHanded
        {
            get { return m_sportsSettings.opponentRightHanded; }
            set { m_sportsSettings.opponentRightHanded = value; }
        }

        /// <summary>
        /// Gets or sets the frame the ball collides with the paddle.
        /// </summary>
        public int CollisionFrame
        {
            get { return m_sportsSettings.paddleCollisionFrame; }
            set { m_sportsSettings.paddleCollisionFrame = value; }
        }

        /// <summary>
        /// Gets or sets the sport relating to a configuration.
        /// </summary>
        public StrikerRenderer.StrikerSelection Sport
        {
            get { return m_sportsSettings.sport; }
            set { m_sportsSettings.sport = value; }
        }
    }   
}

