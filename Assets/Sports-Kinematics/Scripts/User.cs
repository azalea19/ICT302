using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SportsKinematics
{

    /// <summary>
    /// Represents a user that can be loaded and encrypted.
    /// User stores a username, password and email address.
    /// </summary>
    [Serializable]
    public class User
    {
        /// <summary>
        /// Username
        /// </summary>
        private string m_username;
        /// <summary>
        /// Email
        /// </summary>
        private string m_email;
        /// <summary>
        /// Password
        /// </summary>
        private string m_password;

        /// <summary>
        /// Property used to get and set a users username.
        /// </summary>
        public string Username
        {
            get { return m_username; }
            set { m_username = value; }
        }

        /// <summary>
        /// Property used to get and set a users email address.
        /// </summary>
        public string Email
        {
            get { return m_email; }
            set { m_email = value; }
        }

        /// <summary>
        /// Property used to get and set a users password
        /// </summary>
        public string Password
        {
            get { return m_password; }
            set { m_password = value; }
        }

        /// <summary>
        /// Default User constructor
        /// </summary>
        public User()
        { }

        /// <summary>
        /// User constructor that sets a username, email address and password,
        /// sets email as the users username.
        /// </summary>
        /// <param name="user">Username to set users username and email</param>
        /// <param name="pass">Password to set users password</param>
        public User(string user, string pass)
        {
            m_username = user;
            m_email = user;
            m_password = pass;
        }

        /// <summary>
        /// User constructor that sets a username, email address and password.
        /// </summary>
        /// <param name="user">Users username</param>
        /// <param name="email">Users email address</param>
        /// <param name="pass">Users password</param>
        public User(string user, string email, string pass)
        {
            m_username = user;
            m_email = email;
            m_password = pass;
        }

        /// <summary>
        /// Checks whether a password matches the users password
        /// </summary>
        /// <param name="pass">Password to test</param>
        /// <returns>True if matching, false otherwise</returns>
        public bool Matches(string pass)
        {
            return m_password.Equals(pass);
        }

        /// <summary>
        /// Determines if login details are correct for a users username and password
        /// </summary>
        /// <param name="user">Logging in user</param>
        /// <param name="attemptedUser">Input data for login attempt</param>
        /// <returns></returns>
        public static bool Matches(User user, User attemptedUser)
        {
            return (user.Matches(attemptedUser.Password) && user.Username.Equals(attemptedUser.Username));
        }
    }
}