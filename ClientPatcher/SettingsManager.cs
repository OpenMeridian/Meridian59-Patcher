using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Security.AccessControl;
using System.Security.Principal;
using Newtonsoft.Json;
using PatchListGenerator;

namespace ClientPatcher
{
    class SettingsManager
    {
        // Where to download latest setting from.
        private const string SettingsUrl = "http://ww1.openmeridian.org/settings.php";
        private const string UserAgentString = "Mozilla/4.0 (compatible; .NET CLR 4.0.;) OpenMeridianPatcher v1.4";


        private readonly string _settingsPath; // Path to JSON file settings.txt.
        private readonly string _settingsFile;

        // Loaded from settings.txt, or generated on first run and then saved.
        public List<PatcherSettings> Servers { get; set; }

        public SettingsManager()
        {
            _settingsPath = "C:\\Program Files\\Open Meridian";
            _settingsFile = "\\settings.txt";
        }

        #region SettingsList
        /// <summary>
        /// Load settings.txt, also check web settings.
        /// </summary>
        public void Refresh()
        {
            LoadSettings();
            MergeWebSettings(GetNewSettings());
            SaveSettings();
        }

        /// <summary>
        /// Downloads the list of available servers from the open meridian
        /// web server using JSON encoded PatcherSettings objects.
        /// </summary>
        public List<PatcherSettings> GetNewSettings()
        {
            try
            {
                var myClient = new WebClient();
                myClient.Headers.Add("user-agent", UserAgentString);
                // Download the settings from the web, store them in a list.
                var webSettingsList =
                    JsonConvert.DeserializeObject<List<PatcherSettings>>(myClient.DownloadString(SettingsUrl)); 
                return webSettingsList;
                
            }
            catch (Exception e)
            {
                throw new InvalidOperationException("Unable to download settings from server." + e);
            }
        }

