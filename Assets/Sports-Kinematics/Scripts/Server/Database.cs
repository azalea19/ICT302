using System;
using System.Net;
using System.Collections.Specialized;
using System.IO;
using UnityEngine;
using System.Net.Security;

namespace SportsKinematics.Server
{
    public static class Database
    {
        //static string site = "http://localhost/302test/";

        //static string site = "http://10.1.1.226/302test/";
        static string site = "https://creativesolutionsmurdoch.000webhostapp.com/pointlight/";



        //DATA INSERTION
        public static int AddUser(string username,string email, string password)
        {
            int s = -1;
            using (var client = new WebClient())
            {
                NameValueCollection parameters = new NameValueCollection();

                parameters.Add("username", username);
                parameters.Add("email", email);
                parameters.Add("password", password.GetHashCode().ToString());

                
                try
                {
                    InitiateSSLTrust();
                    Debug.Log(site + "scripts/InsertUser.php" + username + " " + email + " " + password.GetHashCode().ToString());
                    byte[] result = client.UploadValues(site + "scripts/InsertUser.php", "POST", parameters);

                    s = Convert.ToInt32(System.Text.Encoding.UTF8.GetString(result, 0, result.Length));
                }
                catch (WebException ex)
                {
                    Console.WriteLine("Server connection error.");
                }
            }
            Debug.Log(s);
            return s;
        }

        public static int AddRecording(string name,string skel, string ball, string paddle)
        {
            int s = -1;
            using (var client = new WebClient())
            {
                NameValueCollection parameters = new NameValueCollection();
                parameters.Add("name", name);
                parameters.Add("skeleton", skel);
                parameters.Add("ball", ball);
                parameters.Add("paddle", paddle);

                try
                {
                    InitiateSSLTrust();
                    byte[] result = client.UploadValues(site + "scripts/InsertRecording.php", parameters);
                    s = Convert.ToInt32(System.Text.Encoding.UTF8.GetString(result, 0, result.Length));
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Server connection error.");
                }
            }
            return s;
        }

        public static int AddExperiment(string study,int expNum, string skel, string ball, string paddle)
        {
            int s = -1;
            using (var client = new WebClient())
            {
                NameValueCollection parameters = new NameValueCollection();
                parameters.Add("studyName", study);
                parameters.Add("expNum", expNum.ToString());
                parameters.Add("skeleton", skel);
                parameters.Add("ball", ball);
                parameters.Add("paddle", paddle);

                try
                {
                    InitiateSSLTrust();
                    byte[] result = client.UploadValues(site + "scripts/InsertExperiment.php", parameters);
                    s = Convert.ToInt32(System.Text.Encoding.UTF8.GetString(result, 0, result.Length));
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Server connection error.");
                }
            }
            return s;
        }

        public static int AddStudy(string studyName, string owner)
        {
            int s = -1;
            using (var client = new WebClient())
            {
                NameValueCollection parameters = new NameValueCollection();

                parameters.Add("study", studyName);
                parameters.Add("owner", owner);

                try
                {
                    InitiateSSLTrust();
                    byte[] result = client.UploadValues(site + "scripts/InsertStudy.php", "POST", parameters);

                    s = Convert.ToInt32(System.Text.Encoding.UTF8.GetString(result, 0, result.Length));
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Server connection error.");
                }
            }
            return s;
        }

        public static int AddPlaylist(string playlistName)
        {
            int s = -1;
            using (var client = new WebClient())
            {
                NameValueCollection parameters = new NameValueCollection();

                parameters.Add("name", playlistName);

                try
                {

                    InitiateSSLTrust();
                    byte[] result = client.UploadValues(site + "scripts/InsertPlaylist.php", "POST", parameters);

                    s = Convert.ToInt32(System.Text.Encoding.UTF8.GetString(result, 0, result.Length));
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Server connection error.");
                }
            }
            return s;
        }

        public static int AddExpPlaylist(string playlistName,int expId)
        {
            int s = -1;
            using (var client = new WebClient())
            {
                NameValueCollection parameters = new NameValueCollection();

                parameters.Add("name", playlistName);
                parameters.Add("expId", expId.ToString());

                try
                {      InitiateSSLTrust();
                    byte[] result = client.UploadValues(site + "scripts/InsertExpPlaylist.php", "POST", parameters);

                    s = Convert.ToInt32(System.Text.Encoding.UTF8.GetString(result, 0, result.Length));
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Server connection error.");
                }
            }
            return s;
        }

        public static int AddResults(int expId, int subjectNum, string resultsFile)
        {
            int s = -1;
            using (var client = new WebClient())
            {
                NameValueCollection parameters = new NameValueCollection();

                parameters.Add("expId", expId.ToString());
                parameters.Add("subNum", subjectNum.ToString());
                parameters.Add("results", resultsFile);

                try
                {
                    InitiateSSLTrust();
                    byte[] result = client.UploadValues(site + "scripts/InsertResults.php", "POST", parameters);

                    s = Convert.ToInt32(System.Text.Encoding.UTF8.GetString(result, 0, result.Length));
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Server connection error.");
                }
            }
            return s;
        }

