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
        private List<string> ScanFiles { get; set; }
        public List<string> ScanExtensions { get; set; }
        public List<ManagedFile> Files { get; set; }
        public virtual string BasePath { get; set; }
        
        public ClientScanner()
        {
        }

        public ClientScanner(string basepath)
        {
            BasePath = basepath;
        }

        public virtual void AddExtensions() { }
        public void ScannerSetup(string basepath)
        {
            AddExtensions();
            ScanFiles = new List<string>();
            ScanFiles.AddRange(DirSearch(basepath));
        }
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

        /// <summary>
        /// Scans each file in the ScanFiles property of the ClientScanner Class, ManagedFile objects are added to the Files property of the ClientScanner Class
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
                    file.Basepath = fileName.Substring(BasePath.Length);
                    file.ComputeHash();
                    file.FillLength();
                    Files.Add(file);
                }
            }
        }

        public string ToJson()
        {
            return JsonConvert.SerializeObject(Files, Formatting.Indented);
        }

        public void Report()
        {
            foreach (ManagedFile file in Files)
                Console.WriteLine(file.ToString());
        }

    }
}
