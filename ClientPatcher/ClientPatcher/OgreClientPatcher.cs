using System.IO;

namespace ClientPatcher
{
    class OgreClientPatcher : ClientPatcher
    {
        private bool IsNewClient()
        {
            return !File.Exists(CurrentProfile.ClientFolder + "\\x86\\Meridian59.Ogre.Client.exe");
        }

    }
}
