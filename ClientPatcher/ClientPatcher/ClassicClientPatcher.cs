using System.IO;

namespace ClientPatcher
{
    class ClassicClientPatcher : ClientPatcher
    {
        private bool IsNewClient()
        {
            return !File.Exists(CurrentProfile.ClientFolder + "\\meridian.exe");
        }

    }
}
