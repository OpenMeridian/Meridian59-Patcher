using System;
using System.IO;

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

    }
}
