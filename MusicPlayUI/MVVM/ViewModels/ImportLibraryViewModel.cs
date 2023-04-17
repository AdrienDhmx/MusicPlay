using AudioHandler;
using DataBaseConnection.DataAccess;
using MessageControl;
using MusicFilesProcessor;
using MusicFilesProcessor.Helpers;
using MusicPlay.Language;
using MusicPlayUI.Core.Commands;
using MusicPlayUI.Core.Enums;
using MusicPlayUI.Core.Factories;
using MusicPlayUI.Core.Services;
using MusicPlayUI.Core.Services.Interfaces;
using MusicPlayUI.MVVM.Models;
using System;
using System.IO;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace MusicPlayUI.MVVM.ViewModels
{
    public class ImportLibraryViewModel : ViewModel
    {
        private readonly IAudioPlayback _audioPlayback;
        private readonly IQueueService _queueService;
        private readonly IModalService _modalService;

        public ImportMusicLibrary ImportMusic { get; private set; }

        private string _currentDirectory = DirectoryHelper.MusicFolder;
        public string CurrentDirectory
        {
            get { return _currentDirectory; }
            set
            {
                _currentDirectory = value;
                OnPropertyChanged(nameof(CurrentDirectory));
            }
        }

        private string _changedDirectory = "";
        public string ChangedDirectory
        {
            get { return _changedDirectory; }
            set
            {
                _changedDirectory = value;
                OnPropertyChanged(nameof(ChangedDirectory));
            }
        }

        private int _importValue = 0;
        public int ImportValue
        {
            get { return _importValue; }
            set
            {
                _importValue = value;
                OnPropertyChanged(nameof(ImportValue));
            }
        }

        private int _maximumValue = 100;
        public int MaximumValue
        {
            get { return _maximumValue; }
            set
            {
                _maximumValue = value;
                OnPropertyChanged(nameof(MaximumValue));
            }
        }

        private string _currentFile = "";
        public string CurrentFile
        {
            get { return _currentFile; }
            set
            {
                _currentFile = value;
                OnPropertyChanged(nameof(CurrentFile));
            }
        }

        private string _percentage;
        public string Percentage
        {
            get { return _percentage; }
            set
            {
                _percentage = value;
                OnPropertyChanged(nameof(Percentage));
            }
        }

        private bool _deleteCovers = true;
        public bool DeleteCovers
        {
            get { return _deleteCovers; }
            set
            {
                _deleteCovers = value;
                OnPropertyChanged(nameof(DeleteCovers));
            }
        }

        public ICommand ScanDirectoryCommand { get; }
        public ICommand ClearDataBaseCommand { get; }
        public ICommand ApplyDirectoryCommand { get; }
        public ImportLibraryViewModel(IAudioPlayback audioPlayback, IQueueService queueService, IModalService modalService)
        {
            _audioPlayback = audioPlayback;
            _queueService = queueService;
            
            _modalService = modalService;

            ScanDirectoryCommand = new RelayCommand(() =>
            {
                  ScanDirectory();
            });

            ClearDataBaseCommand = new RelayCommand(() => _modalService.OpenModal(ViewNameEnum.ConfirmAction, ClearDataBase, ConfirmActionModelFactory.CreateConfirmClearDataBaseModel()));

            ApplyDirectoryCommand = new RelayCommand(() =>
            {
                FolderDialogue folderDialogue = new();
                folderDialogue.InputPath = CurrentDirectory;
                folderDialogue.Title = "Choose a Folder";

                if (folderDialogue.ShowDialog() == true)
                {
                    string newDir = folderDialogue.ResultPath;
                    if (Directory.Exists(newDir))
                    {
                        CurrentDirectory = newDir;
                    }
                }
            });
        }

        private void ClearDataBase(bool canceled)
        {
            if (!canceled)
            {
                _audioPlayback.Dispose();
                _queueService.DeleteQueue();

                DataAccess.Connection.ClearDataBase();

                if (DeleteCovers)
                {
                    foreach (string file in Directory.EnumerateFiles(DirectoryHelper.AppCoverFolder, "*.*", SearchOption.AllDirectories))
                    {
                        try
                        {
                            File.Delete(file);
                        }
                        catch (Exception ex)
                        {
                            Application.Current.Dispatcher.Invoke(() =>
                            {
                                MessageHelper.PublishMessage(DefaultMessageFactory.CreateErrorMessage($"Error while deleting a file: {ex}"));
                            });
                        }
                    } 
                }

                // reset filters
                ConfigurationService.SetPreference(SettingsEnum.AlbumFilter, "", true);
                ConfigurationService.SetPreference(SettingsEnum.ArtistFilter, "", true);


                MessageHelper.PublishMessage(MessageFactory.DataBaseCleared());
            }
        }

        private async void ScanDirectory()
        {
            ImportMusic = new(CurrentDirectory);
            ImportMusic.ProgressChanged += OnProgressChanged;
            int fileNumber = ImportMusic.FileNumber;
            MaximumValue = ImportMusic.TotalProgress;

            await Task.Run(ImportMusic.Import);

            OnProgressChanged(); // update one last time the UI

            MessageHelper.PublishMessage(MessageFactory.ScanDone(fileNumber));
            ImportValue = 0;
        }

        private void OnProgressChanged()
        {
            if (MaximumValue == 0) return;
            else if(MaximumValue != ImportMusic.TotalProgress)
            {
                MaximumValue = ImportMusic.TotalProgress;
            }

            ImportValue = ImportMusic.Progress;
            CurrentFile = ImportMusic.CurrentStep + "\n" + ImportMusic.CurrentFile;
            Percentage = ImportMusic.ProgressPercentage.ToString() + "%";
        }
    }
}
