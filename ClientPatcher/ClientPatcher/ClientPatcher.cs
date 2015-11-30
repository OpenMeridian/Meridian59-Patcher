using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading;
using Newtonsoft.Json;
using PatchListGenerator;
using Shell32;

namespace ClientPatcher
{
    #region Event Delegates and Args

    //Event when we Scan a File, used to notify UI.
    public delegate void ScanFileEventHandler(object sender, ScanEventArgs e);
    public class ScanEventArgs : EventArgs
    {
        private readonly string _filename;
        public string Filename
        {
            get
            {
                return _filename;
            }
        }

        public ScanEventArgs(string filename)
        {
            _filename = filename;
        }
    }

    //Event when we Start a Download, used to notify UI.
    public delegate void StartDownloadEventHandler(object sender, StartDownloadEventArgs e);
    public class StartDownloadEventArgs : EventArgs
    {
        private readonly long _filesize;
        public long Filesize
        {
            get
            {
                return _filesize;
            }
        }

        private readonly string _filename;
        public string Filename
        {
            get
            {
                return _filename;
            }
        }

        public StartDownloadEventArgs(string filename, long filesize)
        {
            _filename = filename;
            _filesize = filesize;
        }
    }

    //Event when we Start to unzip a file, used to notify UI.
    public delegate void StartUnzipEventHandler(object sender, StartUnzipEventArgs e);
    public class StartUnzipEventArgs : EventArgs
    {
        private readonly long _filesize;
        public long Filesize
        {
            get
            {
                return _filesize;
            }
        }

        private readonly string _filename;
        public string Filename
        {
            get
            {
                return _filename;
            }
        }
        private int _totalMembers;
        public int TotalMembers
        {
            get
            {
                return _totalMembers;
            }
        }
        public StartUnzipEventArgs(string filename, long filesize, int totalMembers)
        {
            _filename = filename;
            _filesize = filesize;
            _totalMembers = totalMembers;
        }
    }

    //Event when we progress in an unzip operation, used to notify UI.
    public delegate void ProgressUnzipEventHandler(object sender, ProgressUnzipEventArgs e);
    public class ProgressUnzipEventArgs : EventArgs
    {
        private long _totalfiles;
        public long TotalFiles
        {
            get
            {
                return _totalfiles;
            }
        }

        private long _extractedfiles;
        public long ExtractedFiles
        {
            get
            {
                return _extractedfiles;
            }
        }

        private string _extractedFilename;
        public string ExtractedFilename
        {
            get
            {
                return _extractedFilename;
            }
        }

        public ProgressUnzipEventArgs(string extractedFilename, long extractedFiles, long totalFiles)
        {
            _extractedFilename = extractedFilename;
            _extractedfiles = extractedFiles;
            _totalfiles = totalFiles;
        }
    }

    //Event when we complete an unzip operation, used to notify UI.
    public delegate void EndUnzipEventHandler(object sender, EndUnzipEventArgs e);
    public class EndUnzipEventArgs : EventArgs
    {
        private long _totalbytes;
        public long Totalbytes
        {
            get
            {
                return _totalbytes;
            }
        }
        private string _filename;
        public string FileName
        {
            get
            {
                return _filename;
            }
        }
        public EndUnzipEventArgs(string filename, long totalBytes)
        {
            _filename = filename;
            _totalbytes = totalBytes;
        }
    }

    //Event when we Make progress in a Download, used to notify UI.
    public delegate void ProgressDownloadEventHandler(object sender, DownloadProgressChangedEventArgs e);
    //Event when we Complete a Download, used to notify UI.
    public delegate void EndDownloadEventHandler(object sender, AsyncCompletedEventArgs e);
    public delegate void FailDownloadHandler(object sender, AsyncCompletedEventArgs e);

    #endregion

    abstract class ClientPatcher
    {
        protected const string CacheFile = "\\cache.txt";
        private const string UserAgentString = "Mozilla/4.0 (compatible; .NET CLR 4.0.;) OpenMeridianPatcher v1.4";

