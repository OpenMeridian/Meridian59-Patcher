using System.IO;

namespace ClientPatcher
{
    class ClassicClientPatcher : ClientPatcher
    {
        public ClassicClientPatcher(PatcherSettings settings)
            : base(settings)
        {
                
        }

        protected override bool IsNewClient()
        {
            return !File.Exists(CurrentProfile.ClientFolder + "\\meridian.exe");
        }

    }
}
