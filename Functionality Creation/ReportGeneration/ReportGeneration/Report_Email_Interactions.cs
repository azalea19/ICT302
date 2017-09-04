using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SportsKinematics
{
    public static class Report_Email_Interactions
    {
        private static EmailDetails em;

        public static EmailDetails email
        {
            get { return em; }
            set { em = value; }
        }

        public static string Email_Body
        {
            get { return em.Body; }
            set { em.Body = value; }
        }

        public static string Email_SenderEmail
        {
            get { return em.SenderEmail; }
            set { em.SenderEmail = value; }
        }

        public static string Email_SenderName
        {
            get { return em.SenderName; }
            set { em.SenderName = value; }
        }

        public static string Email_SenderPassword
        {
            get { return em.SenderPassword; }
            set { em.SenderPassword = value; }
        }

        public static string Email_RecipientEmail
        {
            get { return em.RecipientEmail; }
            set { em.RecipientEmail = value; }
        }

        public static string Email_Host
        {
            get { return em.Host; }
            set { em.Host = value; }
        }

        public static List<string> Email_Attachments
        {
            get { return em.Attachments; }
            set { em.Attachments = value; }
        }

        public static void AddAttachment(string attachment)
        {
            em.AddAttachment(attachment);
        }

        public static void AddAttachments(List<string> attachments)
        {
            em.AddAttachments(attachments);
        }

        public static void ReportAndEmail()
        {
            foreach (string path in em.Attachments)
            {
                Reporter.GenerateReport(path);
            }

            EmailingSystem.SendEmail(em);
        }

        public static void Report()
        {
            foreach (string path in em.Attachments)
            {
                Reporter.GenerateReport(path);
            }
        }

        public static void SendEmail()
        {
            EmailingSystem.SendEmail(em);
        }

        public static void SaveAttachments(List<string> paths, List<string> bodies)
        {
            int i = 0;

            foreach (string path in paths)
            {
                Reporter.Save(path, bodies[i]);

                ++i;
            }
        }
    }
}
