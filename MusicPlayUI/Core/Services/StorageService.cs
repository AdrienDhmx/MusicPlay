using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using MusicFilesProcessor.Helpers;
using MusicFilesProcessor;
using MusicPlayModels.StatsModels;
using MusicPlayUI.Core.Helpers;

namespace MusicPlayUI.Core.Services
{
    public class StorageService
    {
        private readonly string _settingsFileName = "folders.mps";
        private readonly string _separator = "||";

        private string _settingFilePath => Path.Combine(DirectoryHelper.AppSettingsFolder, _settingsFileName);

        private static StorageService _instance;
        public static StorageService Instance
        {
            get => _instance ??= new StorageService();
        }

        private int _previousImportedTrackCount = 0;

        public List<FolderModel> Folders { get; set; }

        private Dictionary<string, FileSystemWatcher> _watchers = new Dictionary<string, FileSystemWatcher>();
        private System.Timers.Timer _timer;
        private Action fileCreatedAwaitCallBack;

        public event Action<double> ProgressUpdated;
        public event Action<int> FileImported;

        public FolderModel CurrentScannedFolder { get; set; }
        public string CurrentScannedFile { get; set; }

        private StorageService()
        {
            Init();

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

        private void Init()
        {
            Folders = new List<FolderModel>();

            DirectoryHelper.CheckDirectory(DirectoryHelper.AppSettingsFolder);
            if (!File.Exists(_settingFilePath))
            {
                File.Create(_settingFilePath).Close();
                return;
            }

            string[] lines = File.ReadAllLines(Path.Combine(DirectoryHelper.AppSettingsFolder, _settingsFileName));

            foreach (string line in lines)
            {
                string[] data = line.Split(_separator);

                if (data != null)
                {
                    FolderModel folder = new(data[1])
                    {
                        Monitored = data[2] == "1",
                        TrackImportedCount = int.Parse(data[3])
                    };
                    Folders.Add(folder);

                    if (folder.Monitored)
                    {
                        WatchFolder(folder);
                    }
                }
            }
        }

        private void WatchFolder(FolderModel folder)
        {
            FileSystemWatcher watcher = new FileSystemWatcher(folder.Path);
            watcher.EnableRaisingEvents = true;
            watcher.IncludeSubdirectories = true;
            watcher.NotifyFilter = NotifyFilters.FileName | NotifyFilters.Size;
            watcher.Created += (sender, e) => Watcher_FileCreated(sender, e, folder);
            _watchers.Add(folder.Path, watcher);
        }

        private void StopWatchingFolder(FolderModel folder)
        {
            _watchers[folder.Path].Created -= (sender, e) => Watcher_FileCreated(sender, e, folder);
            _watchers.Remove(folder.Path);
        }

        private void Watcher_FileCreated(object sender, FileSystemEventArgs e, FolderModel folder)
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

                fileCreatedAwaitCallBack = () =>
                {
                    int newTrackCount = ScanFolder(folder);
                    FileImported?.Invoke(newTrackCount);
                };

            }
        }


        public void UpdateFolder(FolderModel folder, bool monitor)
        {
            int index = Folders.FindIndex(f => f.Path == folder.Path);
            if (index == -1) return;

            if(folder.Monitored != monitor)
            {
                if (!monitor && _watchers.ContainsKey(folder.Path))
                {
                    WatchFolder(folder);
                }
                else if (monitor && _watchers.ContainsKey(folder.Path))
                {
                    StopWatchingFolder(folder);
                }

                folder.Monitored = monitor;
            }

            string[] lines = File.ReadAllLines(_settingFilePath);

            if (lines.Length > index)
            {
                string data = folder.Name + _separator + folder.Path + _separator + (folder.Monitored ? "1" : "0") + _separator + folder.TrackImportedCount;
                lines[index] = data;
            }

            File.WriteAllLines(_settingFilePath, lines);
        }

        public void RemoveFolder(FolderModel folder)
        {
            int index = Folders.FindIndex(f => f.Path == folder.Path);
            if (index == -1) return;

            if (folder.Monitored && _watchers.ContainsKey(folder.Path))
            {
                StopWatchingFolder(folder);
            }

            Folders.RemoveAt(index);

            List<string> lines = File.ReadAllLines(_settingFilePath).ToList();
            lines.RemoveAt(index);
            File.WriteAllLines(_settingFilePath, lines);
        }

        public bool AddFolder(FolderModel folder)
        {
            int index = Folders.FindIndex(f => f.Path == folder.Path);
            if (index != -1) return false; // folder already monitored

            Folders.Add(folder);
            WatchFolder(folder);

            string[] lines = File.ReadAllLines(_settingFilePath);
            string data = folder.Name + _separator + folder.Path + _separator + (folder.Monitored ? "1" : "0") + _separator + folder.TrackImportedCount;
            File.WriteAllLines(_settingFilePath, lines.Append(data));
            return true;
        }

        public int ScanFolders(bool onlyMonitoredFolder = false)
        {
            int newTrackCount = 0;
            foreach (FolderModel folder in Folders)
            {
                if (onlyMonitoredFolder && !folder.Monitored)
                    continue;
                newTrackCount += ScanFolder(folder);
            }
            return newTrackCount;
        }

        public int ScanFolder(FolderModel folder)
        {
            ImportMusicLibrary filesProcessor = new(folder.Path);
            if (filesProcessor.FileNumber > 0)
            {
                CurrentScannedFolder = folder;
                folder.Scanning = true;
                filesProcessor.ProgressChanged += () => OnProgressUpdate(filesProcessor);
                filesProcessor.Import();
                folder.Scanning = false;
                _previousImportedTrackCount = 0;
                UpdateFolder(folder, folder.Monitored);
            }
            filesProcessor.ProgressChanged -= () => OnProgressUpdate(filesProcessor);
            return filesProcessor.FileNumber;
        }

        private void OnProgressUpdate(ImportMusicLibrary importMusicLibrary)
        {
            CurrentScannedFile = importMusicLibrary.CurrentStep + "\n" + importMusicLibrary.CurrentFile;
            if (_previousImportedTrackCount < importMusicLibrary.FileImportedCount)
            {
                CurrentScannedFile = importMusicLibrary.CurrentStep; // only give keep the step because the file paths can get very long and look bad on the UI
                _previousImportedTrackCount = importMusicLibrary.FileImportedCount;
                CurrentScannedFolder.TrackImportedCount++;
            }

            ProgressUpdated?.Invoke(importMusicLibrary.ProgressPercentage);
        }
    }
}
