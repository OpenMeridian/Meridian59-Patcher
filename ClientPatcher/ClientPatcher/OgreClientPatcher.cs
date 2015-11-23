using System;
using System.IO;
using System.Diagnostics;
using PatchListGenerator;

namespace ClientPatcher
{
    class OgreClientPatcher : ClientPatcher
    {
        public OgreClientPatcher(PatcherSettings settings)
            : base(settings)
        {
                
        }

        public override bool IsNewClient()
        {
            return !File.Exists(CurrentProfile.ClientFolder + "\\x86\\Meridian59.Ogre.Client.exe");
        }

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
    }
}
