using System;
using System.IO;
using System.Collections.Generic;

namespace PatchListGenerator
{
    public class ClassicClientScanner : ClientScanner
    {
        public ClientType ClientType { get; set; }
        public override String BasePath { get; set; }
        public ClassicClientScanner()
            : base()
        {
            ClientType = ClientType.Classic;
            BasePath = "C:\\Games\\meridian_112\\";
        }
        public ClassicClientScanner(string basepath)
            : base(basepath)
        {
            BasePath = basepath;
        }


        /// <summary>
        /// Adds the extensions of files to scan for when using the Classic/Legacy Meridian Client
        /// </summary>
        public override void AddExtensions()
        {
            ScanExtensions = new List<string> { ".roo", ".dll", ".rsb", ".exe", ".bgf", ".wav", ".mp3", ".ttf", ".bsf" };
        }
        /// <summary>
        /// Adds the special files to scan for when using the Classic/Legacy Meridian Client
        /// </summary>
        public override void AddSpecialFiles()
        {
            SpecialFiles = new List<ManagedFile>();

            if (File.Exists(BasePath + "latest.zip"))
                SpecialFiles.Add(new ManagedFile(BasePath + "latest.zip", false));
        }
    }
}
