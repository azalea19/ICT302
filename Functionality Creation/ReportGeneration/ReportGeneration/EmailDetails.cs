using System.Collections.Generic;

namespace SportsKinematics
{
    public class EmailDetails
    {
        string m_body;
        string m_recipient;
        string m_senderEmail;
        string m_pass;
        string m_senderName;
        string m_host;
        List<string> m_attachPath;

        public EmailDetails()
        { }

        public EmailDetails(string recipient, string sender, string password, string host)
        {
            m_recipient = recipient;
            m_senderEmail = sender;
            m_host = host;
            m_pass = password;
        }

        public EmailDetails(string recipient, string sender, string password, string senderName, string host)
        {
            m_recipient = recipient;
            m_senderEmail = sender;
            m_host = host;
            m_pass = password;
            m_senderName = senderName;
        }

        public string Body
        {
            get { return m_body; }
            set { m_body = value; }
        }

        public string RecipientEmail
        {
            get { return m_recipient; }
            set { m_recipient = value; }
        }

        public string SenderEmail
        {
            get { return m_senderEmail; }
            set { m_senderEmail = value; }
        }

        public string SenderPassword
        {
            get { return m_pass; }
            set { m_pass = value; }
        }

        public string SenderName
        {
            get { return m_senderName; }
            set { m_senderName = value; }
        }

        public string Host
        {
            get { return m_host; }
            set { m_host = value; }
        }

        public List<string> Attachments
        {
            get { return m_attachPath; }
            set { m_attachPath = value; }
        }

        public void AddAttachment(string path)
        {
            m_attachPath.Add(path);
        }

        public void AddAttachments(List<string> paths)
        {
            foreach (string path in paths)
            {
                m_attachPath.Add(path);
            }
        }
    }
}
