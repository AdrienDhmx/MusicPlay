using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using MusicFilesProcessor.Helpers;
using MusicFilesProcessor;
using MusicPlayUI.Core.Helpers;
using MusicPlay.Database.Models;
using System.Threading.Tasks;

using MusicPlay.Database.Helpers;
using MusicPlay.Database.DatabaseAccess;
using System.Data.Entity;

namespace MusicPlayUI.Core.Services
{
    public class StorageService
    {
        private static StorageService _instance;
        public static StorageService Instance
        {
            get => _instance ??= new StorageService();
        }

        private int _previousImportedTrackCount = 0;

        public List<Folder> Folders { get; set; }

        private Dictionary<string, FileSystemWatcher> _watchers = new Dictionary<string, FileSystemWatcher>();
        private System.Timers.Timer _timer;
        private Action fileCreatedAwaitCallBack;

        public event Action<double> ProgressUpdated;
        public event Action<int> FileImported;

        public Folder CurrentScannedFolder { get; set; }
        public string CurrentScannedFile { get; set; }

        private StorageService()
        {
            Init();

            // timer used to scan folder 10 seconds after the last added/created file in a watched folder
            _timer = new()
            {
                Interval = TimeSpan.FromSeconds(10).TotalMilliseconds,
                AutoReset = false,
                Enabled = false,
            };
            _timer.Elapsed += FileCreatedAwaitTimerCallback;
        }

        private void FileCreatedAwaitTimerCallback(object sender, System.Timers.ElapsedEventArgs e)
        {
            fileCreatedAwaitCallBack?.Invoke();
        }

        private async void Init()
        {
            Folders = await Folder.GetAll();

            foreach (Folder folder in Folders)
            {
                folder.TrackImportedCount = Track.CountFromFolder(folder.Id);
                if (folder.IsMonitored)
                {
                    WatchFolder(folder);
                }
            }
        }

        private void WatchFolder(Folder folder)
        {
            FileSystemWatcher watcher = new FileSystemWatcher(folder.Path);
            watcher.EnableRaisingEvents = true;
            watcher.IncludeSubdirectories = true;
            watcher.NotifyFilter = NotifyFilters.FileName | NotifyFilters.Size;
            watcher.Created += (sender, e) => Watcher_FileCreated(sender, e, folder);
            _watchers.Add(folder.Path, watcher);
        }

        private void StopWatchingFolder(Folder folder)
        {
            _watchers[folder.Path].Created -= (sender, e) => Watcher_FileCreated(sender, e, folder);
            _watchers.Remove(folder.Path);
        }

        private void Watcher_FileCreated(object sender, FileSystemEventArgs e, Folder folder)
        {
            string extension = Path.GetExtension(e.FullPath);

            if (Array.Exists(ImportMusicLibrary.FilesExtensions.ToArray(), ext => ext.Equals(extension, StringComparison.OrdinalIgnoreCase)))
            {
                // stop the current timer to restart a new one
                //
                // if the timer started because of another folder then it will not be scanned, but it shouldn't happened often
                // and the user can always scan the folders manually...
                if (_timer.Enabled)
                {
                    _timer.Stop();
                }

                _timer.Start();

                fileCreatedAwaitCallBack = async () =>
                {
                    int newTrackCount = await Task.Run(() => ScanFolder(folder));
                    FileImported?.Invoke(newTrackCount);
                };

            }
        }


        public void UpdateFolder(Folder folder, bool monitor, string name)
        {
            int index = Folders.FindIndex(f => f.Path == folder.Path);
            if (index == -1) return;

            if(folder.IsMonitored != monitor)
            {
                if (!monitor && _watchers.ContainsKey(folder.Path))
                {
                    StopWatchingFolder(folder);
                }
                else if (monitor && !_watchers.ContainsKey(folder.Path))
                {
                    WatchFolder(folder);
                }
            }

            Folder.Update(folder, name, monitor);
        }

        /// <summary>
        /// Add a folder to the list of folder and insert it the db
        /// </summary>
        /// <param name="folder"></param>
        /// <returns></returns>
        public async Task<bool> AddFolder(Folder folder)
        {
            int index = Folders.FindIndex(f => f.Path == folder.Path);
            if (index != -1) return false; // folder already monitored

            Folders.Add(folder);
            WatchFolder(folder);

            await Folder.Insert(folder);
            return true;
        }

        public int ScanFolders(bool onlyMonitoredFolder = false)
        {
            if (Folders.IsNullOrEmpty()) 
                return 0;

            int newTrackCount = 0;
            foreach (Folder folder in Folders)
            {
                if (onlyMonitoredFolder && !folder.IsMonitored)
                    continue;
                newTrackCount += ScanFolder(folder);
            }
            return newTrackCount;
        }

        public void DeleteFolder(Folder folder)
        {
            if(folder is null || folder.TrackImportedCount > 0)
            {
                return;
            }

            if(folder.IsMonitored && _watchers.ContainsKey(folder.Path))
            {
                StopWatchingFolder(folder);
            }

            Folders.Remove(folder);
        }

        public int ScanFolder(Folder folder)
        {
            ClearDatabaseContext(); // avoid trying to insert existing data with embedded references
            ImportMusicLibrary filesProcessor = new(folder);
            if (filesProcessor.FileNumber > 0)
            {
                CurrentScannedFolder = folder;
                folder.Scanning = true;
                filesProcessor.ProgressChanged += () => OnProgressUpdate(filesProcessor);
                filesProcessor.Import();
                folder.Scanning = false;
            }
            filesProcessor.ProgressChanged -= () => OnProgressUpdate(filesProcessor);
            _previousImportedTrackCount = 0; // reset
            return filesProcessor.FileNumber;
        }

        private void ClearDatabaseContext()
        {
            using DatabaseContext context = new();
            // only check if there are artists or albums being tracked because if they are not tracked nothing is
            if(context.Artists.Local.Count > 0 || context.Albums.Local.Count > 0)
                context.ChangeTracker.Clear();
        }

        private void OnProgressUpdate(ImportMusicLibrary importMusicLibrary)
        {
            CurrentScannedFile = importMusicLibrary.CurrentStep;
            if (_previousImportedTrackCount < importMusicLibrary.FileImportedCount)
            {
                _previousImportedTrackCount = importMusicLibrary.FileImportedCount;
                CurrentScannedFolder.TrackImportedCount++;
            }

            ProgressUpdated?.Invoke(importMusicLibrary.ProgressPercentage);
        }
    }
}
