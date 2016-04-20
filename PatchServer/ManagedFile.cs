using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using Newtonsoft.Json;

namespace PatchListGenerator
{
    /// <summary>
    /// Increment version whenever the format of ManagedFile is changed.
    /// Necessary so that format changes don't require all users to download
    /// the entire client again due to having an incorrect cache.txt.
    /// </summary>
    public enum ManagedFileVersion
    {
        VersionNum = 3
    }

    /// <summary>
    /// ManagedFiles are files that can compute their own hash, parse their own path,
    /// and read other metadata from themselves, such as size.
    /// </summary>
    public class ManagedFile
    {
        [JsonIgnore]
        public string Path; //C:\whatever\
        [JsonIgnore]
        public string Filepath;

        public string Basepath = "\\";
        public string Filename { get; set; } //whatever.roo
        public string MyHash { get; set; } //what is the hash of this file
        public long Length;
        public bool Download = true;
        public ManagedFileVersion Version;

        #region Constructors
        public ManagedFile()
        {
        }

        public ManagedFile(string filepath)
        {
            Filepath = filepath;
            ParseFilePath();
            FillLength();
        }

        public ManagedFile(string filepath,bool autoDownload)
        {
            Filepath = filepath;
            Download = autoDownload;
            Version = ManagedFileVersion.VersionNum;
            ParseFilePath();
            ComputeHash();
            FillLength();
        }
        #endregion

        /// <summary>
        /// Pulls out the file path and name, adds Basepath(relative) metadata
        /// </summary>
        public void ParseFilePath()
        {
            Path = System.IO.Path.GetDirectoryName(Filepath);
            Filename = System.IO.Path.GetFileName(Filepath);
            // I don't think this part is needed any longer.
            //if (Filepath.Contains("resource"))
              //  Basepath = "\\resource\\";
        }

        /// <summary>
        /// We compute an MD5 hash of ourselves using the full
        /// contents of the file and save it in the MyHash property
        /// </summary>
        public void ComputeHash()
        {
            MD5 md5 = MD5.Create();
            if (!File.Exists(Filepath))
            {
                MyHash = "";
                return;
            }

            // Create filestream.
            FileStream stream = new FileStream(Filepath, FileMode.Open, FileAccess.Read);

            // Compute md5, store in MyHash.
            MyHash = ByteArrayToString(md5.ComputeHash(stream));

            // Close filestream.
            stream.Close();
            stream.Dispose();
        }

        /// <summary>
        /// Utility function
        /// </summary>
        /// <param name="bytearray"></param>
        /// <returns></returns>
        private string ByteArrayToString(byte[] bytearray)
        {
            string hex = BitConverter.ToString(bytearray);
            return hex.Replace("-", "");
        }

        public override string ToString()
        {
            return String.Format("File: {0,20} Hash: {1} Size: {2}", Filename, MyHash, Length);
        }

        public string ToJson()
        {
            return JsonConvert.SerializeObject(this, Formatting.Indented);
        }

        /// <summary>
        /// Fill file-length metadata
        /// </summary>
        public void FillLength()
        {
            if (!File.Exists(Filepath))
            {
                Length = 0;
                return;
            }

            var file = new FileInfo(Filepath);
            Length = file.Length;
        }
    }
}