        private string _patchInfoJason = "";

        public List<ManagedFile> PatchFiles; //Loaded from the web server at PatchInfoURL
        public List<ManagedFile> downloadFiles; //Loaded with files that do NOT match
        private List<ManagedFile> cacheFiles; //Loaded with cache.txt to compare to PatchFiles

        public WebClient MyWebClient;

        bool _continueAsync;
        bool _retryFile;

        private bool _downloadFileFailed;
        public bool DownloadFileFailed
        {
           get
           {
              return _downloadFileFailed;
           }
        }

        public PatcherSettings CurrentProfile { get; set; }

        #region Events
        //Event when we Scan a File, used to notify UI.
        public event ScanFileEventHandler FileScanned;
        protected virtual void OnFileScan(ScanEventArgs e)
        {
            if (FileScanned != null)
                FileScanned(this, e);
        }
        //Event when we Start a Download, used to notify UI.
        public event StartDownloadEventHandler StartedDownload;
        protected virtual void OnStartDownload(StartDownloadEventArgs e)
        {
            if (StartedDownload != null)
                StartedDownload(this, e);
        }
        //Event when we Make progress in a Download, used to notify UI.
        public event ProgressDownloadEventHandler ProgressedDownload;
        protected virtual void OnProgressedDownload(DownloadProgressChangedEventArgs e)
        {
            if (ProgressedDownload != null)
                ProgressedDownload(this, e);
        }
        //Event when we Complete a Download, used to notify UI.
        public event EndDownloadEventHandler EndedDownload;
        protected virtual void OnEndDownload(AsyncCompletedEventArgs e)
        {
            if (EndedDownload != null)
                EndedDownload(this, e);
        }
        public event FailDownloadHandler FailedDownload;
        protected virtual void OnFailedDownload(AsyncCompletedEventArgs e)
        {
           if (FailedDownload != null)
              FailedDownload(this, e);
        }
        public event StartUnzipEventHandler StartedUnzip;
        protected virtual void OnStartedUnzip(StartUnzipEventArgs e)
        {
            if (StartedUnzip != null)
            {
                StartedUnzip(this, e);
            }
        }
        public event ProgressUnzipEventHandler ProgressedUnzip;
        protected virtual void OnProgressedUnzip(ProgressUnzipEventArgs e)
        {
            if (ProgressedUnzip != null)
            {
                ProgressedUnzip(this, e);
            }
        }
        public event EndUnzipEventHandler EndedUnzip;
        protected virtual void OnEndedUnzip(EndUnzipEventArgs e)
        {
            if (EndedUnzip != null)
            {
                EndedUnzip(this, e);
            }
        }
        #endregion

        public ClientPatcher()
        {
            _downloadFileFailed = false;
            downloadFiles = new List<ManagedFile>();
            MyWebClient = new WebClient();
            MyWebClient.Headers.Add("user-agent", UserAgentString);
        }
        public ClientPatcher(PatcherSettings settings)
        {
            _downloadFileFailed = false;
            downloadFiles = new List<ManagedFile>();
            MyWebClient = new WebClient();
            MyWebClient.Headers.Add("user-agent", UserAgentString);
            CurrentProfile = settings;
        }
        public int DownloadPatchDefinition()
        {
            var wc = new WebClient();
            wc.Headers.Add("user-agent", UserAgentString);
            try
            {
                _patchInfoJason = wc.DownloadString(CurrentProfile.PatchInfoUrl);
                PatchFiles = JsonConvert.DeserializeObject<List<ManagedFile>>(_patchInfoJason);
                return 1;
            }
            catch (WebException e)
            {
                Console.WriteLine("WebException Handler: {0}", e);
                return 0;
            }
        }

        private void TestPath(string path)
        {
            if (!Directory.Exists(Path.GetDirectoryName(path)))
            {
                try
                {
                    Directory.CreateDirectory(Path.GetDirectoryName(path));
                }
                catch (Exception e)
                {
                    throw new IOException("Unable to TestPath()" + e);
                }
            }
        }


