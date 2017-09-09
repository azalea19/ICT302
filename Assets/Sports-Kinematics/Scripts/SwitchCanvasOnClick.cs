using UnityEngine;
using UnityEngine.UI;

namespace SportsKinematics.UI
{
    /// <summary>
    /// Switches Unity canvas on click to hide background interfaces, eliminating multiple
    /// interactions with canvases.
    /// </summary>
    public class SwitchCanvasOnClick : MonoBehaviour//FR6 - UI environment for client/test administrator.
    {
        public Button m_btn;
        /// <summary>
        /// canvas that's currently enabled
        /// </summary>
        public Canvas m_canvasToActivate;
        /// <summary>
        /// canvas that's currently disabled
        /// </summary>
        public Canvas m_canvasToDeactivate;

        /// <summary>
        /// main menu GameObject, set in Unity.
        /// </summary>
        public GameObject m_mainMenu;
        /// <summary>
        /// user manager GameObject, set in Unity.
        /// </summary>
        public GameObject m_userManager;

        /// <summary>
        /// Switches one canvas to another.
        /// Disables the canvas and all children on the canvas within the deactivated canvas
        /// and enables the activated canvas and children.
        /// </summary>
        public void Switch()//FR6 - UI environment for client/test administrator.
        {
            //bool active = m_canvasToActivate.gameObject.activeInHierarchy;

            if (m_canvasToDeactivate)
            {
                m_canvasToActivate.gameObject.SetActive(true);

                for (int i = 0; i < m_canvasToActivate.transform.childCount; i++)
                {
                    m_canvasToActivate.transform.GetChild(i).gameObject.SetActive(true);

                    for (int j = 0; j < m_canvasToActivate.transform.GetChild(i).childCount; j++)
                    {
                        m_canvasToActivate.transform.GetChild(i).GetChild(j).gameObject.SetActive(true);
                    }
                }
            }

            if (m_canvasToDeactivate)
            {
                for (int i = 0; i < m_canvasToDeactivate.transform.childCount; i++)
                {
                    m_canvasToDeactivate.transform.GetChild(i).gameObject.SetActive(false);

                    for (int j = 0; j < m_canvasToDeactivate.transform.GetChild(i).childCount; j++)
                    {
                        m_canvasToDeactivate.transform.GetChild(i).GetChild(j).gameObject.SetActive(false);
                    }
                }
                m_canvasToDeactivate.gameObject.SetActive(false);
            }
        }

        /// <summary>
        /// Navigates to the WelcomePage canvas, AKA
        /// the main menu canvas
        /// </summary>
        public void GoToWelcomePage()
        {
            string username = PlayerPrefs.GetString("Username");

            string name = username.Split('@')[0];
            string namesub = name.Substring(1);
            string nameFinal = char.ToUpper(name.ToCharArray()[0]) + namesub + ".";

            m_userManager.GetComponent<UserManager>().Load(username);

            m_mainMenu.transform.FindChild("WelcomeText").GetComponent<Text>().text = "Welcome,\n" + nameFinal;
            Switch();
        }
    }
}
