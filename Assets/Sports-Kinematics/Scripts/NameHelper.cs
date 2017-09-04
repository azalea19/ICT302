using System.Text.RegularExpressions;

namespace SportsKinematics
{
    /// <summary>
    /// Determines if file name is valid
    /// </summary>
    public static class NameHelper
    {
        /// <summary>
        /// used to determine if file name is valid
        /// </summary>
        /// <param name="testName"></param>
        /// <returns></returns>
        public static bool IsValidFilename(string testName)
        {
            Regex containsABadCharacter = new Regex("["
                  + Regex.Escape(new string(System.IO.Path.GetInvalidPathChars())) + "]");
            if (containsABadCharacter.IsMatch(testName)) { return false; };

            return true;
        }

        /// <summary>
        /// used to determine if file name is not null or empty
        /// </summary>
        /// <param name="testName"></param>
        /// <returns></returns>
        public static bool IsNullOrEmpty(string testName)
        {
            return (testName == null) || (testName == "");
        }
    }
}