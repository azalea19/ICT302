using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace SportsKinematics.UI
{
    /// <summary>
    /// Toggle joint in Configuration Canvas in MainMenu
    /// </summary>
    public class JointToggle : MonoBehaviour
    {
        /// <summary>
        /// Determine if joint is currently on
        /// </summary>
        private bool m_isOn;

        /// <summary>
        /// mouseover text foreground
        /// </summary>
        private GUIStyle m_GUIStyleFore;

        /// <summary>
        /// mouseover text background
        /// </summary>
        private GUIStyle m_GUIStyleBack;

        /// <summary>
        /// mouse over text text
        /// </summary>
        private string m_currentText;

        /// <summary>
        /// joint not-occluded material
        /// </summary>
        public Material m_defaultMat;

        /// <summary>
        /// Joint occluded material
        /// </summary>
        public Material m_occludedMat;

        /// <summary>
        /// name of joint
        /// </summary>
        public string m_name;

        /// <summary>
        /// Start Gameobject. Init all values
        /// </summary>
        void Start()
        {
            m_isOn = true;

            m_GUIStyleFore = new GUIStyle();
            m_GUIStyleFore.normal.textColor = Color.magenta;
            m_GUIStyleFore.alignment = TextAnchor.UpperCenter;
            m_GUIStyleFore.wordWrap = true;
            m_GUIStyleFore.fontSize = 16;

            m_GUIStyleBack = new GUIStyle();
            m_GUIStyleBack.normal.textColor = Color.black;
            m_GUIStyleBack.alignment = TextAnchor.UpperCenter;
            m_GUIStyleBack.wordWrap = true;
            m_GUIStyleBack.fontSize = 16;
        }

        /// <summary>
        /// Accessor method for joint being on
        /// </summary>
        public bool isOn
        {
            get { return m_isOn; }
            set {

                m_isOn = value;

                if (m_isOn)
                    GetComponent<MeshRenderer>().material = m_defaultMat;
                else
                    GetComponent<MeshRenderer>().material = m_occludedMat;
            }
        }

        /// <summary>
        /// Mouse Over event handler
        /// </summary>
        public void OnMouseOver()
        {
            m_currentText = m_name;

            //display banner/tool-tip

            if (Input.GetMouseButtonUp(0))
            {
                m_isOn = !m_isOn;

                if (m_isOn)
                {
                    GetComponent<MeshRenderer>().material = m_defaultMat;
                }
                else
                {
                    GetComponent<MeshRenderer>().material = m_occludedMat;
                }
            }
        }

        /// <summary>
        /// On GUI event handler
        /// </summary>
        public void OnGUI()
        {
            if (m_currentText != "")
            {
                var x = Event.current.mousePosition.x;
                var y = Event.current.mousePosition.y;
                GUI.Label(new Rect(x - 149, y + 21, 300, 60), m_currentText, m_GUIStyleBack);
                GUI.Label(new Rect(x - 150, y + 20, 300, 60), m_currentText, m_GUIStyleFore);
            }
        }

        /// <summary>
        /// Mouse Exit event handler
        /// </summary>
        public void OnMouseExit()
        {
            m_currentText = "";
        }
    }
}