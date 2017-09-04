using UnityEngine;

namespace SportsKinematics
{
    /// <summary>
    /// Start EDiting scene
    /// </summary>
    public class EditingManager : MonoBehaviour
    {
        /// <summary>
        /// Action renderer Script reference
        /// </summary>
        private ActionRenderer m_actionRender;

        /// <summary>
        /// ActionLoader Script reference
        /// </summary>
        private ActionLoader m_actionLoader;

        /// <summary>
        /// action data to load
        /// </summary>
        private string[] m_actiondata;

        /// <summary>
        /// action renderer GameObject from scene
        /// </summary>
        public GameObject m_actionRenderer;

        // Use this for initialization
        /// <summary>
        /// Start function for GameObject. Initialise data.
        /// </summary>
        void Start()
        {
            m_actiondata = PlayerPrefs.GetString("LoadedActionData").Split(',');
            m_actionLoader = new ActionLoader(Application.dataPath + m_actiondata[1], Application.dataPath + m_actiondata[2]);

            if (m_actionRenderer)
            {
                m_actionRender = m_actionRenderer.GetComponent<ActionRenderer>();
                Action temp = m_actionLoader.LoadActionFromFile(m_actiondata[0]);

                m_actionRender.InitiateRenderBody(temp);
            }
        }
    }
}