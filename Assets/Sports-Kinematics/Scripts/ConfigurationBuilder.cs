using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

namespace SportsKinematics
{
    /// <summary>
    /// Used to build a configuration object based off the input
    /// in the configuration canvas. Also provides the functionality to serialise the
    /// configuration.
    /// </summary>
    public class ConfigurationBuilder : MonoBehaviour
    {
        /// <summary>
        /// Toggle Group for the User Prefered hand toggle game objects.
        /// </summary>
        public ToggleGroup m_userToggleGroup;
        /// <summary>
        /// Toggle Group for the Opponent Prefered hand toggle game objects.
        /// </summary>
        public ToggleGroup m_opponentToggleGroup;
        /// <summary>
        /// Toggle Game Object that specifies if the user is right handed.
        /// </summary>
        public Toggle m_userRightHandedToggle;
        /// <summary>
        /// Toggle Game Object that specifies if the user is left handed.
        /// </summary>
        public Toggle m_userLeftHandedToggle;
        /// <summary>
        /// Toggle Game Object that specifies if the opponent is right handed.
        /// </summary>
        public Toggle m_opponentRightHandedToggle;
        /// <summary>
        /// Toggle Game Object that specifies if the opponent is left handed.
        /// </summary>
        public Toggle m_opponentLeftHandToggle;
        /// <summary>
        /// Dropdown gameobject used to set the sports type for the scenario being configured.
        /// </summary>
        public Dropdown m_sportsTypeDropdown;
        /// <summary>
        /// Inputfield to set the frame the paddle collides with the ball.
        /// </summary>
        public InputField m_paddleCollisionFrameInput;
        /// <summary>
        /// Toggle gameobject used to set if temporal occlusion is active in the current scenario.
        /// </summary>
        public Toggle m_temporalIsActive;
        /// <summary>
        /// Toggle gameobject used to set if temporal occlusion occurs when the paddle hits the ball in the current scenario.
        /// </summary>
        public Toggle m_onHitToggle;
        /// <summary>
        /// InputField gameobject used to set the amount of time into the scenario after which temporal occlusion will occur.
        /// </summary>
        public InputField m_occlusionTimeInputField;
        /// <summary>
        /// Toggle gameobject used to set if spatial occlusion is active in the current scenario.
        /// </summary>
        public Toggle m_spatialIsActive;
        /// <summary>
        /// The gameobject used to toggle occlusion for specific joints on the opponent in the current scenario.
        /// </summary>
        public GameObject m_body;

        /// <summary>
        /// Used to manage the m_body gameobject. Speficically it lets you check and set the occlusion of joints.
        /// </summary>
        private UI.BodyOcclusionTracker m_bodyTracker;
        /// <summary>
        /// The configuration settings for the current scenario being configured.
        /// </summary>
        private Configuration m_newConfiguration;

        /// <summary>
        /// Awake used to initialize the coniguration and the BodyOcclusionTracker objects.
        /// </summary>
        void Awake()
        {
            m_newConfiguration = new Configuration();
            m_bodyTracker = m_body.GetComponent<UI.BodyOcclusionTracker>();
        }

        /// <summary>
        /// Saves the configuration canvas to a configuration object, and serialises it.
        /// </summary>
        public void SaveConfiguration()
        {
            SaveSportsSettings();
            SaveOcclusionConfiguration();
            string name = ConstructUniqueConfigurationName();
            Serial<Configuration>.Save(m_newConfiguration, name, PlayerPrefs.GetString("CurrentUserDataPath") + "/Configurations/" + "/newConfigurations/");
            PlayerPrefs.SetString("LatestConfigurationFileName", name);
            if (PlayerPrefs.GetInt("RefreshScrollView") == 1)
            {
                PlayerPrefs.SetInt("Configured", 1);
                PlayerPrefs.SetInt("RefreshScrollView", 0);
                File.Delete(PlayerPrefs.GetString("CurrentUserDataPath") + "/Configurations/" + "/newConfigurations/" + PlayerPrefs.GetString("ConfigBeingModified"));
            }
        }

        /// <summary>
        /// Sets the sports settings in the configuration canvas to the current configuration being built.
        /// Called while saving the configuration.
        /// </summary>
        private void SaveSportsSettings()
        {
            m_newConfiguration.UserRightHanded = m_userRightHandedToggle.isOn;
            m_newConfiguration.OpponentRightHanded = m_opponentRightHandedToggle.isOn;
            int colFrame;
            int.TryParse(m_paddleCollisionFrameInput.text, out colFrame);
            m_newConfiguration.CollisionFrame = colFrame;
        }

