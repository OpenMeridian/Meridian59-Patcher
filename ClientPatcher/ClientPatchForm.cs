using System;
using System.ComponentModel;
using System.Deployment.Application;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Windows.Forms;
using Awesomium.Core;
using Awesomium.Windows.Forms;
using PatchListGenerator;
using DownloadProgressChangedEventArgs = System.Net.DownloadProgressChangedEventArgs;

namespace ClientPatcher
{
    enum ChangeType
    {
        None = 0,
        AddProfile = 1,
        ModProfile = 2
    }

    public partial class ClientPatchForm : Form
    {
        SettingsManager _settings;
        ClientPatcher _patcher;
        ChangeType _changetype = ChangeType.None;

        public bool showFileNames = false;

        public ClientPatchForm()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            // Create Patcher shortcut on user's desktop if not already present.
            CheckForShortcut();

            btnPlay.Enabled = false;

            // Load current settings.txt. TODO: Refresh() seems to do all 3 of these things.
            _settings = new SettingsManager();
            _settings.LoadSettings();
            _settings.Refresh();
            _settings.SaveSettings();

            // Get the default profile settings.
            PatcherSettings ps = _settings.GetDefault();

            // Load the type of client patcher we need for scanning the default profile.
            ClientType ct = ps.ClientType;
            switch (ct)
            {
                case ClientType.Classic:
                    _patcher = new ClassicClientPatcher(ps);
                    break;
                case ClientType.DotNet:
                    _patcher = new OgreClientPatcher(ps);
                    break;
            }

            // Add event handlers.
            _patcher.FileScanned += Patcher_FileScanned;
            _patcher.StartedDownload += Patcher_StartedDownload;
            _patcher.ProgressedDownload += Patcher_ProgressedDownload;
            _patcher.EndedDownload += Patcher_EndedDownload;
            _patcher.FailedDownload += Patcher_FailedDownload;
            _patcher.StartedUnzip += Patcher_StartedUnzip;
            _patcher.ProgressedUnzip += Patcher_ProgressedUnzip;
            _patcher.EndedUnzip += Patcher_EndedUnzip;

            // Set the create account button to display "create account" text.
            SetCreateAccountText(_patcher.CurrentProfile);

            if (ApplicationDeployment.IsNetworkDeployed)
            {
                Version myVersion = ApplicationDeployment.CurrentDeployment.CurrentVersion;
                Text += string.Concat("- Version: v", myVersion);
            }

            // Make sure the default is our current selection, and the data is displayed in options.
            RefreshDdl();
            SetPatcherProfile(ps);
        }

        /// <summary>
        /// Launches the Meridian client for the selected profile.
        /// </summary>
        private void LaunchProfile()
        {
            _patcher.Launch();
            Application.Exit();
            webControl.Dispose();
            Environment.Exit(1);
        }

        #region MainButtonClicked
        private void btnPlay_Click(object sender, EventArgs e)
        {
            LaunchProfile();
        }
        private void btnPatch_Click(object sender, EventArgs e)
        {
            PatchProfile();
        }
        #endregion

        #region OptionsButtonClicked
        /// <summary>
        /// Add a new profile.
        /// </summary>
        private void btnAdd_Click(object sender, EventArgs e)
        {
            if (_changetype == ChangeType.AddProfile)
                return;
            groupProfileSettings.Enabled = true;

            // Clear data fields, if not already adding
            SetProfileDataFields(null);
            _changetype = ChangeType.AddProfile;
        }

        /// <summary>
        /// Modify the current profile. TODO: Moving out of options or clicking out of modify
        /// should ask if you want to save or discard any changes.
        /// </summary>
        private void btnStartModify_Click(object sender, EventArgs e)
        {
            groupProfileSettings.Enabled = true;
            PatcherSettings ps = _settings.FindByName(ddlServer.SelectedItem.ToString());
            txtClientFolder.Text = ps.ClientFolder;
            txtPatchBaseURL.Text = ps.PatchBaseUrl;
            txtPatchInfoURL.Text = ps.PatchInfoUrl;
            txtFullInstallURL.Text = ps.FullInstallUrl;
            txtServerName.Text = ps.ServerName;
            cbDefaultServer.Checked = ps.Default;

            _changetype = ChangeType.ModProfile;
        }

