using UnityEngine;
using UnityEngine.UI;

namespace SportsKinematics.UI
{
    /// <summary>
    /// Changes speed at which actions are rendered
    /// </summary>
    public class SpeedInputScripts : MonoBehaviour//FR6 - UI environment for client/test administrator.
    {
        /// <summary>
        /// Striker renderer reference to determine speed at which to render
        /// </summary>
        private StrikerRenderer m_strikerRender = null;
        /// <summary>
        /// Action renderer reference to determine speed at which to render
        /// </summary>
        private ActionRenderer m_actionRender = null;

        /// <summary>
        /// GameObject to set action renderer, set within Unity.
        /// </summary>
        public GameObject m_actionRenderManager = null;
        /// <summary>
        /// GameObject to set striker renderer, set within Unity.
        /// </summary>
        public GameObject m_strikerRenderManager = null;
        /// <summary>
        /// text variable displaying speed at which the rendering is occuring
        /// </summary>
        public InputField m_text;

        /// <summary>
        /// Initilizes action renderer and striker renderer references from GameObject.
        /// Also sets speed to current speed variables from Unity.
        /// </summary>
        void Start()//FR6 - UI environment for client/test administrator.
        {
            if (m_actionRenderManager)
            {
                m_actionRender = m_actionRenderManager.GetComponent<ActionRenderer>();
            }

            if (m_strikerRenderManager)
            {
                m_strikerRender = m_strikerRenderManager.GetComponent<StrikerRenderer>();
            }

            TaskOnPlayerValueChanged();
            TaskOnStrikerValueChanged();
        }

        /// <summary>
        /// Callback to set striker renderer speed
        /// </summary>
        public void TaskOnStrikerValueChanged()//FR6 - UI environment for client/test administrator.
        {
            if (m_text)
            {
                if (m_strikerRender && m_text.text != "")
                    float.TryParse(m_text.text, out m_strikerRender.m_speed);
            }
        }

        /// <summary>
        /// Callback to set action renderer speed.
        /// Also validates input variables are above 0 and bellow 100
        /// As well as being valid integer values.
        /// </summary>
        public void TaskOnPlayerValueChanged()//FR6 - UI environment for client/test administrator.
        {
            if (m_text)
            {
                if (m_actionRender && m_text.text != "")
                {
                    float playerSpeedChange;
                    float.TryParse(m_text.text, out playerSpeedChange);
                    if(playerSpeedChange <= m_actionRender.MaxSpeed)
                    {
                        if (playerSpeedChange > 0.0f)
                        {
                            m_actionRender.m_speed = playerSpeedChange;
                        }
                        else
                        {
                            //clamp to 0
                            m_actionRender.m_speed = 0.0f;
                            m_text.text = "0";
                        }
                    }
                    else
                    {
                        //clamp to m_actionRender.MaxSpeed
                        m_actionRender.m_speed = m_actionRender.MaxSpeed;
                        m_text.text = m_actionRender.MaxSpeed.ToString();
                    }
                }
            }
        }
    }
}