        public bool HasCache()
        {
            return File.Exists(CurrentProfile.ClientFolder + CacheFile);
        }

        private void CreateFolderStructure()
        {
            TestPath(CurrentProfile.ClientFolder + "\\");
        }

        public void CreateNewClient()
        {
            CreateFolderStructure();

            ManagedFile file = PatchFiles.Find(x => x.Filename == "latest.zip");

            // if we can get a latest.zip use it, if not, just patch as normal
            if (file != null)
            {
                DownloadOneFileAsync(file);


                while (!_continueAsync)
                {
                    //Wait for the download to finish
                    Thread.Sleep(10);
                }

                if (!_downloadFileFailed)
                {
                    UnZip(CurrentProfile.ClientFolder + file.Basepath + file.Filename, CurrentProfile.ClientFolder + file.Basepath);
                    File.Delete(CurrentProfile.ClientFolder + file.Basepath + file.Filename);
                }
                else
                {
                    // don't fail the patch if we couldn't get the full installation zip, just patch them
                    _downloadFileFailed = false;

                    if (File.Exists(CurrentProfile.ClientFolder + file.Basepath + file.Filename))
                        File.Delete(CurrentProfile.ClientFolder + file.Basepath + file.Filename);
                }
            }
        }

        public void UnZip(string zipFile, string folderPath)
        {
            int extractedFiles = 9;
            int totalFiles = 0;
            string filename = Path.GetFileName(zipFile);

            if (!File.Exists(zipFile))
                throw new FileNotFoundException();

            if (!Directory.Exists(folderPath))
                Directory.CreateDirectory(folderPath);

            Shell objShell = new Shell();
            Folder destinationFolder = objShell.NameSpace(folderPath);
            Folder sourceFile = objShell.NameSpace(zipFile);

            // TODO: this doesn't actually get the number of files in the archive
            // just the number of items it sees at the top level of the archive
            // including folders
            totalFiles = sourceFile.Items().Count;

            if (StartedUnzip != null)
                StartedUnzip(this, new StartUnzipEventArgs(filename, 0, totalFiles));

            foreach (var file in sourceFile.Items())
            {
                destinationFolder.CopyHere(file, 16);
                extractedFiles++;

                // useless unless we use a different method of unzipping.  
                // Shell32 extracts the file in one big step and never gives
                // us the opportunity to report progress or update the form
                // from here

                //if (ProgressedUnzip != null)
                //    ProgressedUnzip(this, new ProgressUnzipEventArgs("extractedfilenamehere", extractedFiles, totalFiles));
            }

            if (EndedUnzip != null)
                EndedUnzip(this, new EndUnzipEventArgs(zipFile, 0));
        }

        public abstract bool IsNewClient();
        public abstract void Launch();

        public void ScanClient()
        {
            if (IsNewClient())
                CreateNewClient();

            CompareFiles();
        }

        public abstract void GenerateCache();

        public void SavePatchAsCache()
        {
            string fullpath = CurrentProfile.ClientFolder;
            using (var sw = new StreamWriter(fullpath + CacheFile))
            {
                sw.Write(JsonConvert.SerializeObject(PatchFiles));
            }
        }

        public void LoadCache()
        {
            if (HasCache())
            {
                StreamReader file = File.OpenText(CurrentProfile.ClientFolder + CacheFile); //Open the file

                cacheFiles = JsonConvert.DeserializeObject<List<ManagedFile>>(file.ReadToEnd()); //convert
                file.Close(); //close
            }
            else
            {
                cacheFiles = new List<ManagedFile>();
            }

        }

        public void CompareCache()
        {
            foreach (ManagedFile patchFile in PatchFiles)
            {
                if (!patchFile.Download)
                    continue;

                if (FileScanned != null)
                    FileScanned(this, new ScanEventArgs(patchFile.Filename)); //Tells the form to update the progress bar
                ManagedFile currentFile =
                    cacheFiles.FirstOrDefault(x => x.Basepath + x.Filename == patchFile.Basepath + patchFile.Filename);
                if (currentFile == null) //file not in cache, download it.
                    downloadFiles.Add(patchFile);
                else
                    if (patchFile.MyHash != currentFile.MyHash)
                    {
                        currentFile.Length = patchFile.Length;
                        downloadFiles.Add(currentFile);
                    }

            }
        }

