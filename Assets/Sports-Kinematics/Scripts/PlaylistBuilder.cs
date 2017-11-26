using System.IO;
using System;
using UnityEngine;
using UnityEngine.UI;

namespace SportsKinematics
{
    /// <summary>
    /// Provides all the appropriate functionality to the playlist builder
    /// canvas in the GUI. This allows the construction and saving of playlists.
    /// It also controls the configuration builder and scroll view manager objects.
    /// </summary>
    public class PlaylistBuilder : MonoBehaviour//FR1 - Virtual opponent modelling from captured data.
    {
        /// <summary>
        /// Canvas gameobject to activate after saving a playlist.
        /// </summary>
        public Canvas m_canvasToActivate;
        /// <summary>
        /// Canvas gameobject to de-activate after saving a playlist.
        /// </summary>
        public Canvas m_canvasToDeactivate;
        /// <summary>
        /// COnfigurationManager gameobject, used to retrieve the ConfigurationBuilder. 
        /// </summary>
        public GameObject m_configurationManagerGameObject;
        /// <summary>
        /// ScrollViewPopulator gameobject, used to retrieve the ScrollViewManager.
        /// </summary>
        public GameObject m_scrollViewPopulatorGameObject;
        /// <summary>
        /// InputField used to set the name of the playlist being built.
        /// </summary>
        public InputField m_nameInputField;
        /// <summary>
        /// Text gameobject that contains the error message that pops up if a playlist name isn't valid.
        /// </summary>
        public Text m_NameInputErrorText;

        /// <summary>
        /// ConfigurationBuilder object used to handle the configuration of a scene.
        /// </summary>
        private ConfigurationBuilder m_configurationBuilder;
        /// <summary>
        /// ScrollViewManager object used to handle the population of the scroll view that contains each scenario.
        /// </summary>
        private UI.ScrollViewManager m_scrollViewPopulator;
        /// <summary>
        /// Playlist object for the playlist being built.
        /// </summary>
        private Playlist m_newPlaylist;
        /// <summary>
        /// Used to switch between the playlist builder canvas and the main menu canvas after saving and exiting the playlist builder.
        /// </summary>
        private UI.SwitchCanvasOnClick m_switcher;
        /// <summary>
        /// Bool that specifies if the currently entered playlist name is unique.
        /// </summary>
        private bool m_uniquePlaylistName;
        /// <summary>
        /// The extension the playlist serialised file will use.
        /// </summary>
        private string m_playlistExtension;

        /// <summary>
        /// Awake used to initialise the above game objects.
        /// </summary>
        void Awake()//FR1 - Virtual opponent modelling from captured data.
        {
            m_nameInputField.onEndEdit.AddListener(delegate { SavePlaylistName(); });
            m_configurationBuilder = m_configurationManagerGameObject.GetComponent<ConfigurationBuilder>();
            m_scrollViewPopulator = m_scrollViewPopulatorGameObject.GetComponent<UI.ScrollViewManager>();
            m_playlistExtension = PlayerPrefs.GetString("PlaylistExtension");
            m_NameInputErrorText.enabled = false;
            m_newPlaylist = new Playlist();
            m_switcher =  gameObject.AddComponent<UI.SwitchCanvasOnClick>();
            PlayerPrefs.SetInt("RefreshScrollView", 0);
        }

        /// <summary>
        /// Update loop. Ensures each of the scenario entrys in the scroll view are up to date when
        /// a scenario is configured.
        /// </summary>
        void Update()
        {
            if ((PlayerPrefs.GetInt("Configured") == 1) && (m_newPlaylist.Count != 0))
            {
                int i = PlayerPrefs.GetInt("IndexBeingModified");
                Scenario currentScenario = RetrieveCurrentScenario();

                string scenarioID = m_newPlaylist.Scenarios[i].scenarioID;
                currentScenario.scenarioID = scenarioID;
                m_newPlaylist.Scenarios[i] = currentScenario;

                m_scrollViewPopulator.UpdateScenarioObjectNames(i, currentScenario);
                PlayerPrefs.SetInt("Configured", 0);
                m_configurationBuilder.ResetCanvas();
            }
        }

        /// <summary>
        /// Get the current playlist being built.
        /// </summary>
        public Playlist CurrentPlaylist//FR1 - Virtual opponent modelling from captured data.
        {
            get { return m_newPlaylist; }
        }

        /// <summary>
        /// Checks if the playlist name entered into the playlist name text field is unique.
        /// </summary>
        /// <param name="name">The name entered into the text field.</param>
        /// <returns>True if the name is unique.</returns>
        private bool IsPlaylistNameUnique(string name)//FR1 - Virtual opponent modelling from captured data.
        {
            return !File.Exists(PlayerPrefs.GetString("CurrentUserDataPath") + "Playlists/" + m_nameInputField.text + m_playlistExtension) && !NameHelper.IsNullOrEmpty(m_nameInputField.text);
        }

