using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PatchListGenerator;

namespace ClientPatcher
{
    class OgrePatcherSettings : PatcherSettings
    {
        OgrePatcherSettings() : base()
        {
            ServerName = "103 - Open Meridian Official (Ogre)";
            PatchInfoUrl = "http://ww1.openmeridian.org/103/ogrepatchinfo.txt";
            FullInstallUrl = "http://ww1.openmeridian.org/103/Meridian59.Ogre.Classic.zip";
            ClientFolder = "C:\\Program Files\\Open Meridian\\Meridian 103";
            PatchBaseUrl = "http://ww1.openmeridian.org/103/ogre";
            AccountCreationUrl = "http://ww1.openmeridian.org/103/acctcreate.php";
            Guid = "D687C449-7F3B-4A2E-A1E5-3D40D4FE8F43";
            Default = false;
            ClientType = ClientType.Classic;
        }
    }
}