        /// <summary>
        /// Save the current profile TODO: split this off so it can be called separately.
        /// </summary>
        private void btnSave_Click(object sender, EventArgs e)
        {
            switch (_changetype)
            {
                case ChangeType.AddProfile:
                    ClientType clientType = ClientType.Classic;

                    _settings.AddProfile(txtClientFolder.Text, txtPatchBaseURL.Text, txtPatchInfoURL.Text, txtFullInstallURL.Text, txtServerName.Text, cbDefaultServer.Checked, clientType);
                    groupProfileSettings.Enabled = false;
                    break;
                case ChangeType.ModProfile:
                    ModProfile();
                    groupProfileSettings.Enabled = false;
                    break;
            }
            groupProfileSettings.Enabled = false;
            _changetype = ChangeType.None;
        }

        /// <summary>
        /// Delete a profile. Loads the default profile afterwards.
        /// </summary>
        private void btnRemove_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show("Are you sure you want to delete this Profile?", "Delete Profile?", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
            {
                int selected = _settings.Servers.FindIndex(x => x.ServerName == ddlServer.SelectedItem.ToString());
                _settings.Servers.RemoveAt(selected);
                _settings.SaveSettings();
                _settings.LoadSettings();
                SetPatcherProfile(_settings.GetDefault());
            }
        }

        /// <summary>
        /// Browse for a folder location.
        /// </summary>
        private void btnBrowse_Click(object sender, EventArgs e)
        {
            var fbd = new FolderBrowserDialog();
            fbd.ShowDialog(this);
            txtClientFolder.Text = fbd.SelectedPath;
        }

        /// <summary>
        /// Clear the cache, close Meridian and start a scan. TODO: should call StartScan() instead of PatchProfile()?
        /// </summary>
        private void btnCacheGen_Click(object sender, EventArgs e)
        {
            TxtLogAppendText("Generating Cache of local files, this may take a while..\r\n");
            groupProfileSettings.Enabled = false;
            _changetype = ChangeType.None;
            tabControl1.SelectTab(1);
            _patcher.GenerateCache();
            PatchProfile();
            TxtLogAppendText("Caching Complete!\r\n");
        }
        #endregion

        #region Profiles
        /// <summary>
        /// Handles user changing which profile is selected.
        /// </summary>
        private void ddlServer_SelectionChangeCommitted(object sender, EventArgs e)
        {
            //lblStatus.Text = ddlServer.SelectedItem.ToString();
            PatcherSettings selected = _settings.FindByName(ddlServer.SelectedItem.ToString());
            SetPatcherProfile(selected);
        }

        /// <summary>
        /// Selected a new profile.
        /// </summary>
        private void SetPatcherProfile(PatcherSettings profile)
        {
            // Don't set to null profile.
            if (profile == null)
                return;

            _patcher.CurrentProfile = profile;
            txtLog.Text += String.Format("Server {0} selected. Client located at: {1}\r\n", profile.ServerName, profile.ClientFolder);
            btnPlay.Enabled = false;

            // Set create account text.
            SetCreateAccountText(_patcher.CurrentProfile);
            _creatingAccount = false;

            //if (groupProfileSettings.Enabled != true) return;
            groupProfileSettings.Enabled = false;
            // Set the Options tab data fields to the current selection.
            SetProfileDataFields(profile);

            // Load the type of client patcher we need for scanning the default profile.
            ClientType ct = profile.ClientType;
            switch (ct)
            {
                case ClientType.Classic:
                    _patcher = new ClassicClientPatcher(profile);
                    break;
                case ClientType.DotNet:
                    _patcher = new OgreClientPatcher(profile);
                    break;
            }

            // Add event handlers.
            _patcher.FileScanned += Patcher_FileScanned;
            _patcher.StartedDownload += Patcher_StartedDownload;
            _patcher.ProgressedDownload += Patcher_ProgressedDownload;
            _patcher.EndedDownload += Patcher_EndedDownload;
            _patcher.FailedDownload += Patcher_FailedDownload;
            _patcher.StartedUnzip += Patcher_StartedUnzip;
            _patcher.ProgressedUnzip += Patcher_ProgressedUnzip;
            _patcher.EndedUnzip += Patcher_EndedUnzip;
        }