        /// <summary>
        /// Determines if all the conditions are met in order for the user entered playlist name
        /// to be saved to the playlist object. Also handles the dispaly
        /// of the error text if the name isnt unique.
        /// </summary>
        private void SavePlaylistName()//FR1 - Virtual opponent modelling from captured data.
        {
            if((!IsPlaylistNameUnique(m_nameInputField.text)) && (!m_nameInputField.text.Equals("")))
            {
                m_uniquePlaylistName = false;
                m_NameInputErrorText.enabled = true;
            }
            else
            {
                m_uniquePlaylistName = true;
                m_NameInputErrorText.enabled = false;
                m_newPlaylist.Name = m_nameInputField.text;
            }
        }

        /// <summary>
        /// Adds the current scenario to the playlist game object. Also manages the population of the scroll view
        /// based on the scenario added.
        /// </summary>
        public void AddScenario()//FR1 - Virtual opponent modelling from captured data.
        {
            Scenario newScenario = RetrieveCurrentScenario();
            m_newPlaylist.Add(newScenario);
            PlayerPrefs.SetInt("ScenarioCount", m_newPlaylist.Count);
            m_scrollViewPopulator.AddScenario(newScenario, PlayerPrefs.GetString("Mode"));
        }

        /// <summary>
        /// Handles the retrieval of all the data that makes up a Scenario.
        /// </summary>
        /// <returns>The new scenario to be added.</returns>
        private Scenario RetrieveCurrentScenario()//FR1 - Virtual opponent modelling from captured data.
        {
            Scenario newScenario;
            m_configurationBuilder.SaveConfiguration();
            string[] temp = PlayerPrefs.GetString("ActionData").Split(',');
            newScenario.scenarioID = Guid.NewGuid().ToString();
            newScenario.actionDataPaths = temp;
            newScenario.configuration = PlayerPrefs.GetString("LatestConfigurationFileName");
            return newScenario;
        }

        /// <summary>
        /// Serializes the current playlist being built. Also handles the switching of canvas if the playlist
        /// saves correctly.
        /// </summary>
        public void SavePlaylist()//FR1 - Virtual opponent modelling from captured data.
        {
            string filename = m_newPlaylist.Name + m_playlistExtension;
            if(m_newPlaylist.Count != 0)
            {
                if (!IsPlaylistNameUnique(m_nameInputField.text) && NameHelper.IsValidFilename(filename))
                {
                    m_nameInputField.Select();
                    m_nameInputField.text = "";
                }
                else
                {
                    string savePath = PlayerPrefs.GetString("CurrentUserDataPath") + "Playlists/";
                    Serial<Playlist>.Save(m_newPlaylist, filename, savePath);
                    Directory.Move(PlayerPrefs.GetString("CurrentUserDataPath") + "/Configurations/newConfigurations/", PlayerPrefs.GetString("CurrentUserDataPath") + "/Configurations/" + filename.Split('.')[0] + "/");
                    m_switcher.m_canvasToActivate = m_canvasToActivate;
                    m_switcher.m_canvasToDeactivate = m_canvasToDeactivate;
                    m_switcher.Switch();
                    PlayerPrefs.SetString("PlaylistPath", savePath + filename);
                    GameObject.Find("Simulation/SimulationCanvas/CurrentPlaylistNameLabel").GetComponent<UI.CheckPlaylistLoad>().ChangeText(m_newPlaylist.Name);
                    PurgePlaylistBuilder();
                }
            }

            if (!Server.Database.PlaylistExist(m_newPlaylist.Name))
            {
                Server.Database.AddPlaylist(m_newPlaylist.Name);
            }
            foreach (Scenario s in m_newPlaylist.Scenarios)
            {
                //Database.AddExpPlaylist()
            }
        }

        /// <summary>
        /// The function attached to the onclick event of the create playlist button. Sets up the appropriate files which
        /// will be used in playlist creation.
        /// </summary>
        public void CreatePlaylistOnClick()
        {
            Directory.CreateDirectory(PlayerPrefs.GetString("CurrentUserDataPath") + "/Configurations/newConfigurations/");
            Directory.CreateDirectory(PlayerPrefs.GetString("CurrentUserDataPath") + "Playlists/");
        }

        /// <summary>
        /// Purges al the information/data involved in the creation of a playlist. Called if the playlist build is cancelled.
        /// </summary>
        public void PurgePlaylistBuilder()
        {
            if(m_newPlaylist.Count > 0)
            {
                string dir = PlayerPrefs.GetString("CurrentUserDataPath") + "/Configurations/newConfigurations/";
                if(Directory.Exists(dir))
                    Directory.Delete(dir, true);
                m_scrollViewPopulator.RemoveAllScenarios();
            }
            m_nameInputField.text = "";
        }
    }
}

