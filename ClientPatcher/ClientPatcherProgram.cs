﻿using System;
using System.Diagnostics;
using System.IO;
using System.Security.Principal;
using System.Windows.Forms;

namespace ClientPatcher
{
    static class ClientPatcherProgram
    {
        [STAThread]
        static void Main()
        {
            string path = "C:\\Program Files\\Open Meridian";
            if (!Directory.Exists(path)) //if we're not installed yet, we need admin this run
            {
                MessageBox.Show("In order to set up your computer for the Meridian 59 Patcher, you will need to allow it to run as an administrator once.");
                if (IsAdministrator() == false) 
                {
                    // Restart program and run as admin
                    var exeName = Process.GetCurrentProcess().MainModule.FileName;
                    ProcessStartInfo startInfo = new ProcessStartInfo(exeName);
                    startInfo.Verb = "runas";
                    Process.Start(startInfo);
                    Application.Exit();
                    return;
                }
            }

            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new ClientPatchForm());
        }

        private static bool IsAdministrator()
        {
            WindowsIdentity identity = WindowsIdentity.GetCurrent();
            if (identity != null)
            {
                WindowsPrincipal principal = new WindowsPrincipal(identity);
                return principal.IsInRole(WindowsBuiltInRole.Administrator);
            }
            return false;
        }


    }
}
