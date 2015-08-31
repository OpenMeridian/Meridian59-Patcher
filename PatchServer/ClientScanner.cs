using System;
using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;

namespace PatchListGenerator
{
    public enum ClientType
    {
        Classic,
        DotNetX64,
        DotNetX86
    }

    public class ClientScanner
    {
        private List<string> ScanFolder { get; set; }
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
        }

        private void ScannerSetup(string basepath)
        {
            if (ClientType != ClientType.Classic)
            {
                AddDotNetExensions();
            }
            else
            {
                ScanFolder = new List<string> {basepath, basepath + "\\resource"};
                AddLegacyExensions();
            }
        }

        private void AddDotNetExensions()
        {
            ScanExtensions = new List<string> { ".roo", ".dll", ".rsb", ".exe", ".bgf", ".wav", ".mp3", ".ttf", ".bsf",
                ".font", ".ttf", ".md", ".png", ".material", ".hlsl", ".dds", ".mesh", ".xml", ".pu", ".compsoitor",
                ".imageset", ".layout", ".looknfeel", ".scheme" };
        }

        private void AddLegacyExensions()
        {
            ScanExtensions = new List<string> {".roo", ".dll", ".rsb", ".exe", ".bgf", ".wav", ".mp3", ".ttf", ".bsf"};
        }

        public void ScanSource()
        {
            Files = new List<ManagedFile>();
            foreach (string folder in ScanFolder) //
            {
                if (!Directory.Exists(folder))
                {
                    //Folder doesn't exist =(
                    throw new Exception();
                }
                // Process the list of files found in the directory. 
                string[] fileEntries = Directory.GetFiles(folder);
                foreach (string fileName in fileEntries)
                {
                    string ext = fileName.Substring(fileName.Length - 4).ToLower();
                    if (ScanExtensions.Contains(ext))
                    {
                        var file = new ManagedFile(fileName);
                        file.ParseFilePath();
                        file.ComputeHash();
                        file.FillLength();
                        Files.Add(file);
                    }
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
