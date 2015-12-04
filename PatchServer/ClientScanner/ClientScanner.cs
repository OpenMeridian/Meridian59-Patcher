using System;
using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;

namespace PatchListGenerator
{
    /// <summary>
    /// Signifies which version of the Meridian 59 Client we are working with
    /// </summary>
    public enum ClientType
    {
        Classic = 0,
        DotNet = 1,
    }
    /// <summary>
    /// ClientScanner is in charge of scanning a list of files and instantiating ManagedFile objects to track them with metadata.
    /// </summary>
    public class ClientScanner
    {
        #region Properties
        private List<string> ScanFiles { get; set; }
        public List<string> ScanExtensions { get; set; }
        public List<ManagedFile> Files { get; set; }
        public List<ManagedFile> SpecialFiles { get; set; }
        public virtual string BasePath { get; set; }
        #endregion

        #region Constructors
        public ClientScanner()
        {
        }

        public ClientScanner(string basepath)
        {
            BasePath = basepath;
        }
        #endregion

        #region File Handling/Searching
        public virtual void AddExtensions() { }
        public virtual void AddSpecialFiles() { }

        /// <summary>
        /// Recursive function to get all files in a directory and sub-directories
        /// </summary>
        /// <param name="sDir"></param>
        /// <returns></returns>
        private List<String> DirSearch(string sDir)
        {
            List<String> files = new List<String>();
            try
            {
                foreach (string f in Directory.GetFiles(sDir))
                {
                    files.Add(f);
                }
                foreach (string d in Directory.GetDirectories(sDir))
                {
                    files.AddRange(DirSearch(d));
                }
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }

            return files;
        }
        #endregion

        #region Scanning
        public void ScannerSetup(string basepath)
        {
            AddExtensions();
            AddSpecialFiles();
            ScanFiles = new List<string>();
            ScanFiles.AddRange(DirSearch(basepath));
        }

        /// <summary>
        /// Scans each file in the ScanFiles property of the ClientScanner Class,
        /// ManagedFile objects are added to the Files property of the ClientScanner Class
        /// </summary>
        public void ScanSource()
        {
            Files = new List<ManagedFile>();
            foreach (string fileName in ScanFiles)
            {
                string ext = Path.GetExtension(fileName).ToLower();
                if (ScanExtensions.Contains(ext))
                {
                    var file = new ManagedFile(fileName);
                    file.Basepath = fileName.Substring(BasePath.Length, (fileName.Length - BasePath.Length - Path.GetFileName(fileName).Length));
                    file.ComputeHash();
                    file.FillLength();
                    file.Version = ManagedFileVersion.VersionNum;
                    Files.Add(file);
                }
            }
        }
        #endregion

        #region JSON
        public string ToJson()
        {
            if (SpecialFiles != null)
                Files.AddRange(SpecialFiles);
            return JsonConvert.SerializeObject(Files, Formatting.Indented);
        }
        #endregion

        #region Util
        public void Report()
        {
            foreach (ManagedFile file in Files)
                Console.WriteLine(file.ToString());
        }
        #endregion
    }
}