        /// <summary>
        /// Sets the occlusion settings in the configuration canvas to the current configuration being built.
        /// Called while saving the configuration.
        /// </summary>
        private void SaveOcclusionConfiguration()
        {
            //Saving temporal occlusion settings.
            if (m_temporalIsActive.isOn)
            {
                m_newConfiguration.TemporalIsActive = m_temporalIsActive.isOn;
                m_newConfiguration.TemporalOnHitIsActive = m_onHitToggle.isOn;

                float time;
                float.TryParse(m_occlusionTimeInputField.text, out time);
                m_newConfiguration.OcclusionTime = time/1000f; //convert to ms
            }
            
            //Saving spatial occlusion settings.
            if (m_spatialIsActive.isOn)
            {
                m_newConfiguration.SpatialIsActive = m_spatialIsActive.isOn;
                 m_newConfiguration.OcclusionArray = m_body.GetComponent<UI.BodyOcclusionTracker>().Occlusions;
            }
            else
            {
                bool[] bodyBoolArr = new bool[25];
                for (int i = 0; i < 25; i++)
                {
                    bodyBoolArr[i] = true;
                }

                m_newConfiguration.OcclusionArray = bodyBoolArr;
            }
        }

        /// <summary>
        /// Saves the sport based off the sport chosen in the configuration canvas drop down menu.
        /// </summary>
        private void SaveSportsType()
        {
            switch (m_sportsTypeDropdown.value)
            {
                case 0:
                    m_newConfiguration.Sport = StrikerRenderer.StrikerSelection.Catch;
                    break;
                case 1:
                    m_newConfiguration.Sport = StrikerRenderer.StrikerSelection.TableTennis;
                    break;
                default:
                    m_newConfiguration.Sport = StrikerRenderer.StrikerSelection.TableTennis;
                    break;
            }
        }

        /// <summary>
        /// Creates a unique configuration name based off the current time.
        /// </summary>
        /// <returns>The unique configuration filename.</returns>
        private string ConstructUniqueConfigurationName()
        {
            return System.DateTime.Now.ToString("yyyy-MMM-dd HH-mm-ss") + PlayerPrefs.GetString("ConfigurationExtension");
        }

        /// <summary>
        /// Restores the configuration canvas game object states based off a configuration object.
        /// </summary>
        /// <param name="config">The configuration object used to restore the configuration canvas game object states.</param>
        public void RestoreSettings(Configuration config)
        {
            m_userRightHandedToggle.isOn = config.UserRightHanded;
            m_userLeftHandedToggle.isOn = !config.UserRightHanded;
            m_opponentRightHandedToggle.isOn = config.OpponentRightHanded;
            m_opponentLeftHandToggle.isOn = !config.OpponentRightHanded;

            m_sportsTypeDropdown.value = (int)config.Sport;
            m_temporalIsActive.isOn = config.TemporalIsActive;
            m_onHitToggle.isOn = config.TemporalOnHitIsActive;
            m_occlusionTimeInputField.text = (config.OcclusionTime*1000f).ToString();
            m_paddleCollisionFrameInput.text = (config.CollisionFrame*1000f).ToString();
            m_spatialIsActive.isOn = config.SpatialIsActive;
            m_paddleCollisionFrameInput.text = config.CollisionFrame.ToString();
            //if (PlayerPrefs.GetInt("ReloadBody") != 0)
            //    m_bodyTracker.Occlusions = config.OcclusionArray;
        }

        /// <summary>
        /// Resets the configuration game objects to their default states.
        /// </summary>
        public void ResetCanvas()
        {
            m_userToggleGroup.SetAllTogglesOff();
            m_opponentToggleGroup.SetAllTogglesOff();
            m_sportsTypeDropdown.value = 0;
            m_temporalIsActive.isOn = false;
            m_onHitToggle.isOn = false;
            m_occlusionTimeInputField.text = "";
            m_paddleCollisionFrameInput.text = "";
            m_spatialIsActive.isOn = false;
            //if (PlayerPrefs.GetInt("ReloadBody") != 0)
            //    ResetBody();
            m_newConfiguration = new Configuration();
        }

        /// <summary>
        /// Sets the body, which is used to represent the joints to occlude, back to true. A value of true 
        /// means the joint is not occluded.
        /// </summary>
        private void ResetBody()
        {
            bool[] resetBody = new bool[25];
            for (int i = 0; i < resetBody.Length; i++)
            {
                resetBody[i] = true;
            }

            m_bodyTracker.Occlusions = resetBody;
        }
    }
}
