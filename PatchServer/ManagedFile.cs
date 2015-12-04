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
        VersionNum = 1
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
        /// Pulls out the file path and name, adds Beasepath(relative) metadata
        /// </summary>
        public void ParseFilePath()
        {
            Path = System.IO.Path.GetDirectoryName(Filepath);
            Filename = System.IO.Path.GetFileName(Filepath);
            if (Filepath.Contains("resource"))
                Basepath = "\\resource\\";
        }

        /// <summary>
        /// We compute an MD5 hash of ourselves using the firs 64 bytes of the file and metadata and save it in the MyHash property
        /// </summary>
        public void ComputeHash()
        {
            //We're going to read the first 64 bytes or so of the file, salt it with the filename, and coompute a MD5 hash.

            MD5 md5 = MD5.Create();
            if (!File.Exists(Filepath))
            {
                MyHash = "";
                return;
            }

            FileStream stream = File.OpenRead(Filepath);
            long numBytesToRead = 64;
            if (stream.Length < 64) //Make sure we dont read past the end of a file smaller than 64 bytes
            {
                numBytesToRead = stream.Length;
            }
            
            byte[] fileBytes = new byte[numBytesToRead];
            byte[] fileNameBytes = Encoding.ASCII.GetBytes(Filename);
            byte[] hashableBytes = new byte[numBytesToRead + Filename.Length];

            stream.Read(fileBytes, 0, (int) numBytesToRead);

            Buffer.BlockCopy(fileBytes,0,hashableBytes,0,fileBytes.Length);
            Buffer.BlockCopy(fileNameBytes,0,hashableBytes,fileBytes.Length, fileNameBytes.Length);

            MyHash = ByteArrayToString(md5.ComputeHash(hashableBytes));
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
