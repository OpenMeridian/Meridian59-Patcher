using System;
using System.IO;
using System.Diagnostics;
using PatchListGenerator;
using Microsoft.Win32;

namespace ClientPatcher
{
    class OgreClientPatcher : ClientPatcher
    {
        #region Constructors
        private string NETDownloadUri = "https://www.microsoft.com/en-US/download/details.aspx?id=48130";
        private string NETDownloadMsg = "The Ogre client requires .NET version 4.5.1 or greater, please install this to continue.";

        public OgreClientPatcher(PatcherSettings settings)
            : base(settings)
        {
        }
        #endregion

        #region Client
        public override bool IsNewClient()
        {
            return !File.Exists(CurrentProfile.ClientFolder + "\\x86\\Meridian59.Ogre.Client.exe");
        }

        public override void Launch()
        {
            var meridian = new ProcessStartInfo
            {
                FileName = CurrentProfile.ClientFolder + "\\x86\\Meridian59.Ogre.Client.exe",
                WorkingDirectory = CurrentProfile.ClientFolder + "\\x86\\",

                //TODO: add ability to enter username and password during patching
                //meridian.Arguments = "/U:username /P:password /H:host";
            };

            Process.Start(meridian);
        }
        #endregion

        #region Cache
        public override void GenerateCache()
        {
            string fullpath = CurrentProfile.ClientFolder;
            var scanner = new OgreClientScanner(fullpath);
            scanner.ScannerSetup(fullpath);
            scanner.ScanSource();
            using (var sw = new StreamWriter(fullpath + CacheFile))
            {
                sw.Write(scanner.ToJson());
            }    
        }
        #endregion

        #region Dependencies
        public override bool CheckDependencies()
        {
            using (RegistryKey ndpKey = RegistryKey.OpenBaseKey(RegistryHive.LocalMachine,
                RegistryView.Registry32).OpenSubKey("SOFTWARE\\Microsoft\\NET Framework Setup\\NDP\\v4\\Full\\"))
            {
                int releaseKey = Convert.ToInt32(ndpKey.GetValue("Release"));
                // If a user has a .NET version lower than 4.5, send them to download it.
                // Release key value from https://msdn.microsoft.com/en-us/library/hh925568%28v=vs.110%29.aspx#net_d
                if (releaseKey < 378675)
                {
                    OnFailedDependency(new FailedDependencyEventArgs(NETDownloadUri, NETDownloadMsg));

                    return false;
                }

                return true;
            }
        }
        #endregion
    }
}