        public void CompareFiles()
        {
            foreach (ManagedFile patchFile in PatchFiles)
            {
                if (!patchFile.Download)
                    continue;

                string fullpath = CurrentProfile.ClientFolder + patchFile.Basepath + patchFile.Filename;
                if (FileScanned != null)
                    FileScanned(this, new ScanEventArgs(patchFile.Filename)); //Tells the form to update the progress bar
                var localFile = new ManagedFile(fullpath);
                localFile.Basepath = patchFile.Basepath;
                localFile.ComputeHash();
                if (patchFile.MyHash != localFile.MyHash)
                {
                    downloadFiles.Add(localFile);
                    localFile.Length = patchFile.Length;
                }
            }
        }

        public void DownloadFiles()
        {
            foreach (ManagedFile file in downloadFiles)
            {
                string temp = file.Basepath.Replace("\\", "/");
                try
                {
                    if (StartedDownload != null)
                        StartedDownload(this, new StartDownloadEventArgs(file.Filename, file.Length));
                    MyWebClient.DownloadFile(CurrentProfile.PatchBaseUrl + temp + file.Filename, CurrentProfile.ClientFolder + file.Basepath + file.Filename);
                }
                catch (WebException e)
                {
                    Console.WriteLine("WebException Handler: {0}", e);
                    return;
                }
            }
        }

        public void DownloadOneFileAsync(ManagedFile file)
        {
           string temp = file.Basepath.Replace("\\", "/");
            if (StartedDownload != null) StartedDownload(this, new StartDownloadEventArgs(file.Filename, file.Length));
            DownloadFileAsync(CurrentProfile.PatchBaseUrl + temp + file.Filename, CurrentProfile.ClientFolder + file.Basepath + file.Filename, file);
        }

        public void DownloadFilesAsync()
        {
            _retryFile = true;
            foreach (ManagedFile file in downloadFiles)
            {
               DownloadOneFileAsync(file);

                while (!_continueAsync)
                {
                    //Wait for the previous file to finish
                    Thread.Sleep(10);
                }
            }
        }

        public void DownloadFileAsync(string url, string path, ManagedFile file)
        {
            TestPath(path);
            using (var client = new WebClient())
            {
                client.Headers.Add("user-agent", UserAgentString);
                try
                {
                    client.DownloadProgressChanged += client_DownloadProgressChanged;
                    client.DownloadFileCompleted += client_DownloadFileCompleted;
                    client.DownloadFileAsync(new Uri(url), path, file);
                }
                catch (WebException e)
                {
                    Console.WriteLine("Exception: {0}", e);
                }
                _continueAsync = false;
            }
        }

        private void client_DownloadFileCompleted(object sender, AsyncCompletedEventArgs e)
        {
           ManagedFile file = (ManagedFile)e.UserState;
           if (e.Error is InvalidOperationException)
           {
               OnFailedDownload(e);
               // Try file again.
               if (_retryFile)
               {
                  _retryFile = false;
                  DownloadOneFileAsync(file);
                  return;
               }
               else
               {
                  // Patching this file failed.
                  _downloadFileFailed = true;
                  // Add the local file's hash to the patchfile, which will be saved at the end.
                  foreach (ManagedFile patchFile in PatchFiles)
                  {
                     if (patchFile.Filename == file.Filename)
                     {
                        patchFile.MyHash = file.MyHash;
                        break;
                     }
                  }
               }
           }
           else
           {
              _retryFile = true;
              OnEndDownload(e);
           }
            _continueAsync = true;
        }
        private void client_DownloadProgressChanged(object sender, DownloadProgressChangedEventArgs e)
        {
            OnProgressedDownload(e);
        }
    }
}
