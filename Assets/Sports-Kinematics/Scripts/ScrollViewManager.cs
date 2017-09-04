using System.IO;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

namespace SportsKinematics.UI
{
    /// <summary>
    /// Manages the Scroll view in the playlist builder canvas. The PlaylistBuilder class calls ScrollViewManager when
    /// a new scenario is added to a playlist. The ScrollViewManager handles the addition of the scenario into the scroll view in the playlist
    /// builder canvas. 
    /// A scenario is represented by a two buttons:
    ///     1. The configuration button whos text contains the name of the action added, thus creating a scenario. Clicking this button allows you to
    ///         configure this scenario.
    ///     2. The delete button. When clicked, purges the scenario from the scroll view and also removes all data files relating to that scenario.
    ///     
    /// Scroll Views are annoying.
    /// </summary>
    public class ScrollViewManager : MonoBehaviour
    {
        /// <summary>
        /// Canvas to activate when the configure scenario button is clicked.
        /// </summary>
        public Canvas m_canvasToActivate;
        /// <summary>
        /// Canvas to de-activate when the configure scenario button is clicked.
        /// </summary>
        public Canvas m_canvasToDeactivate;
        /// <summary>
        /// Prefeb for the button gameobject used in the scroll view for each scenario. When
        /// clicked the configure canvas for that scenario is loaded.
        /// </summary>
        public GameObject m_scenarioButtonPrefab;
        /// <summary>
        /// Prefeb for the button gameobject used in the scroll view for each scenario. When
        /// clicked the relating scenario is removed from the playlist and the scroll view.
        /// </summary>
        public GameObject m_removeScenarioButtonPrefab;
        /// <summary>
        /// PlaylistBuilder gameobject. Used to retrieve PlaylistBuilder script.
        /// </summary>
        public GameObject m_playlistBuilderGameObject;
        /// <summary>
        /// Gameobject with content of the scroll view that contains the configure scenario buttons.
        /// </summary>
        public GameObject m_scenarioNameColumn;
        /// <summary>
        /// Gameobject with content of the scroll view that contains the remove scenario buttons.
        /// </summary>
        public GameObject m_removeScenarioColumn;
        /// <summary>
        /// Index of the action name in the action data csv string.
        /// </summary>
        public int m_actionNameIndex;

        /// <summary>
        /// PlaylistBuilder object.
        /// </summary>
        private PlaylistBuilder m_playlistBuilder;
        /// <summary>
        /// Used to switch cavases when configure scenario button is clicked.
        /// </summary>
        private SwitchCanvasOnClick m_switcher;
        /// <summary>
        /// Configure scenario button instantiated from the m_scenarioButtonPrefab.
        /// </summary>
        private Button m_newScenarioButton;
        /// <summary>
        /// Remove scenario button instantiated from the m_removeScenarioButtonPrefab.
        /// </summary>
        private Button m_newRemoveScenarioButton;

        /// <summary>
        /// Awake - Used to initialise member variables.
        /// </summary>
        void Awake()
        {
            m_playlistBuilder = m_playlistBuilderGameObject.GetComponent<PlaylistBuilder>();
            m_actionNameIndex = 0;
            m_switcher = gameObject.AddComponent<SwitchCanvasOnClick>();
        }

        /// <summary>
        /// Adds a new scenario to the scroll view. Adding a scenario to the scroll view involves the creation 
        /// of the scenario's configure button (whos button text is the action name) and its delete button.
        /// </summary>
        /// <param name="newScenario">Struct of the scenario to be added.</param>
        /// <param name="mode">Edited or Unedited action in the scenario.</param>
        public void AddScenario(Scenario newScenario, string mode)
        {
            SetupScenarioButton(newScenario.actionDataPaths[m_actionNameIndex], newScenario.configuration, newScenario.scenarioID, mode);
            SetupRemoveScenarioButton(newScenario.actionDataPaths[m_actionNameIndex], newScenario.configuration, newScenario.scenarioID, mode);
        }

        /// <summary>
        /// Instantiates the remove scenario button and adds it to the scroll view. This also adds its onclick event.
        /// This also generates a unique name for the gameobject based off the scenario GUID and other relevant information.
        /// </summary>
        /// <param name="actionName">Name of the action the button removes.</param>
        /// <param name="configurationName">Name of th configuration</param>
        /// <param name="scenarioID">GUID of the scenario.</param>
        /// <param name="mode">Edited or Unedited action attached to the scenario this button deletes.</param>
        private void SetupRemoveScenarioButton(string actionName, string configurationName, string scenarioID, string mode)
        {
            m_newRemoveScenarioButton = Instantiate(m_removeScenarioButtonPrefab).GetComponent<Button>();
            m_newRemoveScenarioButton.transform.SetParent(m_removeScenarioColumn.GetComponent<Transform>().transform, false);
            m_newRemoveScenarioButton.name = scenarioID + "_" + actionName + "_" + configurationName + "_" + mode;
            m_newRemoveScenarioButton.onClick.AddListener(RemoveScenarioOnClick);
        }

