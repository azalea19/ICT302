using System.Collections.Generic;
using System.Net.Mail;

namespace SportsKinematics
{
    /// <summary>
    /// Data structure to contain email details
    /// </summary>
    public class EmailDetails
    {
        /// <summary>
        /// email subject
        /// </summary>
        string m_subject;

        /// <summary>
        /// email body
        /// </summary>
        string m_body;

        /// <summary>
        /// email recipient
        /// </summary>
        string m_recipient;

        /// <summary>
        /// sender email
        /// </summary>
        string m_senderEmail;

        /// <summary>
        /// sender password
        /// </summary>
        string m_pass;

        /// <summary>
        /// sedner's name
        /// </summary>
        string m_senderName;

        /// <summary>
        /// email host
        /// </summary>
        string m_host;

        /// <summary>
        /// list of attachment paths
        /// </summary>
        List<string> m_attachPath;

        /// <summary>
        /// CC list
        /// </summary>
        MailAddressCollection m_CC;

        /// <summary>
        /// Constructor for email details
        /// </summary>
        public EmailDetails()
        {
            m_CC = new MailAddressCollection();
        }
        
        /// <summary>
        /// Constructor for email details, taking 4 params
        /// </summary>
        /// <param name="recipient">recipient email address</param>
        /// <param name="sender">sender email address</param>
        /// <param name="password">sender password</param>
        /// <param name="host">host server name</param>
        public EmailDetails(string recipient, string sender, string password, string host)
        {
            m_recipient = recipient;
            m_senderEmail = sender;
            m_host = host;
            m_pass = password;
            m_CC = new MailAddressCollection();
        }

        /// <summary>
        /// Constructor for email details, taking 5 params
        /// </summary>
        /// <param name="recipient">recipient email address</param>
        /// <param name="sender">sender email address</param>
        /// <param name="password">sender password</param>
        /// <param name="senderName">sender name</param>
        /// <param name="host">host server name</param>
        public EmailDetails(string recipient, string sender, string password, string senderName, string host)
        {
            m_recipient = recipient;
            m_senderEmail = sender;
            m_host = host;
            m_pass = password;
            m_senderName = senderName;
            m_CC = new MailAddressCollection();
            m_attachPath = new List<string>();
        }

        /// <summary>
        /// Accessor method for body
        /// </summary>
        public string Body
        {
            get { return m_body; }
            set { m_body = value; }
        }

        /// <summary>
        /// Accessor method for subject
        /// </summary>
        public string Subject
        {
            get { return m_subject; }
            set { m_subject = value; }
        }

        /// <summary>
        /// Accessor method for recipient details
        /// </summary>
        public string RecipientEmail
        {
            get { return m_recipient; }
            set { m_recipient = value; }
        }

        /// <summary>
        /// Accessor method for sender email address
        /// </summary>
        public string SenderEmail
        {
            get { return m_senderEmail; }
            set { m_senderEmail = value; }
        }

        /// <summary>
        /// Accessor method for sender password
        /// </summary>
        public string SenderPassword
        {
            get { return m_pass; }
            set { m_pass = value; }
        }

        /// <summary>
        /// Accessor method for sender name
        /// </summary>
        public string SenderName
        {
            get { return m_senderName; }
            set { m_senderName = value; }
        }

        /// <summary>
        /// Accessor method for host name
        /// </summary>
        public string Host
        {
            get { return m_host; }
            set { m_host = value; }
        }

        /// <summary>
        /// Accessor method for CC list
        /// </summary>
        public MailAddressCollection CC
        {
            get { return m_CC; }
            set { m_CC = value; }
        }

        /// <summary>
        /// Accessor method for Attachments
        /// </summary>
        public List<string> Attachments
        {
            get { return m_attachPath; }
            set { m_attachPath = value; }
        }

        /// <summary>
        /// Add an attachment to lsit of attachments
        /// </summary>
        /// <param name="path">path to attachmnet</param>
        public void AddAttachment(string path)
        {
            m_attachPath.Add(path);
        }

        /// <summary>
        /// Add attachments to list
        /// </summary>
        /// <param name="paths">paths to attach</param>
        public void AddAttachments(List<string> paths)
        {
            foreach (string path in paths)
            {
                m_attachPath.Add(path);
            }
        }

        /// <summary>
        /// Add CC address to CC list
        /// </summary>
        /// <param name="address">email address for CC</param>
        public void AddCC(string address)
        {
            m_CC.Add(address);
        }
    }
}
