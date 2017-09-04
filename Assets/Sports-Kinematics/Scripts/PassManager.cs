using UnityEngine;
using UnityEngine.UI;
using System.IO;

namespace SportsKinematics.UI
{
    /// <summary>
    /// Allows password character to be set to a bullet point
    /// </summary>
    public class PassManager : MonoBehaviour
    {
        /// <summary>
        /// Password field
        /// </summary>
        public InputField m_password;

        /// <summary>
        /// sets the password character for attached input field to a bullet point
        /// </summary>
        void Start()
        {
            m_password.asteriskChar = char.Parse(char.ConvertFromUtf32(8226));
        }
    }
}