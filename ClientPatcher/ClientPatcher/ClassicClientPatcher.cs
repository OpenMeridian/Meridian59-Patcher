using System.IO;
using System.Diagnostics;
using PatchListGenerator;

namespace ClientPatcher
{
    class ClassicClientPatcher : ClientPatcher
    {
        #region Constructors
        public ClassicClientPatcher(PatcherSettings settings)
            : base(settings)
        {
        }
        #endregion

        #region Client
        public override bool IsNewClient()
        {
            return !Directory.Exists(CurrentProfile.ClientFolder + "\\resource\\");
        }

        public override void Launch()
        {
            var meridian = new ProcessStartInfo
            {
                FileName = CurrentProfile.ClientFolder + "\\meridian.exe",
                WorkingDirectory = CurrentProfile.ClientFolder + "\\",

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
            var scanner = new ClassicClientScanner(fullpath);
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
            // No dependencies.
            return true;
        }
        #endregion
    }
}
