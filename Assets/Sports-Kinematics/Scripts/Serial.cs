using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;

namespace SportsKinematics
{
    /// <summary>
    /// Wrapper for Serialize using Generic Classes and Methods.
    /// </summary>
    /// <typeparam name="T">Generic type.</typeparam>
    public static class Serial<T> where T : class
    {
        /// <summary>
        /// Serialize an object into Binary Formatting. Object must be marked as [Serializable]
        /// </summary>
        /// <param name="fileN">string      File Name, not including folder reference. "example/savename.extension" is enterred as "savename.extension"</param>
        /// <param name="folderN">string    Folder Name, not including file reference. "example/" is enterred here.</param>
        /// <param name="other">            Templated object for serialization. Must be marked [Serializable]</param>
        /// <author>Aiden Triffitt</author>
        /// <date>10/04/2017</date>
        public static void Save(T other, string fileN, string folderN)//FR5 - Data logging facilities.
        {
            if (!folderN.Contains("/") && !folderN.Contains(@"\"))
            {
                folderN += "/";
            }

            if (!File.Exists(folderN))
            {
                Directory.CreateDirectory(folderN);
            }

            Stream stream = null;
            try
            {
                IFormatter formatter = new BinaryFormatter();
                stream = new FileStream(folderN + fileN, FileMode.Create, FileAccess.Write, FileShare.None);
                formatter.Serialize(stream, other);
            }
            catch
            {
                // do nothing, just ignore any possible errors
            }
            finally
            {
                if (null != stream)
                    stream.Close();
            }
        }

        /// <summary>
        /// Serialize an object into Binary Formatting. Object must be marked as [Serializable]
        /// </summary>
        /// <param name="fileN">string      File Name, not including folder reference. "example/savename.extension" is enterred as "savename.extension"</param>
        /// <param name="folderN">string    Folder Name, not including file reference. "example/" is enterred here.</param>
        /// <param name="other">            Templated object for serialization. Must be marked [Serializable]</param>
        /// <author>Aiden Triffitt</author>
        /// <date>10/04/2017</date>
        public static void Save(T other, string fileN)//FR5 - Data logging facilities.
        {
            string folderN = "";

            if (!folderN.Contains("/") && !folderN.Contains(@"\"))
            {
                folderN += "/";
            }

            if (!File.Exists(folderN))
            {
                Directory.CreateDirectory(folderN);
            }

            Stream stream = null;
            try
            {
                IFormatter formatter = new BinaryFormatter();
                stream = new FileStream(folderN + fileN, FileMode.Create, FileAccess.Write, FileShare.None);
                formatter.Serialize(stream, other);
            }
            catch
            {
                // do nothing, just ignore any possible errors
            }
            finally
            {
                if (null != stream)
                    stream.Close();
            }
        }

        /// <summary>
        /// Load object from serial file
        /// </summary>
        /// <param name="fileN">string      File Name, not including folder reference. "example/savename.extension" is enterred as "savename.extension"</param>
        /// <param name="folderN">string    Folder Name, not including file reference. "example/" is enterred here.</param>
        /// <returns>Object from template</returns>
        /// <author>Aiden Triffitt, James Howson</author>
        /// <date>16/04/2017</date>
        public static T Load(string fileN, string folderN)//FR5 - Data logging facilities.
        {
            Stream stream = null;
            T other = null;

            IFormatter formatter = new BinaryFormatter();
            stream = new FileStream(folderN + fileN, FileMode.Open, FileAccess.Read, FileShare.None);
            other = (T)formatter.Deserialize(stream);

            if (null != stream)
                stream.Close();

            return other;
        }

        /// <summary>
        /// Load object from serial file
        /// </summary>
        /// <param name="fileN">string      File Name, not including folder reference. "example/savename.extension" is enterred as "savename.extension"</param>
        /// <param name="folderN">string    Folder Name, not including file reference. "example/" is enterred here.</param>
        /// <returns>Object from template</returns>
        /// <author>Aiden Triffitt, James Howson</author>
        /// <date>16/04/2017</date>
        public static T Load(string fileN)//FR5 - Data logging facilities.
        {
            Stream stream = null;
            T other = null;

            IFormatter formatter = new BinaryFormatter();
            stream = new FileStream(fileN, FileMode.Open, FileAccess.Read, FileShare.None);
            other = (T)formatter.Deserialize(stream);

            if (null != stream)
                stream.Close();

            return other;
        }
    }
}