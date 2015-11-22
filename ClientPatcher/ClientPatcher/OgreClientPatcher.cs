using System;
using System.IO;
using PatchListGenerator;

namespace ClientPatcher
{
    class OgreClientPatcher : ClientPatcher
    {
        public OgreClientPatcher(PatcherSettings settings)
            : base(settings)
        {
                
        }

        protected override bool IsNewClient()
        {
            return !File.Exists(CurrentProfile.ClientFolder + "\\x86\\Meridian59.Ogre.Client.exe");
        }

        public override void GenerateCache()
        {
            string fullpath = CurrentProfile.ClientFolder;
            var scanner = new OgreClientScanner(fullpath);
            scanner.ScanSource();
            using (var sw = new StreamWriter(fullpath + CacheFile))
            {
                sw.Write(scanner.ToJson());
            }    
        }
    }
}
