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
    //Event when we Make progress in a Download, used to notify UI.
    public delegate void ProgressDownloadEventHandler(object sender, DownloadProgressChangedEventArgs e);
    //Event when we Complete a Download, used to notify UI.
    public delegate void EndDownloadEventHandler(object sender, AsyncCompletedEventArgs e);
    public delegate void FailDownloadHandler(object sender, AsyncCompletedEventArgs e);

    #endregion

    abstract class ClientPatcher
    {
        const string CacheFile = "\\cache.txt";
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

        private void CreateNewClient()
        {
            var wc = new WebClient();
            wc.Headers.Add("user-agent", UserAgentString);
            
            CreateFolderStructure();

            ManagedFile file = new ManagedFile(Path.GetFileName(CurrentProfile.FullInstallUrl));

            // TODO: Where do we get the hash and size from?
            file.Length = 493663553;
            file.MyHash = "54238DFC713DC5A7EA4ED0CB6D25C91D";

            try
            {
                if (StartedDownload != null)
                    StartedDownload(this, new StartDownloadEventArgs(file.Filename, file.Length));

                wc.DownloadFile(CurrentProfile.FullInstallUrl, CurrentProfile.ClientFolder + file.Basepath + file.Filename);
                
                UnZip(CurrentProfile.ClientFolder + file.Basepath + file.Filename, CurrentProfile.ClientFolder + file.Basepath);
                File.Delete(CurrentProfile.ClientFolder + file.Basepath + file.Filename);
            }
            catch (WebException e)
            {
                Console.WriteLine("WebException Handler: {0}", e);
                return;
            }
        }

        public void UnZip(string zipFile, string folderPath)
        {
            if (!File.Exists(zipFile))
                throw new FileNotFoundException();

            if (!Directory.Exists(folderPath))
                Directory.CreateDirectory(folderPath);

            Shell objShell = new Shell();
            Folder destinationFolder = objShell.NameSpace(folderPath);
            Folder sourceFile = objShell.NameSpace(zipFile);

            foreach (var file in sourceFile.Items())
            {
                destinationFolder.CopyHere(file, 4 | 16);
            }
        }

        protected abstract bool IsNewClient();

        public void ScanClient()
        {
            if (IsNewClient())
            {
                CreateNewClient();
            }
            CompareFiles();
        }

        public void GenerateCache()
        {
            string fullpath = CurrentProfile.ClientFolder;
            var scanner = new ClientScanner(fullpath);
            scanner.ScanSource();
            using (var sw = new StreamWriter(fullpath + CacheFile))
            {
                sw.Write(scanner.ToJson());
            }    
        }

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
                string fullpath = CurrentProfile.ClientFolder + patchFile.Basepath + patchFile.Filename;
                if (FileScanned != null)
                    FileScanned(this, new ScanEventArgs(patchFile.Filename)); //Tells the form to update the progress bar
                var localFile = new ManagedFile(fullpath);
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
