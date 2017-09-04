using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Mail;

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
                    Credentials = new NetworkCredential(senderEm, senderPass)
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

                    smtp.Send(message);
                }
            }
            catch (Exception)
            {
                Console.Out.WriteLine("E-mail failed to send!");
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
                    Credentials = new NetworkCredential(senderEm, senderPass)
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

                    smtp.Send(message);
                }
            }
            catch (Exception)
            {
                Console.Out.WriteLine("E-mail failed to send!");
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
                const string subject = "Automated Email Service.";

                var smtp = new SmtpClient
                {
                    Host = em.Host,
                    EnableSsl = true,
                    DeliveryMethod = SmtpDeliveryMethod.Network,
                    UseDefaultCredentials = false,
                    Credentials = new NetworkCredential(em.SenderEmail, em.SenderPassword)
                };

                using (MailMessage message = new MailMessage(fromAddress, toAddress)
                {
                    Subject = subject,
                    Body = em.Body
                })
                {
                    message.IsBodyHtml = true;

                    foreach (string att in em.Attachments)
                    {
                        var attachment = new Attachment(att);

                        message.Attachments.Add(attachment);
                    }

                    smtp.Send(message);
                }
            }
            catch (Exception e)
            {
                Console.Out.WriteLine("E-mail failed to send!");
            }
        }
    }
}
