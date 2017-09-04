using UnityEngine;
using System.Text;

namespace SportsKinematics
{
    /// <summary>
    /// Class allowing encryption of data
    /// </summary>
    public static class Encryption
    {
        /// <summary>
        /// Encryption key.
        /// 
        /// Ily Shri.
        /// </summary>
        private const string m_key = "Shri Rai";

        /// <summary>
        /// XOR function to encrypt input string
        /// </summary>
        /// <param name="encrypt">string to encrypt</param>
        /// <returns>XOR encrypted data</returns>
        public static string XOR(string encrypt)
        {
            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < encrypt.Length; i++)
                sb.Append((char)(encrypt[i] ^ m_key[(i % m_key.Length)]));

            string result = sb.ToString();

            return result;
        }

        /// <summary>
        /// XOR encryption for string
        /// </summary>
        /// <param name="encrypt">string to encrypt</param>
        /// <param name="key">key to encrypt with</param>
        /// <returns>encrypted value</returns>
        public static string XOR(string encrypt, string key)
        {
            try
            {
                StringBuilder sb = new StringBuilder();
                for (int i = 0; i < encrypt.Length; i++)
                    sb.Append((char)(encrypt[i] ^ key[(i % key.Length)]));

                string result = sb.ToString();

                return result;
            }
            catch
            {

            }

            return null;
        }
    }
}