        //UPDATE DATA
        public static int UpdateUser(string username, string email, string password)
        {
            User a = SelectUser(username);
            

            int s = -1;
            using (var client = new WebClient())
            {
                NameValueCollection parameters = new NameValueCollection();

                parameters.Add("username", username);
                parameters.Add("email", email);
                if (a.Password == password)
                {
                    parameters.Add("password", a.Password);
                }
                else
                {
                    parameters.Add("password", password.GetHashCode().ToString());
                }

                try
                {
                    InitiateSSLTrust();
                    byte[] result = client.UploadValues(site + "scripts/UpdateUser.php", "POST", parameters);

                    s = Convert.ToInt32(System.Text.Encoding.UTF8.GetString(result, 0, result.Length));
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Server connection error.");
                }
            }
            return s;
        }

        //DATA USAGE
        public static int LoginUser(string username, string password)
        {
            int s = -1;
            using (var client = new WebClient())
            {
                NameValueCollection parameters = new NameValueCollection();

                parameters.Add("username", username);
                parameters.Add("password", password.GetHashCode().ToString());

                try
                {
                    InitiateSSLTrust();
                    byte[] result = client.UploadValues(site + "scripts/Login.php", "POST", parameters);

                    s = Convert.ToInt32(System.Text.Encoding.UTF8.GetString(result, 0, result.Length));
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Server connection error.");
                }
            }

            return s;
        }

        public static bool PlaylistExist(string playlist)
        {
            bool s = false;
            using (var client = new WebClient())
            {
                NameValueCollection parameters = new NameValueCollection();

                parameters.Add("playlist", playlist);

                try
                {
                    InitiateSSLTrust();
                    byte[] result = client.UploadValues(site + "scripts/PlaylistExist.php", "POST", parameters);

                    if (Convert.ToInt32(System.Text.Encoding.UTF8.GetString(result, 0, result.Length)) == 1)
                    {
                        s = true;
                    }
                    else if (Convert.ToInt32(System.Text.Encoding.UTF8.GetString(result, 0, result.Length)) == 2)
                    {
                        s = false;
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Server connection error.");
                }
            }

            return s;
        }

        public static bool UserExist(string username)
        {

            //DirectoryInfo dir = new DirectoryInfo(Application.dataPath + "/../Users/");
            //DirectoryInfo[] info = dir.GetDirectories();
            //for (int i = 0; i < info.Length; i++)
            //{
            //    if (username.ToLower() == info[i].Name.ToLower())
            //    {
            //        PlayerPrefs.SetString("CurrentUsername", username);
            //        return true;
            //    }
            ////}
            //string f = "../Users/" + PlayerPrefs.GetString("CurrentUsername") + "/Actions/things/" + "File.ext";
            //string paths = f.Substring(0, f.LastIndexOf("/")+1);
            //string file = f.Substring(f.LastIndexOf("/")+1);
            //Debug.Log(paths + " : " + file);
            
            bool s = false;
            using (var client = new WebClient())
            {
                NameValueCollection parameters = new NameValueCollection();

                parameters.Add("username", username);

                try
                {
                    InitiateSSLTrust();
                    byte[] result = client.UploadValues(site + "scripts/UserExist.php", "POST", parameters);

                    if (Convert.ToInt32(System.Text.Encoding.UTF8.GetString(result, 0, result.Length))==1)
                    {
                        s = true;
                    }
                    else if(Convert.ToInt32(System.Text.Encoding.UTF8.GetString(result, 0, result.Length)) == 2)
                    {
                        s = false;
                    }

                }
                catch (Exception ex)
                {
                    Console.WriteLine("Server connection error.");
                }
            }
            
            return s;
        }

        public static User SelectUser(string username)
        {
            User tmp = new User();

            using (var client = new WebClient())
            {
                NameValueCollection parameters = new NameValueCollection();

                parameters.Add("username", username);

                try
                {
                    InitiateSSLTrust();
                    string userStr = "";
                    byte[] result = client.UploadValues(site + "scripts/SelectUser.php", "POST", parameters);

                    userStr = Convert.ToString(System.Text.Encoding.UTF8.GetString(result, 0, result.Length));

                    string[] us = userStr.Split('#');

                    tmp = new User(us[0], us[1], us[2]);
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Server connection error.");
                }
            }

            return tmp;
        }

        public static void InitiateSSLTrust()
        {
            try
            {
                //Change SSL checks so that all checks pass
                ServicePointManager.ServerCertificateValidationCallback =
                   new RemoteCertificateValidationCallback(
                        delegate
                        { return true; }
                    );
            }
            catch (Exception ex)
            {
                //ActivityLog.InsertSyncActivity(ex);
            }
        }
    }
}
