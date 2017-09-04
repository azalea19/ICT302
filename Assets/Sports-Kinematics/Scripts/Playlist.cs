using System;
using System.Collections.Generic;

namespace SportsKinematics
{
    [Serializable]
    public struct Scenario
    {
        public string scenarioID;
        public string[] actionDataPaths;
        public string configuration;
    }

    /// <summary>
    /// Defines the data structures that make up a playlist. 
    /// </summary>
    /// <author>James Howson</author>
    /// <date>01/04/2017</date>
    [Serializable]
    public class Playlist
    {
        //Name of the playlist.
        private string m_playlistName;
        //All the scenarios in the playlist.
        List<Scenario> m_scenarios;


        /// <summary>
        /// Default constructor for Playlist.
        /// </summary>
        /// <author>James Howson</author>
        /// <date>01/04/2017</date>
        public Playlist()
        {
            m_playlistName = "empty";
            m_scenarios = new List<Scenario>();
        }

        /// <summary>
        /// Instance constructor for playlist. Initializes a Playlist with a
        /// name, as specified, but an empty scenarios list.
        /// </summary>
        /// <param name="name">Name of the playlist being created.</param>
        /// <author>James Howson</author>
        /// <date>01/04/2017</date>
        public Playlist(string name)
        {
            m_playlistName = name;
            m_scenarios = new List<Scenario>();
        }

        /// <summary>
        /// Instance constructor for playlist. Initializes a playlist with the 
        /// member varialbles, as specified as parameters.
        /// </summary>
        /// <param name="name">Name of the playlist being constructed.</param>
        /// <param name="scenarios">List of all scenarios in the playlist.</param>
        /// <author>James Howson</author>
        /// <date>01/04/2017</date>
        public Playlist(string name, List<Scenario> scenarios)
        {
            m_playlistName = name;
            m_scenarios = scenarios;
        }

        /// <summary>
        /// Accessor and mutator functions for m_playlistName.
        /// </summary>
        /// <author>James Howson</author>
        /// <date>01/04/2017</date>
        public string Name
        {
            get { return m_playlistName; }
            set { m_playlistName = value; }
        }

        /// <summary>
        /// Accessor and mutator functions for m_scenarios.
        /// </summary>
        /// <author>James Howson</author>
        /// <date>01/04/2017</date>
        public List<Scenario> Scenarios
        {
            get { return m_scenarios; }
            set { m_scenarios = value; }
        }

        /// <summary>
        /// Accessor function for the number of scenarios in the list.
        /// </summary>
        /// <author>James Howson</author>
        /// <date>01/04/2017</date>
        public int Count
        {
            get { return m_scenarios.Count; }
        }

        /// <summary>
        /// Adds a Scenario to the list of scenarios.
        /// </summary>
        /// <param name="action">The scenario to be added to the list of scenarios.</param>
        /// <author>James Howson</author>
        /// <date>01/04/2017</date>
        public void Add(Scenario scenario)
        {
            m_scenarios.Add(scenario);
        }

        /// <summary>
        /// Remove a scenario from the list of scenarios at the specified index.
        /// </summary>
        /// <param name="i">The index of the scenario to remove from the list of actions.</param>
        /// <author>James Howson</author>
        /// <date>01/04/2017</date>
        public void Remove(int i)
        {
            m_scenarios.RemoveAt(i);
        }

        /// <summary>
        /// Overloaded subscript operator to return the scenario at the specified index.
        /// </summary>
        /// <param name="index">The index of the scenario in the list to return.</param>
        /// <returns>An action.</returns>
        public Scenario this[int index]
        {
            get { return m_scenarios[index]; }
        }
    }
}

