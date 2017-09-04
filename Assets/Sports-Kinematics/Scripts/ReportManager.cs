using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using UnityEngine;

namespace SportsKinematics
{
    /// <summary>
    /// Manages the creation of the overall and scenario reports for a playlist currently loaded into
    /// the simulation. Reports are always saved locally and emailed to the user attached email if there
    /// is an internet connection available.
    /// </summary>
    public class ReportManager : MonoBehaviour
    {
        /// <summary>
        /// Contains the report CSV string for each scenario that has been completed in the currently
        /// running playlist.
        /// </summary>
        private string m_overallReport;
        /// <summary>
        /// Contains the report CSV string for the most recently completed scenario in the current playlist.
        /// </summary>
        private string m_individualReport;
        /// <summary>
        /// Thread used to send report emails during the simulation.
        /// </summary>
        private Thread t_generateReport;
        /// <summary>
        /// Project email address to send reports to.
        /// </summary>
        private string m_projectEmail;
        /// <summary>
        /// Name of the current playlist in the simulation.
        /// </summary>
        private string m_playlistName;
        /// <summary>
        /// Username of the currently logged in user.
        /// </summary>
        private string m_username;
        /// <summary>
        /// Email address of the currently logged in user. Empty if the user has no
        /// attached email.
        /// </summary>
        private string m_userEmail;

        /// <summary>
        /// Awake used to initialise member variables.
        /// </summary>
        void Awake()
        {
            m_individualReport = "";
            m_overallReport = "";
            m_projectEmail = "sports.kinematics.murdoch@gmail.com";
        }

        /// <summary>
        /// Start - Initialises the email addresses to send reports to.
        /// </summary>
        void Start()
        {
            m_playlistName = System.IO.Path.GetFileNameWithoutExtension(PlayerPrefs.GetString("PlaylistPath"));
            m_username = PlayerPrefs.GetString("Username");

            if (PlayerPrefs.GetInt("EnableEmail") == 1)
            {
                m_userEmail = PlayerPrefs.GetString("ReportEmailAddress");
            }
            else
            {
                m_userEmail = null;
            }
        }

        /// <summary>
        /// Creates a CSV entry for a scenario in the playlist using the 
        /// configuration file passed as a parameter. The scenario report is 
        /// also appended to the overall report.
        /// </summary>
        /// <param name="config">Configuration file to be converted to a CSV entry.</param>
        public void CreateSceneReport(Configuration config)
        {
            string overall = PlayerPrefs.GetString("OverallReport");
            overall += Reporter.SceneToCSV(config);
            PlayerPrefs.SetString("OverallReport", overall);
            m_individualReport = Reporter.SceneToCSV(config);
        }

        /// <summary>
        /// Creates a CSV entry for a scenario in the playlist using the 
        /// configuration file and results passed as a parameter. The scenario report is 
        /// also appended to the overall report.
        /// </summary>
        /// <param name="config">Configuration file to be converted to a CSV entry.</param>
        /// <param name="correctSwing">String representing if the user swung in the correct direction.</param>
        /// <param name="paddleBallDistance">The distance from the ball to the paddle.</param>
        /// <param name="paddleBallDisplacement">Vector representing the distance from the paddle to ball.</param>
        public void CreateSceneReport(Configuration config, string correctSwing, string paddleBallDistance, string paddleBallDisplacement)
        {
            string overall = PlayerPrefs.GetString("OverallReport");
            overall += Reporter.SceneToCSV(config, correctSwing, paddleBallDistance, paddleBallDisplacement);
            PlayerPrefs.SetString("OverallReport", overall);
            m_individualReport = Reporter.SceneToCSV(config, correctSwing, paddleBallDistance, paddleBallDisplacement);
        }

        /// <summary>
        /// Saves the current sceario report to file and also generates a thread
        /// to attempt to email it to the user attached email address.
        /// </summary>
        /// <param name="path">Location to save the report to file.</param>
        public void SaveAndSend(string path)
        {
            string individualToSend = Reporter.ConfigCSVHeaders() + m_individualReport;
            Reporter.Save(path, individualToSend);

            string reportFilename = System.IO.Path.GetFileNameWithoutExtension(path);
            string subject = "Automated Email - Individual Report " + reportFilename + " for " + m_username;
            string body = "Individual Report for scenario number " + reportFilename.Split(' ')[2] +
                " in the playlist " + m_playlistName + " for the username " + m_username + ".";

            if (!Application.isEditor)
            {
                t_generateReport = new Thread(() => AttemptEmail(path, subject, body));
                if (!t_generateReport.IsAlive)
                    t_generateReport.Start();
            }
        }

        /// <summary>
        /// Saves the overall report to file and also generates a thread
        /// to attempt to email it to the user attached email address.
        /// </summary>
        /// <param name="path">Location to save the report to file.</param>
        public void SaveAndSendOverall(string path)
        {
            m_overallReport = PlayerPrefs.GetString("OverallReport");
            m_overallReport = Reporter.ConfigCSVHeaders() + m_overallReport;
            Reporter.Save(path, m_overallReport);

            string reportFilename = System.IO.Path.GetFileNameWithoutExtension(path);
            string subject = "Automated Email - Overall Report " + reportFilename + " for " + m_username;
            string body = "Overall Report for playlist " + m_playlistName + " for the username " + m_username + ".";

            if (!Application.isEditor)
            {
                t_generateReport = new Thread(() => AttemptEmail(path, subject, body));
                if (!t_generateReport.IsAlive)
                    t_generateReport.Start();
            }
        }

        /// <summary>
        /// Attempts to send an email, by first checking internet connectivity.
        /// </summary>
        /// <param name="path">Path to the email attachment.</param>
        /// <param name="subject">Subjet of the email.</param>
        /// <param name="body">The body text of the email.</param>
        private void AttemptEmail(string path, string subject, string body)
        {
            if (Connectivity.CheckConnection())
            {
                if (!NameHelper.IsNullOrEmpty(m_userEmail))
                {
                    Email(m_userEmail, m_projectEmail, path, subject, body);
                }
                else
                {
                    Email(m_projectEmail, null, path, subject, body);
                }
            }
        }

        /// <summary>
        /// Sends an email, created using the specified information.
        /// </summary>
        /// <param name="recipient">Email address of the recipient.</param>
        /// <param name="cc">The email address of anyone to be CC'ed.</param>
        /// <param name="attachmentPath">Path to an attachment.</param>
        /// <param name="subject">Subject text of the email.</param>
        /// <param name="body">Nody text of the email.</param>
        public void Email(string recipient, string cc, string attachmentPath, string subject, string body)
        {
            string pass = "Krs2017!";
            string senderName = "[NO REPLY] Kinematic Research Solutions";
            string host = "smtp.gmail.com";

            EmailDetails em = new EmailDetails(recipient, m_projectEmail, pass, senderName, host);

            if (File.Exists(attachmentPath))
            {
                em.AddAttachment(attachmentPath);
            }

            if (!NameHelper.IsNullOrEmpty(cc))
            {
                em.AddCC(cc);

            }

            em.Subject = subject;
            em.Body = body;

            EmailingSystem.SendEmail(em);
        }
    }
}
