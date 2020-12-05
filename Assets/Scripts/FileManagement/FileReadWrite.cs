using Microsoft.Win32;
using System;
using System.Diagnostics;
using System.IO;

namespace yipli.Windows
{
    public static class FileReadWrite
    {
        static string myDocLoc = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
        static readonly string yipliFolder = "Yipli";
        static readonly string yipliFile = "userinfo.txt";

        static readonly string yipliAppDownloadUrl = "https://www.playyipli.com/download.html";

        static RegistryKey rk = Registry.CurrentUser;

        public static string ReadFromFile()
        {
            try
            {
                string linedata = null;

                // Create an instance of StreamReader to read from a file.
                // The using statement also closes the StreamReader.
                if (File.Exists(myDocLoc + "/Yipli/userinfo.txt"))
                {
                    using (StreamReader sr = new StreamReader(myDocLoc + "/Yipli/userinfo.txt"))
                    {
                        string line;

                        // Read and display lines from the file until 
                        // the end of the file is reached. 
                        while ((line = sr.ReadLine()) != null)
                        {
                            linedata = line.Substring(17);

                            //UnityEngine.Debug.LogError("received line : " + linedata);
                        }
                    }

                    return linedata;
                }
                else
                {
                    return null;
                }
            }
            catch (Exception e)
            {
                // Let the user know what went wrong.
                UnityEngine.Debug.LogError("Reading Failed : " + e.Message);

                return null;
            }
        }

        public static void WriteToFile(string userID)
        {
            if (!Directory.Exists(myDocLoc + "/" + yipliFolder))
            {
                Directory.CreateDirectory(myDocLoc + "/" + yipliFolder);
                var yipliFileToCreate = File.Create(myDocLoc + "/" + yipliFolder + "/" + yipliFile);
                yipliFileToCreate.Close();
            }

            StreamWriter sw = new StreamWriter(myDocLoc + "/Yipli/userinfo.txt");
            sw.WriteLine("Current UserID : " + userID);
            sw.Close();

            //ReadFromFile();
        }

        public static void OpenYipliApp()
        {
            string yipliAppExeLoc = GetApplictionInstallPath("yipliapp") + "\\" + "YipliApp.exe";

            if (ValidateFile(yipliAppExeLoc))
            {
                Process.Start(yipliAppExeLoc);
            }
            else
            {
                Process.Start(yipliAppDownloadUrl);
            }

            //UnityEngine.Debug.LogError("Application is switched");
            UnityEngine.Application.Quit();
        }

        public static string GetApplictionInstallPath(string gameName)
        {
            string installPath = null;

            RegistryKey subKey = rk.OpenSubKey(gameName);

            try
            {
                installPath = subKey.GetValue("InstallPath").ToString();
                //UnityEngine.Debug.LogError("sub key : " + subKey.GetValue("InstallPath"));
            }
            catch (Exception e)
            {
                UnityEngine.Debug.LogError("sub key not found. Error : " + e.Message);
            }

            return installPath;
        }

        public static bool ValidateFile(string fileLocation)
        {
            return File.Exists(fileLocation);
        }
    }
}
