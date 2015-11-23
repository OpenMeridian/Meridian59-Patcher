using System.IO;
using PatchListGenerator;

namespace ClientPatcher
{
    class ClassicClientPatcher : ClientPatcher
    {
        public ClassicClientPatcher(PatcherSettings settings)
            : base(settings)
        {
                
        }

        public override bool IsNewClient()
        {
            return !File.Exists(CurrentProfile.ClientFolder + "\\meridian.exe");
        }

        public override void GenerateCache()
        {
            string fullpath = CurrentProfile.ClientFolder;
            var scanner = new ClassicClientScanner(fullpath);
            scanner.ScannerSetup(fullpath);
            scanner.ScanSource();
            using (var sw = new StreamWriter(fullpath + CacheFile))
            {
                sw.Write(scanner.ToJson());
            }    
        }
    }
}
