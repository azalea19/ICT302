using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SportsKinematics
{
    /// <summary>
    /// Used to generate a report for client viewing
    /// </summary>
    public static class Reporter
    {
        public static void GenerateReport(string reportPath)
        {
            string body = "test";

            Save(reportPath, body);
        }

        public static void Save(string reportPath, string body)
        {
            // Write the text asynchronously to a new file named "WriteTextAsync.txt".
            using (StreamWriter outputFile = new StreamWriter(reportPath))
            {
                outputFile.Write(body);
            }
        }
     }
}