        /// <summary>
        /// Fills out the Options tab data fields when a new profile is selected.
        /// </summary>
        private void SetProfileDataFields(PatcherSettings profile)
        {
            if (profile == null)
            {
                txtClientFolder.Text = "";
                txtPatchBaseURL.Text = "";
                txtPatchInfoURL.Text = "";
                txtFullInstallURL.Text = "";
                txtServerName.Text = "";
            }
            else
            {
                txtClientFolder.Text = profile.ClientFolder;
                txtPatchBaseURL.Text = profile.PatchBaseUrl;
                txtPatchInfoURL.Text = profile.PatchInfoUrl;
                txtFullInstallURL.Text = profile.FullInstallUrl;
                txtServerName.Text = profile.ServerName;
                cbDefaultServer.Checked = profile.Default;
            }
        }

        /// <summary>
        /// Allows modifying the current profile selection.
        /// </summary>
        private void ModProfile()
        {
            int selected = _settings.Servers.FindIndex(x => x.ServerName == ddlServer.SelectedItem.ToString());
            _settings.Servers[selected].ClientFolder = txtClientFolder.Text;
            _settings.Servers[selected].PatchBaseUrl = txtPatchBaseURL.Text;
            _settings.Servers[selected].PatchInfoUrl = txtPatchInfoURL.Text;
            _settings.Servers[selected].FullInstallUrl = txtFullInstallURL.Text;
            _settings.Servers[selected].ServerName = txtServerName.Text;
            _settings.Servers[selected].Default = cbDefaultServer.Checked;

            _changetype = ChangeType.None;
            _settings.SaveSettings();
            _settings.LoadSettings();
        }

        /// <summary>
        /// Refreshes the profile selection to default.
        /// </summary>
        private void RefreshDdl()
        {
            foreach (PatcherSettings profile in _settings.Servers)
            {
                ddlServer.Items.Add(profile.ServerName);
                if (profile.Default)
                    ddlServer.SelectedItem = profile.ServerName;
            }
        }
        #endregion

        #region Scanning
        private void PatchProfile()
        {
            CheckMeridianRunning();
            StartScan();
        }

        private void Patcher_FileScanned(object sender, ScanEventArgs e)
        {
            PbProgressPerformStep();
            if (showFileNames)
                TxtLogAppendText(String.Format("Scanning Files.... {0}\r\n", e.Filename));
        }

        private void StartScan()
        {
            btnPatch.Enabled = false;
            ddlServer.Enabled = false;

            txtLog.AppendText("Downloading Patch Information....\r\n");
            if (_patcher.DownloadPatchDefinition() == 1)
            {
                if (_patcher.IsNewClient())
                {
                    // show download progress bar, we are downloading the full zip
                    pbFileProgress.Visible = true;
                }

                pbProgress.Value = 0;
                pbProgress.Maximum = _patcher.PatchFiles.Count;
                if (_patcher.HasCache())
                {
                    TxtLogAppendText("Using local cache....\r\n");
                    showFileNames = false;
                    _patcher.LoadCache();
                    _patcher.CompareCache();
                    PostScan();
                }
                else
                {
                    TxtLogAppendText("Scanning local files...\r\n");
                    showFileNames = true;
                    bgScanWorker.RunWorkerAsync(_patcher);
                }
            }
            else
            {
                txtLog.AppendText("ERROR: Unable to download Patch Information! Please try again later or raise an issue at openmeridian.org/forums/\r\n");
                btnPatch.Enabled = true;
            }
        }