        /// <summary>
        /// Merges a list of PatcherSettings objects with the currently loaded settings
        /// </summary>
        /// <param name="webSettingsList">List of PatcherSetting objects to merge</param>
        public void MergeWebSettings(List<PatcherSettings> webSettingsList)
        {
            if (Servers == null)
            {
                Servers = webSettingsList;
                return;
            }

            // Loop through loaded settings from settings.txt.
            foreach (PatcherSettings webProfile in webSettingsList)
            {
                // Find the matching local profile by Guid.
                PatcherSettings localProfile = Servers.FirstOrDefault(i => i.Guid == webProfile.Guid);

                // If a local match, update, else, add a new local profile.
                if (localProfile != null)
                {
                    if (!webProfile.DeleteProfile)
                    {
                        // Don't update all fields on purpose. user retains control of paths, which client to use.
                        UpdateProfile(localProfile, webProfile);
                    }
                    else
                    {
                        // Remove from the list of servers (will also not be in saved list).
                        Servers.Remove(localProfile);
                    }
                }
                else
                {
                    // Don't add a server that is supposed to be deleted.
                    if (!webProfile.DeleteProfile)
                        Servers.Add(webProfile);
                }
            }

            // Handle the transition from older profiles with null Guid to newer ones as
            // the old profiles will be deleted. If a user is still updating the old
            // profile, it is likely it will be the default. Search profiles and if a
            // null Guid is found in a profile matching a newer entry, and the old
            // profile is the default, put the old profile's path in the new profile
            // and set that one to default.
            foreach (PatcherSettings server in Servers)
            {
                if (server.Guid == null && server.Default)
                {
                    foreach (PatcherSettings serverMatch in Servers)
                    {
                        // Same PatchInfoUrl, one null Guid and one not. Use the path from
                        // the old profile, set the new one to default.
                        if (serverMatch.Guid != null && server.PatchInfoUrl == serverMatch.PatchInfoUrl)
                        {
                            serverMatch.ClientFolder = server.ClientFolder;
                            server.Default = false;
                            serverMatch.Default = true;
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Load the settings.txt file and use json deserialization
        /// to create an array of PatcherSettings objects.
        /// Otherwise, download the settings from the web.
        /// </summary>
        public void LoadSettings()
        {
            if (File.Exists(_settingsPath + _settingsFile))
            {
                StreamReader file = File.OpenText(_settingsPath+_settingsFile); //Open the file

                Servers = JsonConvert.DeserializeObject<List<PatcherSettings>>(file.ReadToEnd()); //convert
                file.Close(); //close
            }
            else
            {
                Servers = GetNewSettings();
                GrantAccess();
            }
            Servers.Sort((x, y) => x.ServerNumber.CompareTo(y.ServerNumber));
        }

        /// <summary>
        /// JSON Serialize our PatcherSettings and write them to settings.txt
        /// </summary>
        public void SaveSettings()
        {
            List<PatcherSettings> ServerSaveList = new List<PatcherSettings>();

            foreach (PatcherSettings server in Servers)
            {
                if (server.SaveProfile)
                {
                    ServerSaveList.Add(server);
                }
            }

            try
            {
                using (var sw = new StreamWriter(_settingsPath + _settingsFile)) //open file
                {
                    sw.Write(JsonConvert.SerializeObject(ServerSaveList, Formatting.Indented)); //write shit
                }
            }
            catch (Exception e)
            {
                
                throw new Exception("Unable to SaveSettings()" + e);
            }
            
        }
        #endregion

        #region Profiles
        /// <summary>
        /// Add a profile to the Servers list, used by the add form.
        /// </summary>
        public void AddProfile(string clientfolder, string patchbaseurl, string patchinfourl,
                               string fullinstallurl, string servername, int servernumber,
                               bool isdefault = false, ClientType clientType = ClientType.Classic)
        {
            var ps = new PatcherSettings
            {
                ClientFolder = clientfolder,
                PatchBaseUrl = patchbaseurl,
                PatchInfoUrl = patchinfourl,
                FullInstallUrl = fullinstallurl,
                ServerName = servername,
                ServerNumber = servernumber,
                Default = isdefault,
                ClientType = clientType,
                Enabled = true,
                SaveProfile = true,
                DeleteProfile = false
            };

            if (isdefault)
            {
                foreach (PatcherSettings patcherSettingse in Servers.FindAll(s => s.Default))
                {
                    patcherSettingse.Default = false;
                }
            }
            Servers.Add(ps);
            SaveSettings();
            LoadSettings();
        }

        /// <summary>
        /// Add a profile to the Servers list.
        /// </summary>
        public void AddProfile(PatcherSettings newprofile)
        {
            if (newprofile.Default)
            {
                foreach (PatcherSettings patcherSettingse in Servers.FindAll(s => s.Default))
                {
                    patcherSettingse.Default = false;
                }
            }
            Servers.Add(newprofile);
            SaveSettings();
            LoadSettings();
        }

        /// <summary>
        /// Removes a profile from the Servers list by server name.
        /// </summary>
        public void RemoveProfileByName(string name)
        {
            Servers.RemoveAt(Servers.FindIndex(x => x.ServerName == name));
            SaveSettings();
            LoadSettings();
        }

        /// <summary>
        /// Update a local profile data fields from a web profile.
        /// Does not update path or Guid.
        /// </summary>
        private void UpdateProfile(PatcherSettings localProfile, PatcherSettings webProfile)
        {
            localProfile.PatchBaseUrl = webProfile.PatchBaseUrl;
            localProfile.PatchInfoUrl = webProfile.PatchInfoUrl;
            localProfile.ServerName = webProfile.ServerName;
            localProfile.ServerNumber = webProfile.ServerNumber;
            localProfile.FullInstallUrl = webProfile.FullInstallUrl;
            localProfile.AccountCreationUrl = webProfile.AccountCreationUrl;
            localProfile.Enabled = webProfile.Enabled;
            localProfile.SaveProfile = webProfile.SaveProfile;
            localProfile.DeleteProfile = webProfile.DeleteProfile;
        }
        #endregion

        #region Find
        public PatcherSettings FindByName(string name)
        {
            return Servers.Find(x => x.ServerName == name);
        }

        public PatcherSettings FindByNumber(int number)
        {
            return Servers.Find(x => x.ServerNumber == number);
        }

        /// <summary>
        /// Get default profile. If there isn't one for some reason,
        /// set the first profile to default and return it.
        /// </summary>
        public PatcherSettings GetDefault()
        {
            PatcherSettings ps = Servers.Find(x => x.Default);
            if (ps == null)
            {
                ps = Servers.First();
                ps.Default = true;
            }

            return ps;
        }
        #endregion

        #region Util
        /// <summary>
        /// Sets proper NTFS permissions for the patcher to operate
        /// </summary>
        private void GrantAccess()
        {
            try
            {
                var dSecurity = new DirectorySecurity();
                dSecurity.AddAccessRule(new FileSystemAccessRule(new SecurityIdentifier(WellKnownSidType.WorldSid, null), FileSystemRights.Modify | FileSystemRights.Synchronize,
                                                                          InheritanceFlags.ContainerInherit | InheritanceFlags.ObjectInherit,
                                                                          PropagationFlags.None, AccessControlType.Allow));
                dSecurity.SetAccessRuleProtection(false, true);
                Directory.CreateDirectory(_settingsPath, dSecurity);
            }
            catch (Exception e)
            {
                
                throw new Exception("Unable to GrantAccess()" + e);
            }

        }
        #endregion
    }
}
