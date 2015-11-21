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
        DotNetX64 = 1,
        DotNetX86 = 2
    }
    /// <summary>
    /// ClientScanner is in charge of scanning a list of files and instantiating ManagedFile objects to track them with metadata.
    /// </summary>
    public class ClientScanner
    {
        private List<string> ScanFiles { get; set; }
        private List<string> ScanExtensions { get; set; }
        public List<ManagedFile> Files { get; set; }
        public string BasePath { get; set; }
        public ClientType ClientType { get; set; }
        
        public ClientScanner()
        {
            string basepath = "C:\\Meridian59-master\\run\\localclient\\";
            ClientType = ClientType.Classic;
            ScannerSetup(basepath);
        }

        public ClientScanner(string basepath)
        {
            ClientType = ClientType.Classic;
            ScannerSetup(basepath);
        }

        public ClientScanner(string basepath, ClientType clientType)
        {
            ClientType = clientType;
            ScannerSetup(basepath);
            BasePath = basepath;
        }

        private void ScannerSetup(string basepath)
        {
            switch (ClientType)
            {
                    case ClientType.Classic:
                        AddLegacyExensions();
                        ScanFiles = new List<string>();
                        ScanFiles.AddRange(DirSearch(basepath));
                        break;
                    case ClientType.DotNetX64:
                        ScanFiles = new List<string>();
                        ScanFiles.AddRange(DirSearch(basepath + "\\x64")); //TODO: is this needed since DirSearch() is recursive?
                        ScanFiles.AddRange(DirSearch(basepath + "\\resources"));
                        AddDotNetExensions();
                        break;
                    case ClientType.DotNetX86:
                        ScanFiles = new List<string>();
                        ScanFiles.AddRange(DirSearch(basepath + "\\x86"));
                        ScanFiles.AddRange(DirSearch(basepath + "\\resources"));
                        AddDotNetExensions();
                        break;
            }
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
            catch (System.Exception e)
            {
                throw new Exception(e.Message);
            }

            return files;
        }

        /// <summary>
        /// Adds the extensions of files to scan for when using Ogre3d/.NET Meridian Client
        /// </summary>
        private void AddDotNetExensions()
        {
            ScanExtensions = new List<string> { ".roo", ".dll", ".rsb", ".exe", ".bgf", ".wav", ".mp3", ".ttf", ".bsf",
                ".font", ".ttf", ".md", ".png", ".material", ".hlsl", ".dds", ".mesh", ".xml", ".pu", ".compsoitor",
                ".imageset", ".layout", ".looknfeel", ".scheme" };
        }
        /// <summary>
        /// Adds the extensions of files to scan for when using the Classic/Legacy Meridian Client
        /// </summary>
        private void AddLegacyExensions()
        {
            ScanExtensions = new List<string> {".roo", ".dll", ".rsb", ".exe", ".bgf", ".wav", ".mp3", ".ttf", ".bsf"};
        }
        /// <summary>
        /// Scans each file in the ScanFiles property of the ClientScanner Class, ManagedFile objects are added to the Files property of the ClientScanner Class
        /// </summary>
        public void ScanSource()
        {
            Files = new List<ManagedFile>();
            foreach (string fileName in ScanFiles)
            {
                string ext = fileName.Substring(fileName.Length - 4).ToLower();
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
