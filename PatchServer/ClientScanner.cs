using System;
using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;

namespace PatchListGenerator
{
    public enum ClientType
    {
        Classic = 0,
        DotNetX64 = 1,
        DotNetX86 = 2
    }

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
                        ScanFiles.AddRange(DirSearch(basepath + "\\x64"));
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
