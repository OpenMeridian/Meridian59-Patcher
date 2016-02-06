using System;
using System.IO;
using System.Collections.Generic;

namespace PatchListGenerator
{
    public class OgreClientScanner : ClientScanner
    {
        #region Properties
        public ClientType ClientType { get; set; }
        public override String BasePath { get; set; }
        #endregion

        #region Constructors
        public OgreClientScanner()
            : base()
        {
            ClientType = ClientType.DotNet;
            BasePath = "C:\\Games\\Meridian59-1.0.4.0\\";
        }

        public OgreClientScanner(string basepath)
             : base(basepath)
        {
             BasePath = basepath;
        }
        #endregion

        #region File Handling/Searching
        /// <summary>
        /// Adds the extensions of files to scan for when using Ogre3d/.NET Meridian Client
        /// </summary>
        public override void AddExtensions()
        {
           ScanExtensions = new List<string> { ".roo", ".dll", ".rsb", ".exe", ".bgf", ".wav", ".mp3", ".bsf",
                ".font", ".ttf", ".md", ".png", ".material", ".hlsl", ".dds", ".mesh", ".xml", ".pu", ".compositor",
                ".imageset", ".layout", ".looknfeel", ".scheme", ".ogg" };
        }

        /// <summary>
        /// Adds the special files to scan for when using Ogre3d/.NET Meridian Client
        /// </summary>
        public override void AddSpecialFiles()
        {
            SpecialFiles = new List<ManagedFile>();

            if (File.Exists(BasePath + "\\" + "latest.zip"))
                SpecialFiles.Add(new ManagedFile(BasePath + "\\" + "latest.zip", false));
        }
        #endregion
    }
}
