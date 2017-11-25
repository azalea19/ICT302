using System;
using System.Net;
using System.Collections.Specialized;
using System.IO;

namespace SportsKinematics.Server
{
    class Files
    {
        //static string site = "http://localhost/302test/";
        static string site = "http://10.1.1.226/302test/";

        public static string UploadFile(string frompath,string topath,string filename)
        {
            string s = "";
            //string path = "./recordings/raw/";
            using (var client = new WebClient())
            {
                NameValueCollection parameters = new NameValueCollection();
                //Console.WriteLine(path + filename);
                parameters.Add("filepath", topath);
                parameters.Add("filename", filename);

                client.Headers.Add("Content-Type", "binary/octet-stream");
                client.QueryString = parameters;
                try
                {
                    byte[] result = client.UploadFile(site + "scripts/Upload.php", "POST", frompath + filename);

                    string k = Convert.ToString(System.Text.Encoding.UTF8.GetString(result, 0, result.Length));
                    //print(k);

                    //s = Convert.ToInt32(System.Text.Encoding.UTF8.GetString(result, 0, result.Length));
                    s = Convert.ToString(System.Text.Encoding.UTF8.GetString(result, 0, result.Length));

                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex + ": Server connection error.");
                }
            }

            return s;
        }
    }
}
