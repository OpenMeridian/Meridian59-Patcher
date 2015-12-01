using Newtonsoft.Json;
using PatchListGenerator;

namespace ClientPatcher
{
    /// <summary>
    /// A set of settings about a specific Meridian 59 Server
    /// </summary>
    public class PatcherSettings
    {
        public string ServerName { get; set; }     //What do we call this profile?
        public int ServerNumber { get; set; }      // Server number.
        public string PatchInfoUrl { get; set; }   //Where is the file containing md5 hashes to compare?
        public string ClientFolder { get; set; }   //Where is the local copy of the client?
        public string PatchBaseUrl { get; set; }   //Where to download individual files?
        public string Guid { get; set; }           //Unique ID for a server profile (so they can be renamed by end users)
        public string FullInstallUrl { get; set; } //Path to a .zip file of the full client to download for first run
        public string AccountCreationUrl { get; set; } //URL to load when "Create Account" button is clicked.
        public bool Default { get; set; }          //Is this profile the default-selected at start up?
        public ClientType ClientType { get; set; } //Which client does this profile use?
        public bool Enabled { get; set; }          // show server in available profiles
        public bool SaveProfile { get; set; }      // save profile to settings.txt
        public bool DeleteProfile { get; set; }    // delete profile from settings.txt

        public PatcherSettings()
        {

        }

        public PatcherSettings(string servername, int servernumber, string patchinfourl,
                               string clientfolder, string patchbaseurl, string fullinstallurl,
                               bool defaultserver = false, ClientType clientType = ClientType.Classic,
                               bool enabled = true, bool saveProfile = true, bool deleteProfile = false)
        {
            ServerName = servername;
            ServerNumber = servernumber;
            PatchInfoUrl = patchinfourl;
            ClientFolder = clientfolder;
            PatchBaseUrl = patchbaseurl;
            FullInstallUrl = fullinstallurl;
            Default = defaultserver;
            ClientType = clientType;
            Enabled = enabled;
            SaveProfile = saveProfile;
            DeleteProfile = deleteProfile;
        }

        public string ToJson()
        {
            return JsonConvert.SerializeObject(this, Formatting.Indented);
        }
    }
}