        /// <summary>
        /// Instantiates the configure scenario button and adds it to the scroll view. This also adds its onclick event.
        /// This also generates a unique name for the gameobject based off the scenario GUID and other relevant information.
        /// </summary>
        /// <param name="actionName">Name of the action this button configures.</param>
        /// <param name="configurationName">Name of the configuration file for this scenario.</param>
        /// <param name="scenarioID">GUID of the scenario.</param>
        /// <param name="mode">Edited or Unedited action attached to the scenario this button deletes.</param>
        private void SetupScenarioButton(string actionName, string configurationName, string scenarioID, string mode)
        {
            m_newScenarioButton = Instantiate(m_scenarioButtonPrefab).GetComponent<Button>();
            m_newScenarioButton.transform.SetParent(m_scenarioNameColumn.GetComponent<Transform>().transform, false);
            m_newScenarioButton.name = scenarioID + "_" + actionName + "_" + configurationName + "_" + mode;
            m_newScenarioButton.GetComponentInChildren<Text>().text = actionName;
            m_switcher.m_canvasToActivate = m_canvasToActivate;
            m_switcher.m_canvasToDeactivate = m_canvasToDeactivate;
            m_newScenarioButton.onClick.AddListener(ConfigureScenarioOnClick);
        }

        /// <summary>
        /// The function attached to the onclick event of the remove scenario button in the scroll view. This removes all related 
        /// data to the scenario being deleted.
        /// </summary>
        private void RemoveScenarioOnClick()
        {
            var go = EventSystem.current.currentSelectedGameObject;
            if (go != null)
            {
                string[] scenarioFileNames = go.name.Split('_');
                int scenarioIndex = RetrieveScenarioIndex(scenarioFileNames[0]);

                Destroy(m_scenarioNameColumn.transform.GetChild(scenarioIndex).gameObject);
                Destroy(m_removeScenarioColumn.transform.GetChild(scenarioIndex).gameObject);
                m_playlistBuilder.CurrentPlaylist.Remove(scenarioIndex);
                File.Delete(PlayerPrefs.GetString("CurrentUserDataPath") + "/Configurations/" + "/newConfigurations/" + scenarioFileNames[2]);
                ConfigurationBuilder cbuilder = m_playlistBuilder.m_configurationManagerGameObject.GetComponent<ConfigurationBuilder>();
                cbuilder.ResetCanvas();
            }
            else
                Debug.Log("currentSelectedGameObject is null");
        }

        /// <summary>
        /// The function attached to the onclick event of the configure scenario button in the scroll view. This switches to
        /// the configuration canvas and initiates its setup based on the scenario being configured.
        /// </summary>
        private void ConfigureScenarioOnClick()
        {
            var go = EventSystem.current.currentSelectedGameObject;
            if (go != null)
            {
                string[] scenarioFileNames = go.name.Split('_');
                int scenarioIndex = RetrieveScenarioIndex(scenarioFileNames[0]);

                PlayerPrefs.SetInt("IndexBeingModified", scenarioIndex);
                PlayerPrefs.SetString("ActionData", string.Join(",", m_playlistBuilder.CurrentPlaylist.Scenarios[PlayerPrefs.GetInt("IndexBeingModified")].actionDataPaths));
                m_switcher.Switch();
                PlayerPrefs.SetInt("RefreshScrollView", 1);
                ConfigurationBuilder cbuilder = m_playlistBuilder.m_configurationManagerGameObject.GetComponent<ConfigurationBuilder>();
                PlayerPrefs.SetString("ConfigBeingModified", System.IO.Path.GetFileNameWithoutExtension(scenarioFileNames[2]));
                cbuilder.RestoreSettings(Serial<Configuration>.Load(PlayerPrefs.GetString("CurrentUserDataPath") + "/Configurations/" + "/newConfigurations/" + scenarioFileNames[2]));
            }
            else
                Debug.Log("currentSelectedGameObject is null");
        }

        /// <summary>
        /// Updates the unique name of the remove and configure buttons in the scroll view that relate to the scenario specified by the 
        /// parameters passed in. This is called when a scenario's configuration is changed.
        /// </summary>
        /// <param name="index">Index in the playlist of the scenario in the scroll view to update.</param>
        /// <param name="scenario">Scenario Struct relating to the scenario being updated.</param>
        public void UpdateScenarioObjectNames(int index, Scenario scenario)
        {
            string mode = m_scenarioNameColumn.transform.GetChild(index).gameObject.name.Split('_')[3].Split('.')[0];
            string newName = m_scenarioNameColumn.transform.GetChild(index).gameObject.name.Split('_')[0] + "_" + scenario.actionDataPaths[m_actionNameIndex] + "_" + scenario.configuration + "_" + mode;

            m_scenarioNameColumn.transform.GetChild(index).gameObject.name = newName;
            m_removeScenarioColumn.transform.GetChild(index).gameObject.name = newName;
        }
        
        /// <summary>
        /// Retrieves a scenarios index in the playlist based off the GUID passed in.
        /// </summary>
        /// <param name="scenarioID">GUID. Used to find the index relating to this scenario.</param>
        /// <returns>The index of the sceario relating to the GUID.</returns>
        private int RetrieveScenarioIndex(string scenarioID)
        {
            for (int i = 0; i < m_playlistBuilder.CurrentPlaylist.Count; i++)
            {
                if (m_playlistBuilder.CurrentPlaylist.Scenarios[i].scenarioID == scenarioID)
                    return i;
            }
            Debug.Log("Couldn't find a Scenario with GUI: " + scenarioID);
            return -1;
        }

        /// <summary>
        /// The function attached to the onclick event of the cancelPlaylist button.
        /// </summary>
        public void RemoveAllScenarios()
        {
            DeleteChildren(m_scenarioNameColumn.transform);
            DeleteChildren(m_removeScenarioColumn.transform);
        }

        /// <summary>
        /// Deletes all the children off the transform passed in. Used to remove all the scenario buttons in the
        /// scroll view.
        /// </summary>
        /// <param name="parent">The transform of the parent whos children will be brutally killed.</param>
        private void DeleteChildren(Transform parent)
        {
            foreach(Transform child in parent)
            {
                GameObject.Destroy(child.gameObject);
            }
        }      
    }
}