using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SportsKinematics
{
    /// <summary>
    /// Tracks who's logged in and allows others to log in.
    /// Utilizes PlayerPrefs to store users data when they log in and out.
    /// </summary>
    public class UserManager : MonoBehaviour
    {
        /// <summary>
        /// current user logged in
        /// </summary>
        private User m_myUser;

        /// <summary>
        /// Property for MyUser to get and set data.
        /// </summary>
        public User MyUser
        {
            get { return m_myUser; }
            set {
                m_myUser.Username = value.Username;
                m_myUser.Email = value.Email;
                m_myUser.Password = value.Password; //Copy me deeper, daddy-kun
            }
        }

        /// <summary>
        /// Changes path to relevant users directory.
        /// </summary>
        public void SetCurrentUserDataPath()
        {
            PlayerPrefs.SetString("CurrentUserDataPath", Application.dataPath + "/../Users/" + m_myUser.Username + "/");
            PlayerPrefs.SetString("CurrentUsername", m_myUser.Username);
        }

        /// <summary>
        /// Saves and encrypts new users data.
        /// Used when theyre is no loaded user.
        /// </summary>
        /// <param name="u"> Users whos data is encrypted and saved</param>
        public void Save(User u)
        {
            m_myUser = new User();

            m_myUser.Username = Encryption.XOR(u.Username);
            m_myUser.Password = Encryption.XOR(u.Password);
            m_myUser.Email = Encryption.XOR(u.Email);

            Serial<User>.Save(m_myUser, u.Username + ".shri", Application.dataPath + "/../Users/" + u.Username + "/");

            if(Server.Database.UserExist(u.Username))
            {
                Server.Database.UpdateUser(u.Username, u.Email, u.Password);
            }
            else
            {
                Server.Database.AddUser(u.Username, u.Email, u.Password);
            }
            print("../Users/" + u.Username + "/" + u.Username + ".shri");
            string r = Server.Files.UploadFile(Application.dataPath + "/../Users/" + u.Username + "/","../Users/" + u.Username + "/", u.Username + ".shri");
            print(r);

            m_myUser = u;
        }

        /// <summary>
        /// Saves currently loaded users data if no user is available.
        /// </summary>
        public void Save()
        {
            if (m_myUser != null)
            {
                Save(m_myUser);
            }
        }

        /// <summary>
        /// Loads a specific users data.
        /// </summary>
        /// <param name="username">Specific user whose data will be loaded</param>
        /// <returns>Specific users data</returns>
        public User Load(string username)
        {
            m_myUser = Serial<User>.Load(username + ".shri", Application.dataPath + "/../Users/" + username + "/");

            m_myUser.Username = Encryption.XOR(m_myUser.Username);
            m_myUser.Password = Encryption.XOR(m_myUser.Password);
            m_myUser.Email = Encryption.XOR(m_myUser.Email);

            return m_myUser;
        }

        /// <summary>
        /// Loads current logged in users data.
        /// </summary>
        /// <returns>Loaded users data</returns>
        public User Load()
        {
            if (m_myUser != null)
            {
                return Load(m_myUser.Username);
            }
            else
            {
                Debug.Log("No user available to load.");

                return null;
            }
        }
    }
}