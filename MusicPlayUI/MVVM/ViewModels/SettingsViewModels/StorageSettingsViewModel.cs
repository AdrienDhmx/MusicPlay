using AudioHandler;
using DataBaseConnection.DataAccess;
using MessageControl;
using MusicFilesProcessor;
using MusicFilesProcessor.Helpers;
using MusicPlay.Language;
using MusicPlayModels.StatsModels;
using MusicPlayUI.Core.Commands;
using MusicPlayUI.Core.Enums;
using MusicPlayUI.Core.Factories;
using MusicPlayUI.Core.Services;
using MusicPlayUI.Core.Services.Interfaces;
using MusicPlayUI.MVVM.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Documents;
using System.Windows.Input;

namespace MusicPlayUI.MVVM.ViewModels.SettingsViewModels
{
    public class StorageSettingsViewModel : SettingsViewModel
    {
        private readonly IAudioPlayback _audioPlayback;
        private readonly IQueueService _queueService;
        private readonly IModalService _modalService;

        private StorageService _storageSettings => StorageService.Instance;

        private ObservableCollection<FolderModel> _folders;
        public ObservableCollection<FolderModel> Folders
        {
            get => _folders;
            set => SetField(ref _folders, value);
        }

        private double _importValue = 0;
        public double ImportValue
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

        private string _currentInfo = "";
        public string CurrentInfo
        {
            get { return _currentInfo; }
            set
            {
                _currentInfo = value;
                OnPropertyChanged(nameof(CurrentInfo));
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

        public ICommand AddFolderCommand { get; }
        public ICommand ScanFolderCommand { get; }
        public ICommand ScanAllFoldersCommand { get; }
        public ICommand EditFolderCommand { get; }
        public ICommand RemoveFolderCommand { get; }
        public ICommand ClearDataBaseCommand { get; }
        public StorageSettingsViewModel(IAudioPlayback audioPlayback, IQueueService queueService, IModalService modalService, INavigationService navigationService, IWindowService windowService) : base(navigationService, windowService)
        {
            _audioPlayback = audioPlayback;
            _queueService = queueService;
            _modalService = modalService;

            ScanFolderCommand = new RelayCommand<FolderModel>((folder) => Task.Run(() => _storageSettings.ScanFolder(folder)));

            ScanAllFoldersCommand = new RelayCommand(() => Task.Run(() => _storageSettings.ScanFolders()));

            EditFolderCommand = new RelayCommand<FolderModel>((folder) =>
            {
                _modalService.OpenModal(ViewNameEnum.EditFolder, (canceled) => { }, folder);
            });

            RemoveFolderCommand = new RelayCommand<FolderModel>((folder) =>
            {
                _storageSettings.RemoveFolder(folder);
                Folders = new(_storageSettings.Folders);
            });

            ClearDataBaseCommand = new RelayCommand(() => _modalService.OpenModal(ViewNameEnum.ConfirmAction, ClearDataBase, ConfirmActionModelFactory.CreateConfirmClearDataBaseModel()));

            AddFolderCommand = new RelayCommand(() =>
            {
                FolderDialogue folderDialogue = new();
                folderDialogue.InputPath = DirectoryHelper.MusicFolder;
                folderDialogue.Title = "Choose a Folder";

                if (folderDialogue.ShowDialog() == true)
                {
                    string newDir = folderDialogue.ResultPath;
                    if (Directory.Exists(newDir))
                    {
                        FolderModel folderModel = new FolderModel(newDir);
                        if (!_storageSettings.AddFolder(folderModel))
                        {
                            MessageHelper.PublishMessage(DefaultMessageFactory.CreateWarningMessage($"This folder is already monitored !"));
                        }
                        Folders = new(_storageSettings.Folders);
                    }
                }
            });

            _storageSettings.ProgressUpdated += OnProgressChanged;
            Folders = new(_storageSettings.Folders);
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
                            //Application.Current.Dispatcher.Invoke(() =>
                            //{
                            //    MessageHelper.PublishMessage(DefaultMessageFactory.CreateErrorMessage($"Error while deleting a file: {ex}"));
                            //});
                        }
                    }
                }

                // reset filters
                ConfigurationService.SetPreference(SettingsEnum.AlbumFilter, "", true);
                ConfigurationService.SetPreference(SettingsEnum.ArtistFilter, "", true);

                foreach (FolderModel folder in _storageSettings.Folders)
                {
                    folder.TrackImportedCount = 0;
                    _storageSettings.UpdateFolder(folder, folder.Monitored);
                }

                MessageHelper.PublishMessage(MessageFactory.DataBaseCleared());
            }
        }

        public override void Dispose()
        {
            _storageSettings.ProgressUpdated -= OnProgressChanged;
        }

        private void OnProgressChanged(double percentage)
        {
            ImportValue = percentage;
            CurrentInfo = _storageSettings.CurrentScannedFile;
            Percentage = ImportValue.ToString() + "%";
        }
    }
}
