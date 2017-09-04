using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SportsKinematics
{
    class Program
    {       
        static void Main(string[] args)
        {
            string  body = "test";

            List<string> attachBodies = new List<string>();
            attachBodies.Add("This is a report.\n\tIt features formatting.");

            string recipient = "aidentriffitt@gmail.com";
            string senderEmail = "sports.kinematics.murdoch@gmail.com";
            string pass = "Krs2017!";
            string senderName = "[NO REPLY] Kinematic Research Solutions";
            string host = "smtp.gmail.com";
            List<string> attachPath = new List<string>();
            attachPath.Add("test.txt");

            EmailDetails em = new EmailDetails(recipient, senderEmail, pass, senderName, host);

            em.Attachments = attachPath;
            em.Body = body;

            Report_Email_Interactions.SaveAttachments(attachPath, attachBodies);

            Report_Email_Interactions.email = em;

            Report_Email_Interactions.SendEmail();

            string s = Console.In.ReadLine();
        }
    }
}
