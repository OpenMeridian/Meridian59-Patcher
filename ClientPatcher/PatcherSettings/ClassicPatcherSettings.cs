using PatchListGenerator;

namespace ClientPatcher
{
    class ClassicPatcherSettings : PatcherSettings
    {
        public ClassicPatcherSettings()
            : base()
        {
            ServerName = "103 - Open Meridian Official (Classic)";
            ServerNumber = 103;
            PatchInfoUrl = "http://ww1.openmeridian.org/103/patchinfo.txt";
            FullInstallUrl = "http://ww1.openmeridian.org/103/Meridian59.Client.Classic.zip";
            ClientFolder = "C:\\Program Files\\Open Meridian\\Meridian 103";
            PatchBaseUrl = "http://ww1.openmeridian.org/103/clientpatch";
            AccountCreationUrl = "http://ww1.openmeridian.org/103/acctcreate.php";
            Guid = "5AD1FB01-A84A-47D1-85B8-5F85FB0C201E";
            Default = true;
            ClientType = ClientType.Classic;
        }
    }
}
