using System.ComponentModel;
using System.Windows.Forms;
using Awesomium.Windows.Forms;

namespace ClientPatcher
{
    partial class ClientPatchForm
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ClientPatchForm));
            this.label1 = new System.Windows.Forms.Label();
            this.pbProgress = new System.Windows.Forms.ProgressBar();
            this.btnPlay = new System.Windows.Forms.Button();
            this.ddlServer = new System.Windows.Forms.ComboBox();
            this.label2 = new System.Windows.Forms.Label();
            this.btnPatch = new System.Windows.Forms.Button();
            this.label3 = new System.Windows.Forms.Label();
            this.groupProfileSettings = new System.Windows.Forms.GroupBox();
            this.label4 = new System.Windows.Forms.Label();
            this.txtFullInstallURL = new System.Windows.Forms.TextBox();
            this.btnBrowse = new System.Windows.Forms.Button();
            this.btnSave = new System.Windows.Forms.Button();
            this.label8 = new System.Windows.Forms.Label();
            this.label7 = new System.Windows.Forms.Label();
            this.label6 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.cbDefaultServer = new System.Windows.Forms.CheckBox();
            this.txtPatchBaseURL = new System.Windows.Forms.TextBox();
            this.txtClientFolder = new System.Windows.Forms.TextBox();
            this.txtPatchInfoURL = new System.Windows.Forms.TextBox();
            this.txtServerName = new System.Windows.Forms.TextBox();
            this.btnCacheGen = new System.Windows.Forms.Button();
            this.txtLog = new System.Windows.Forms.TextBox();
            this.pbFileProgress = new System.Windows.Forms.ProgressBar();
            this.bgScanWorker = new System.ComponentModel.BackgroundWorker();
            this.bgDownloadWorker = new System.ComponentModel.BackgroundWorker();
            this.tabControl1 = new System.Windows.Forms.TabControl();
            this.tabBrowser = new System.Windows.Forms.TabPage();
            this.webControl = new Awesomium.Windows.Forms.WebControl(this.components);
            this.tabLog = new System.Windows.Forms.TabPage();
            this.tabOptions = new System.Windows.Forms.TabPage();
            this.btnStartModify = new System.Windows.Forms.Button();
            this.btnRemove = new System.Windows.Forms.Button();
            this.btnAdd = new System.Windows.Forms.Button();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.btnCreateAccount = new System.Windows.Forms.Button();
            this.groupProfileSettings.SuspendLayout();
            this.tabControl1.SuspendLayout();
            this.tabBrowser.SuspendLayout();
            this.tabLog.SuspendLayout();
            this.tabOptions.SuspendLayout();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 8F);
            this.label1.Location = new System.Drawing.Point(615, 164);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(120, 33);
            this.label1.TabIndex = 25;
            this.label1.Text = "Recreates local cache. May take a moment.";
            // 
            // pbProgress
            // 
            this.pbProgress.Location = new System.Drawing.Point(12, 265);
            this.pbProgress.Name = "pbProgress";
            this.pbProgress.Size = new System.Drawing.Size(187, 32);
            this.pbProgress.Step = 1;
            this.pbProgress.TabIndex = 0;
            // 
            // btnPlay
            // 
            this.btnPlay.Location = new System.Drawing.Point(12, 416);
            this.btnPlay.Name = "btnPlay";
            this.btnPlay.Size = new System.Drawing.Size(187, 46);
            this.btnPlay.TabIndex = 1;
            this.btnPlay.Text = "Play!";
            this.btnPlay.UseVisualStyleBackColor = true;
            this.btnPlay.Click += new System.EventHandler(this.btnPlay_Click);
            // 
            // ddlServer
            // 
            this.ddlServer.FormattingEnabled = true;
            this.ddlServer.Location = new System.Drawing.Point(12, 41);
            this.ddlServer.Name = "ddlServer";
            this.ddlServer.Size = new System.Drawing.Size(187, 21);
            this.ddlServer.TabIndex = 4;
            this.ddlServer.SelectionChangeCommitted += new System.EventHandler(this.ddlServer_SelectionChangeCommitted);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("Microsoft Sans Serif", 18F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label2.Location = new System.Drawing.Point(10, 9);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(192, 29);
            this.label2.TabIndex = 6;
            this.label2.Text = "Select a Server";
            // 
            // btnPatch
            // 
            this.btnPatch.Location = new System.Drawing.Point(12, 175);
            this.btnPatch.Name = "btnPatch";
            this.btnPatch.Size = new System.Drawing.Size(187, 46);
            this.btnPatch.TabIndex = 7;
            this.btnPatch.Text = "Update/Install";
            this.btnPatch.UseVisualStyleBackColor = true;
            this.btnPatch.Click += new System.EventHandler(this.btnPatch_Click);
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Font = new System.Drawing.Font("Microsoft Sans Serif", 18F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label3.Location = new System.Drawing.Point(18, 133);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(174, 29);
            this.label3.TabIndex = 8;
            this.label3.Text = "Update/Install";
            // 
            // groupProfileSettings
            // 
            this.groupProfileSettings.Controls.Add(this.label4);
            this.groupProfileSettings.Controls.Add(this.txtFullInstallURL);
            this.groupProfileSettings.Controls.Add(this.btnBrowse);
            this.groupProfileSettings.Controls.Add(this.btnSave);
            this.groupProfileSettings.Controls.Add(this.label8);
            this.groupProfileSettings.Controls.Add(this.label7);
            this.groupProfileSettings.Controls.Add(this.label6);
            this.groupProfileSettings.Controls.Add(this.label5);
            this.groupProfileSettings.Controls.Add(this.cbDefaultServer);
            this.groupProfileSettings.Controls.Add(this.txtPatchBaseURL);
            this.groupProfileSettings.Controls.Add(this.txtClientFolder);
            this.groupProfileSettings.Controls.Add(this.txtPatchInfoURL);
            this.groupProfileSettings.Controls.Add(this.txtServerName);
            this.groupProfileSettings.Enabled = false;
            this.groupProfileSettings.Location = new System.Drawing.Point(6, 7);
            this.groupProfileSettings.Name = "groupProfileSettings";
            this.groupProfileSettings.Size = new System.Drawing.Size(605, 341);
            this.groupProfileSettings.TabIndex = 3;
            this.groupProfileSettings.TabStop = false;
            this.groupProfileSettings.Text = "Profile Settings";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(11, 170);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(72, 13);
            this.label4.TabIndex = 30;
            this.label4.Text = "FullInstallURL";
            // 
            // txtFullInstallURL
            // 
            this.txtFullInstallURL.Location = new System.Drawing.Point(6, 186);
            this.txtFullInstallURL.Name = "txtFullInstallURL";
            this.txtFullInstallURL.Size = new System.Drawing.Size(593, 20);
            this.txtFullInstallURL.TabIndex = 29;
            // 
            // btnBrowse
            // 
            this.btnBrowse.Location = new System.Drawing.Point(502, 110);
            this.btnBrowse.Name = "btnBrowse";
            this.btnBrowse.Size = new System.Drawing.Size(97, 20);
            this.btnBrowse.TabIndex = 22;
            this.btnBrowse.Text = "Browse for Folder";
            this.btnBrowse.UseVisualStyleBackColor = true;
            this.btnBrowse.Click += new System.EventHandler(this.btnBrowse_Click);
            // 
            // btnSave
            // 
            this.btnSave.Location = new System.Drawing.Point(502, 275);
            this.btnSave.Name = "btnSave";
            this.btnSave.Size = new System.Drawing.Size(97, 55);
            this.btnSave.TabIndex = 21;
            this.btnSave.Text = "Save Changes";
            this.btnSave.UseVisualStyleBackColor = true;
            this.btnSave.Click += new System.EventHandler(this.btnSave_Click);
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Location = new System.Drawing.Point(11, 133);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(81, 13);
            this.label8.TabIndex = 20;
            this.label8.Text = "PatchBaseURL";
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(11, 94);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(62, 13);
            this.label7.TabIndex = 19;
            this.label7.Text = "ClientFolder";
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(11, 55);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(75, 13);
            this.label6.TabIndex = 18;
            this.label6.Text = "PatchInfoURL";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(11, 16);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(67, 13);
            this.label5.TabIndex = 17;
            this.label5.Text = "Profile Name";
            // 
            // cbDefaultServer
            // 
            this.cbDefaultServer.AutoSize = true;
            this.cbDefaultServer.Location = new System.Drawing.Point(6, 212);
            this.cbDefaultServer.Name = "cbDefaultServer";
            this.cbDefaultServer.Size = new System.Drawing.Size(100, 17);
            this.cbDefaultServer.TabIndex = 16;
            this.cbDefaultServer.Text = "Default Server?";
            this.cbDefaultServer.UseVisualStyleBackColor = true;
            // 
            // txtPatchBaseURL
            // 
            this.txtPatchBaseURL.Location = new System.Drawing.Point(6, 149);
            this.txtPatchBaseURL.Name = "txtPatchBaseURL";
            this.txtPatchBaseURL.Size = new System.Drawing.Size(593, 20);
            this.txtPatchBaseURL.TabIndex = 15;
            // 
            // txtClientFolder
            // 
            this.txtClientFolder.Location = new System.Drawing.Point(6, 110);
            this.txtClientFolder.Name = "txtClientFolder";
            this.txtClientFolder.Size = new System.Drawing.Size(490, 20);
            this.txtClientFolder.TabIndex = 14;
            // 
            // txtPatchInfoURL
            // 
            this.txtPatchInfoURL.Location = new System.Drawing.Point(6, 71);
            this.txtPatchInfoURL.Name = "txtPatchInfoURL";
            this.txtPatchInfoURL.Size = new System.Drawing.Size(593, 20);
            this.txtPatchInfoURL.TabIndex = 13;
            // 
            // txtServerName
            // 
            this.txtServerName.Location = new System.Drawing.Point(6, 32);
            this.txtServerName.Name = "txtServerName";
            this.txtServerName.Size = new System.Drawing.Size(593, 20);
            this.txtServerName.TabIndex = 12;
            // 
            // btnCacheGen
            // 
            this.btnCacheGen.Location = new System.Drawing.Point(617, 135);
            this.btnCacheGen.Name = "btnCacheGen";
            this.btnCacheGen.Size = new System.Drawing.Size(105, 23);
            this.btnCacheGen.TabIndex = 24;
            this.btnCacheGen.Text = "Verify All Files";
            this.btnCacheGen.UseVisualStyleBackColor = true;
            this.btnCacheGen.Click += new System.EventHandler(this.btnCacheGen_Click);
            // 
            // txtLog
            // 
            this.txtLog.BackColor = System.Drawing.SystemColors.Control;
            this.txtLog.ForeColor = System.Drawing.SystemColors.ControlText;
            this.txtLog.Location = new System.Drawing.Point(6, 3);
            this.txtLog.Multiline = true;
            this.txtLog.Name = "txtLog";
            this.txtLog.ReadOnly = true;
            this.txtLog.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.txtLog.Size = new System.Drawing.Size(722, 468);
            this.txtLog.TabIndex = 11;
            // 
            // pbFileProgress
            // 
            this.pbFileProgress.Location = new System.Drawing.Point(12, 227);
            this.pbFileProgress.Name = "pbFileProgress";
            this.pbFileProgress.Size = new System.Drawing.Size(187, 32);
            this.pbFileProgress.TabIndex = 12;
            this.pbFileProgress.Visible = false;
            // 
            // bgScanWorker
            // 
            this.bgScanWorker.DoWork += new System.ComponentModel.DoWorkEventHandler(this.bgScanWorker_DoWork);
            this.bgScanWorker.RunWorkerCompleted += new System.ComponentModel.RunWorkerCompletedEventHandler(this.bgScanWorker_RunWorkerCompleted);
            // 
            // bgDownloadWorker
            // 
            this.bgDownloadWorker.DoWork += new System.ComponentModel.DoWorkEventHandler(this.bgDownloadWorker_DoWork);
            this.bgDownloadWorker.RunWorkerCompleted += new System.ComponentModel.RunWorkerCompletedEventHandler(this.bgDownloadWorker_RunWorkerCompleted);
            // 
            // tabControl1
            // 
            this.tabControl1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.tabControl1.Controls.Add(this.tabBrowser);
            this.tabControl1.Controls.Add(this.tabLog);
            this.tabControl1.Controls.Add(this.tabOptions);
            this.tabControl1.Location = new System.Drawing.Point(212, 12);
            this.tabControl1.Multiline = true;
            this.tabControl1.Name = "tabControl1";
            this.tabControl1.SelectedIndex = 0;
            this.tabControl1.Size = new System.Drawing.Size(739, 506);
            this.tabControl1.TabIndex = 14;
            // 
            // tabBrowser
            // 
            this.tabBrowser.Controls.Add(this.webControl);
            this.tabBrowser.Location = new System.Drawing.Point(4, 22);
            this.tabBrowser.Name = "tabBrowser";
            this.tabBrowser.Size = new System.Drawing.Size(731, 480);
            this.tabBrowser.TabIndex = 2;
            this.tabBrowser.Text = "News";
            // 
            // webControl
            // 
            this.webControl.Location = new System.Drawing.Point(3, 0);
            this.webControl.Size = new System.Drawing.Size(725, 480);
            this.webControl.Source = new System.Uri("http://openmeridian.org/forums/latestnews.php", System.UriKind.Absolute);
            this.webControl.TabIndex = 0;
            this.webControl.ShowCreatedWebView += new Awesomium.Core.ShowCreatedWebViewEventHandler(this.Awesomium_Windows_Forms_WebControl_ShowCreatedWebView);
            this.webControl.DocumentReady += new Awesomium.Core.DocumentReadyEventHandler(this.Awesomium_Windows_Forms_WebControl_DocumentReady);
            // 
            // tabLog
            // 
            this.tabLog.Controls.Add(this.txtLog);
            this.tabLog.Location = new System.Drawing.Point(4, 22);
            this.tabLog.Name = "tabLog";
            this.tabLog.Padding = new System.Windows.Forms.Padding(3);
            this.tabLog.Size = new System.Drawing.Size(731, 480);
            this.tabLog.TabIndex = 0;
            this.tabLog.Text = "Log";
            this.tabLog.UseVisualStyleBackColor = true;
            // 
            // tabOptions
            // 
            this.tabOptions.Controls.Add(this.btnStartModify);
            this.tabOptions.Controls.Add(this.btnRemove);
            this.tabOptions.Controls.Add(this.label1);
            this.tabOptions.Controls.Add(this.btnAdd);
            this.tabOptions.Controls.Add(this.btnCacheGen);
            this.tabOptions.Controls.Add(this.groupBox1);
            this.tabOptions.Controls.Add(this.groupProfileSettings);
            this.tabOptions.Location = new System.Drawing.Point(4, 22);
            this.tabOptions.Name = "tabOptions";
            this.tabOptions.Padding = new System.Windows.Forms.Padding(3);
            this.tabOptions.Size = new System.Drawing.Size(731, 480);
            this.tabOptions.TabIndex = 1;
            this.tabOptions.Text = "Options";
            this.tabOptions.UseVisualStyleBackColor = true;
            // 
            // btnStartModify
            // 
            this.btnStartModify.Location = new System.Drawing.Point(617, 73);
            this.btnStartModify.Name = "btnStartModify";
            this.btnStartModify.Size = new System.Drawing.Size(108, 23);
            this.btnStartModify.TabIndex = 28;
            this.btnStartModify.Text = "Modify Profile";
            this.btnStartModify.UseVisualStyleBackColor = true;
            this.btnStartModify.Click += new System.EventHandler(this.btnStartModify_Click);
            // 
            // btnRemove
            // 
            this.btnRemove.Location = new System.Drawing.Point(617, 44);
            this.btnRemove.Name = "btnRemove";
            this.btnRemove.Size = new System.Drawing.Size(108, 23);
            this.btnRemove.TabIndex = 27;
            this.btnRemove.Text = "Remove Profile";
            this.btnRemove.UseVisualStyleBackColor = true;
            this.btnRemove.Click += new System.EventHandler(this.btnRemove_Click);
            // 
            // btnAdd
            // 
            this.btnAdd.Location = new System.Drawing.Point(617, 13);
            this.btnAdd.Name = "btnAdd";
            this.btnAdd.Size = new System.Drawing.Size(108, 23);
            this.btnAdd.TabIndex = 26;
            this.btnAdd.Text = "Add New Profile";
            this.btnAdd.UseVisualStyleBackColor = true;
            this.btnAdd.Click += new System.EventHandler(this.btnAdd_Click);
            // 
            // groupBox1
            // 
            this.groupBox1.Location = new System.Drawing.Point(12, 354);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(524, 111);
            this.groupBox1.TabIndex = 16;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Global Settings";
            // 
            // btnCreateAccount
            // 
            this.btnCreateAccount.Location = new System.Drawing.Point(12, 468);
            this.btnCreateAccount.Name = "btnCreateAccount";
            this.btnCreateAccount.Size = new System.Drawing.Size(187, 46);
            this.btnCreateAccount.TabIndex = 15;
            this.btnCreateAccount.Text = "Create Account";
            this.btnCreateAccount.UseVisualStyleBackColor = true;
            this.btnCreateAccount.Click += new System.EventHandler(this.btnCreateAccount_Click);
            // 
            // ClientPatchForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(963, 527);
            this.Controls.Add(this.btnPlay);
            this.Controls.Add(this.btnPatch);
            this.Controls.Add(this.tabControl1);
            this.Controls.Add(this.pbFileProgress);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.ddlServer);
            this.Controls.Add(this.pbProgress);
            this.Controls.Add(this.btnCreateAccount);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "ClientPatchForm";
            this.Text = "OpenMeridian Client Patcher";
            this.Load += new System.EventHandler(this.Form1_Load);
            this.groupProfileSettings.ResumeLayout(false);
            this.groupProfileSettings.PerformLayout();
            this.tabControl1.ResumeLayout(false);
            this.tabBrowser.ResumeLayout(false);
            this.tabLog.ResumeLayout(false);
            this.tabLog.PerformLayout();
            this.tabOptions.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private ProgressBar pbProgress;
        private Button btnPlay;
        private ComboBox ddlServer;
        private Label label2;
        private Button btnPatch;
        private Label label3;
        private GroupBox groupProfileSettings;
        private Label label8;
        private Label label7;
        private Label label6;
        private Label label5;
        private CheckBox cbDefaultServer;
        private TextBox txtPatchBaseURL;
        private TextBox txtClientFolder;
        private TextBox txtPatchInfoURL;
        private TextBox txtServerName;
        private Button btnSave;
        private Button btnBrowse;
        private TextBox txtLog;
        private ProgressBar pbFileProgress;
        private BackgroundWorker bgScanWorker;
        private BackgroundWorker bgDownloadWorker;
        private TabControl tabControl1;
        private TabPage tabLog;
        private TabPage tabOptions;
        private TabPage tabBrowser;
        private WebControl webControl;
        private Button btnCreateAccount;
        private GroupBox groupBox1;
        private Button btnStartModify;
        private Button btnRemove;
        private Button btnAdd;
        private Button btnCacheGen;
        private Label label4;
        private TextBox txtFullInstallURL;
        private Label label1;
    }
}