        private void PostScan()
        {
            if (_patcher.downloadFiles.Count > 0)
            {
                pbProgress.Value = 0;
                pbProgress.Maximum = _patcher.downloadFiles.Count;
                pbFileProgress.Visible = true;
                bgDownloadWorker.RunWorkerAsync(_patcher);
            }
            else
                PostDownload();
        }
        #endregion

        #region extracting
        private void Patcher_StartedUnzip(object sender, StartUnzipEventArgs e)
        {
            TxtLogAppendText(String.Format("Decompressing Archive: {0}\r\n", e.Filename));
        }
        private void Patcher_ProgressedUnzip(object sender, ProgressUnzipEventArgs e)
        {
            // this is worthless, the current unzip code unzips the whole thing then gets to here
            // leaving the event handler in case we need it in the future
            // TxtLogAppendText(String.Format("Extracted File: {0}\r\n", e.ExtractedFilename));
        }
        private void Patcher_EndedUnzip(object sender, EndUnzipEventArgs e)
        {
            TxtLogAppendText(String.Format("Decompressed {0}\r\n", e.FileName));
        }
        #endregion

        #region Patching
        private void Patcher_StartedDownload(object sender, StartDownloadEventArgs e)
        {
            PbProgressPerformStep();
            TxtLogAppendText(String.Format("Downloading File..... {0} ({1})\r\n", e.Filename, e.Filesize.ToString(CultureInfo.InvariantCulture)));
            pbFileProgress.Maximum = 100;
            PbFileProgressSetValueStep(0);
        }

        private void Patcher_ProgressedDownload(object sender, DownloadProgressChangedEventArgs e)
        {
            PbFileProgressSetValueStep(e.ProgressPercentage);
        }

        private void Patcher_EndedDownload(object sender, AsyncCompletedEventArgs e)
        {
            PbFileProgressSetValueStep(100);
        }

        private void Patcher_FailedDownload(object sender, AsyncCompletedEventArgs e)
        {
            ManagedFile file = (ManagedFile)e.UserState;
            TxtLogAppendText(String.Format("Failed to download file {0}\r\n", file.Filename));
            CheckMeridianRunning();
        }

        private void PostDownload()
        {
            pbProgress.Value = pbProgress.Maximum;
            pbFileProgress.Visible = true;
            pbFileProgress.Value = pbFileProgress.Maximum;
            if (_patcher.DownloadFileFailed)
            {
               TxtLogAppendText("Patching failed!\r\n");
                TxtLogAppendText("Ensure the Meridian 59 client is closed while patching and if the problem persists, raise an issue at openmeridian.org/forums/\r\n");
            }
            else
                TxtLogAppendText("Patching Complete!\r\nWriting File Cache.\r\n");
            _patcher.SavePatchAsCache();
            btnPlay.Enabled = true;
        }
        #endregion

