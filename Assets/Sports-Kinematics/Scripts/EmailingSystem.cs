using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Mail;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;

namespace SportsKinematics
{
    /// <summary>
    /// Email a document to a client. Is static, so functions are used without an object reference. 
    /// </summary>
    static class EmailingSystem
    {
        /// <summary>
        /// Sends an email to a specific recipient. Used as "EmailingSystem.SendEmail(<params>);"
        /// </summary>
        /// <param name="body">Message body to send</param>
        /// <param name="senderEm">Sender's email address</param>
        /// <param name="senderName">Sender's name</param>
        /// <param name="host">host for sending</param>
        /// <param name="recipientEm">recipient's email address</param>
        /// <param name="senderPass">password for sender</param>
        /// <param name="attachPath">Paths for any attachments</param>
        /// <author>Aiden Triffitt</author>
        /// <date>29/04/2017</date>
        public static void SendEmail(string body, string senderEm, string senderName, string host, string recipientEm, string senderPass, string[] attachPath)
        {
            try
            {
                var fromAddress = new MailAddress(senderEm, senderName);
                var toAddress = new MailAddress(recipientEm);
                const string subject = "Automated Email Service.";

                var smtp = new SmtpClient
                {
                    Host = host,
                    EnableSsl = true,
                    DeliveryMethod = SmtpDeliveryMethod.Network,
                    UseDefaultCredentials = false,
                    Credentials = (ICredentialsByHost)new NetworkCredential(senderEm, senderPass)
                };

                using (MailMessage message = new MailMessage(fromAddress, toAddress)
                {
                    Subject = subject,
                    Body = body
                })
                {
                    message.IsBodyHtml = true;

                    foreach (string att in attachPath)
                    {
                        var attachment = new Attachment(att);

                        message.Attachments.Add(attachment);
                    }

                    ServicePointManager.ServerCertificateValidationCallback =
                    delegate (object s, X509Certificate certificate, X509Chain chain, SslPolicyErrors sslPolicyErrors)
                    { return true; };

                    smtp.Send(message);
                }
            }
            catch (Exception)
            {

            }
        }

        /// <summary>
        /// Sends an email to a specific recipient. Used as "EmailingSystem.SendEmail(<params>);"
        /// </summary>
        /// <param name="body">Message body to send</param>
        /// <param name="senderEm">Sender's email address</param>
        /// <param name="senderName">Sender's name</param>
        /// <param name="host">host for sending</param>
        /// <param name="recipientEm">recipient's email address</param>
        /// <param name="senderPass">password for sender</param>
        /// <param name="attachPath">Paths for any attachments</param>
        /// <author>Aiden Triffitt</author>
        /// <date>29/04/2017</date>
        public static void SendEmail(string body, string senderEm, string senderName, string host, string recipientEm, string senderPass, List<string> attachPath)
        {
            try
            {
                var fromAddress = new MailAddress(senderEm, senderName);
                var toAddress = new MailAddress(recipientEm);
                const string subject = "Automated Email Service.";

                var smtp = new SmtpClient
                {
                    Host = host,
                    EnableSsl = true,
                    DeliveryMethod = SmtpDeliveryMethod.Network,
                    UseDefaultCredentials = false,
                    Credentials = (ICredentialsByHost)new NetworkCredential(senderEm, senderPass)
                };

                using (MailMessage message = new MailMessage(fromAddress, toAddress)
                {
                    Subject = subject,
                    Body = body
                })
                {
                    message.IsBodyHtml = true;

                    foreach (string att in attachPath)
                    {
                        var attachment = new Attachment(att);

                        message.Attachments.Add(attachment);
                    }

                    ServicePointManager.ServerCertificateValidationCallback =
                    delegate (object s, X509Certificate certificate, X509Chain chain, SslPolicyErrors sslPolicyErrors)
                    { return true; };

                    smtp.Send(message);
                }
            }
            catch (Exception)
            {
            }
        }

        /// <summary>
        /// Sends an email to a specific recipient. Used as "EmailingSystem.SendEmail(<params>);"
        /// <param name="em">EmailDetails object, which contains all required email data</param>
        /// <author>Aiden Triffitt</author>
        /// <date>29/04/2017</date>
        public static void SendEmail(EmailDetails em)
        {
            try
            {
                var fromAddress = new MailAddress(em.SenderEmail, em.SenderName);
                var toAddress = new MailAddress(em.RecipientEmail);
                //const string subject = "Automated Email Service.";

                var smtp = new SmtpClient
                {
                    Host = em.Host,
                    EnableSsl = true,
                    DeliveryMethod = SmtpDeliveryMethod.Network,
                    UseDefaultCredentials = false,
                    Credentials = (ICredentialsByHost)Extract(em)
                };

                using (MailMessage message = new MailMessage(fromAddress, toAddress)
                {

                    Subject = em.Subject,
                    Body = em.Body
                })
                {
                    foreach (MailAddress cc in em.CC)
                    {
                        message.CC.Add(cc);
                    }

                    message.IsBodyHtml = true;

                    //if (em.Attachment != null)
                    //{
                    //    var attachment = new Attachment(em.Attachment);
                    //    message.Attachments.Add(attachment);
                    //}

                    if (em.Attachments != null && em.Attachments.Count > 0)
                    {
                        foreach (string att in em.Attachments)
                        {
                            var attachment = new Attachment(att);

                            message.Attachments.Add(attachment);
                        }
                    }

                    ServicePointManager.ServerCertificateValidationCallback =
                    delegate (object s, X509Certificate certificate, X509Chain chain, SslPolicyErrors sslPolicyErrors)
                    { return true; };

                    smtp.Send(message);
                }
            }
            catch (Exception)
            {
            }
        }

        private static NetworkCredential Extract(EmailDetails em)
        {
            return new NetworkCredential(em.SenderEmail, em.SenderPassword);
        }
    }
}
