using System;
using System.Net;
using System.Collections.Specialized;
using System.IO;

namespace BPTracking.Server
{
    class Files
    {
        //static string site = "http://localhost/ICT302_Test/";
        static string site = "http://10.1.1.226/302test/";

        public static void SaveResults()
        {

        }

        public static int SaveCapture(string study, string filename)
        {
            int s = -1;
            string path = "./recordings/raw/";
            using (var client = new WebClient())
            {
                NameValueCollection parameters = new NameValueCollection();
                //Console.WriteLine(path + filename);
                parameters.Add("filepath", path + study);
                parameters.Add("filename", filename);

                client.Headers.Add("Content-Type", "binary/octet-stream");
                client.QueryString = parameters;
                try
                {
                    byte[] result = client.UploadFile(site + "scripts/Upload.php", "POST", filename);
                    s = Convert.ToInt32(System.Text.Encoding.UTF8.GetString(result, 0, result.Length));
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex + ": Server connection error.");
                }
            }

            return s;
        }

        public static int SaveExperiment(string study, int experimentNum)
        {
            int s = -1;
            string path = "./recordings/";
            string file = experimentNum + ".bvh";
            using (var client = new WebClient())
            {
                NameValueCollection parameters = new NameValueCollection();
                //Console.WriteLine(path + filename);
                parameters.Add("filepath", path + study);
                parameters.Add("filename", file);

                client.Headers.Add("Content-Type", "binary/octet-stream");
                client.QueryString = parameters;
                try
                {
                    byte[] result = client.UploadFile(site + "scripts/Upload.php", "POST", file);
                    s = Convert.ToInt32(System.Text.Encoding.UTF8.GetString(result, 0, result.Length));
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex + ": Server connection error.");
                }
            }

            return s;
        }

        public static void LoadCapture(string study, string captureName)
        {
            using (var client = new WebClient())
            {
                string path = site + "/recordings/raw/" + study + "/" + captureName;
                string localPath = "./recordings/raw/" + study;
                string file = captureName + ".bvh";
                if (!Directory.Exists(localPath))
                {
                    Directory.CreateDirectory(localPath);
                }
                client.DownloadFile(path, localPath + "/" + file);
            }
        }

        public static void LoadExperiment(string study, string captureNum)
        {
            using (var client = new WebClient())
            {
                string path = site + "/recordings/raw/" + study + "/" + captureNum;
                string localPath = "./recordings/raw/" + study;
                string file = captureNum + ".bvh";
                if (!Directory.Exists(localPath))
                {
                    Directory.CreateDirectory(localPath);
                }
                client.DownloadFile(path, localPath + "/" + file);
            }
        }
    }
}
