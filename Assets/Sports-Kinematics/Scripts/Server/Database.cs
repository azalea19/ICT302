using System;
using System.Net;
using System.Collections.Specialized;
using System.IO;

namespace SportsKinematics.Server
{
    public static class Database
    {
        static string site = "http://localhost/302test/";
        //static string site = "http://10.1.1.226/302test/";


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
                    byte[] result = client.UploadValues(site + "scripts/InsertUser.php", "POST", parameters);

                    s = Convert.ToInt32(System.Text.Encoding.UTF8.GetString(result, 0, result.Length));
                }
                catch (WebException ex)
                {
                    Console.WriteLine("Server connection error.");
                }
            }
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
                {
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
            int s = -1;
            using (var client = new WebClient())
            {
                NameValueCollection parameters = new NameValueCollection();

                parameters.Add("username", username);
                parameters.Add("email", email);
                parameters.Add("password", password.GetHashCode().ToString());

                try
                {
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
            bool s = false;
            using (var client = new WebClient())
            {
                NameValueCollection parameters = new NameValueCollection();

                parameters.Add("username", username);

                try
                {
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
    }
}