        #region bgScanWorker
        private void bgScanWorker_DoWork(object sender, DoWorkEventArgs e)
        {
            var myPatcher = (ClientPatcher)e.Argument;
            myPatcher.ScanClient();
        }
        private void bgScanWorker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            PostScan();
        }
        #endregion

        #region bgDownloadWorker
        private void bgDownloadWorker_DoWork(object sender, DoWorkEventArgs e)
        {
            var myPatcher = (ClientPatcher)e.Argument;
            //myPatcher.DownloadFiles();
            myPatcher.DownloadFilesAsync();
        }
        private void bgDownloadWorker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            PostDownload();
        }
        #endregion

        #region ThreadSafe Control Updates
        //Used to update main progress bar, one step
        delegate void ProgressPerformStepCallback();
        private void PbProgressPerformStep()
        {
            if (pbProgress.InvokeRequired)
            {
                ProgressPerformStepCallback d = PbProgressPerformStep;
                Invoke(d);
            }
            else
            {
                pbProgress.PerformStep();
            }
        }
        //Used to update per-file progress bar, set to value
        delegate void FileProgressSetValueCallback(int value);
        private void PbFileProgressSetValueStep(int value)
        {
            if (pbFileProgress.InvokeRequired)
            {
                FileProgressSetValueCallback d = PbFileProgressSetValueStep;
                Invoke(d, new object[] { value });
            }
            else
            {
                pbFileProgress.Value = value;
            }
        }
        //Used to add stuff to the Log
        delegate void TxtLogAppendTextCalback(string text);
        private void TxtLogAppendText(string text)
        {
            if (txtLog.InvokeRequired)
            {
                TxtLogAppendTextCalback d = TxtLogAppendText;
                Invoke(d, new object[] { text });
            }
            else
            {
                txtLog.AppendText(text);
            }
        }
        #endregion

        #region AccountCreation
        private bool _creatingAccount;

        private void btnCreateAccount_Click(object sender, EventArgs e)
        {
            if (!_creatingAccount)
            {
                webControl.Source = new Uri(_patcher.CurrentProfile.AccountCreationUrl);
                btnCreateAccount.Text = "Back to News";
                _creatingAccount = true;
            }
            else
            {
                webControl.Source = new Uri("http://openmeridian.org/forums/latestnews.php");
                SetCreateAccountText(_patcher.CurrentProfile);
                _creatingAccount = false;
            }
            tabControl1.SelectTab(0);
        }

        /// <summary>
        /// Display create account text for the current profile.
        /// </summary>
        private void SetCreateAccountText(PatcherSettings ps)
        {
            btnCreateAccount.Text = String.Format("Create Account for {0}", ps.ServerName);
        }
        #endregion

        private void Awesomium_Windows_Forms_WebControl_DocumentReady(object sender, DocumentReadyEventArgs e)
        {
            //TxtLogAppendText(webControl.HTML);
        }

        private void Awesomium_Windows_Forms_WebControl_ShowCreatedWebView(object sender, ShowCreatedWebViewEventArgs e)
        {
            WebControl webControl = sender as WebControl;
            if (webControl == null)
                return;

            if (!webControl.IsLive)
                return;
            webControl.Source = e.TargetURL;
        }

        #region Util
        void CheckForShortcut()
        {
            if (ApplicationDeployment.IsNetworkDeployed)
            {
                ApplicationDeployment ad = ApplicationDeployment.CurrentDeployment;
                if (ad.IsFirstRun)  //first time user has run the app
                {
                    string company = "OpenMeridian";
                    string description = "Open Meridian Patch and Client Management";

                    string desktopPath =
                        string.Concat(Environment.GetFolderPath(Environment.SpecialFolder.Desktop),
                        "\\", description, ".appref-ms");
                    string shortcutName =
                        string.Concat(Environment.GetFolderPath(Environment.SpecialFolder.Programs),
                        "\\", company, "\\", description, ".appref-ms");
                    File.Copy(shortcutName, desktopPath, true);

                }
            }
        }

        private void CheckMeridianRunning()
        {
            Process[] processlist = Process.GetProcessesByName("meridian");
            if (processlist.Length != 0)
            {
                foreach (Process process in processlist)
                {
                    if (process.Modules[0].FileName.ToLower() ==
                        (_patcher.CurrentProfile.ClientFolder + "\\meridian.exe").ToLower())
                    {
                        process.Kill();
                        MessageBox.Show("Warning! You must have Meridian closed in order to patch successfully!",
                            "Meridian Already Running!!", MessageBoxButtons.OK);
                    }
                }
            }
        }
        #endregion
    }
}
