﻿using MusicPlayUI.Core.Commands;
using MusicPlayUI.Core.Enums;
using MusicPlayUI.MVVM.Models;
using System.Linq;
using System.Windows.Input;
using MusicPlayUI.Core.Services.Interfaces;


using MusicPlay.Database.Models;
using MusicPlayUI.Core.Services;
using MusicPlayUI.Core.Helpers;
using MusicPlayUI.Controls;

namespace MusicPlayUI.MVVM.ViewModels.PopupViewModels
{
    public class QueueDrawerViewModel : BaseQueueViewModel
    {
        private readonly IModalService _modalService;
        private readonly IPlaylistService _playlistService;
        private readonly ICommandsManager _commandsManager;

        public bool AreCoversEnabled { get; } = ConfigurationService.AreCoversEnabled;

        public ICommand RemoveTrackCommand { get; }
        public ICommand PlayTrackCommand { get; }
        public ICommand SaveQueueAsPlaylistCommand { get; }
        public ICommand OpenTrackPopupCommand { get; }
        public ICommand NavigateToPlayingFromCommand { get; }
        public ICommand NavigateToArtistCommand { get; }
        public ICommand ClearQueueCommand { get; }
        public QueueDrawerViewModel(IQueueService queueService, IModalService modalService, IPlaylistService playlistService, ICommandsManager commandsManager, IAudioTimeService audioTimeService)
            : base(queueService, audioTimeService)
        {
            _modalService = modalService;
            _playlistService = playlistService;
            _commandsManager = commandsManager;

            ConfigurationService.QueueCoversChange += () => OnPropertyChanged(nameof(AreCoversEnabled));
            _queueService.QueueChanged += QueueService_QueueChanged;

            ClearQueueCommand = new RelayCommand(_queueService.ClearQueue);
            RemoveTrackCommand = new RelayCommand<Track>(_queueService.RemoveTrack);
            PlayTrackCommand = new RelayCommand<Track>(_queueService.PlayTrack);
            OpenTrackPopupCommand = _commandsManager.OpenTrackPopupCommand;
            SaveQueueAsPlaylistCommand = new RelayCommand(() =>_modalService.OpenModal(ViewNameEnum.CreatePlaylist, OnCreatePlaylistClosed));
            NavigateToPlayingFromCommand = new RelayCommand(async () => await _queueService.NavigateToPlayingFrom());
            NavigateToArtistCommand = _commandsManager.NavigateToArtistByIdCommand;

            // this does not have a state
            base._saveScrollOffset = false;
            base.Init();
        }

        private void QueueService_QueueChanged()
        {
            AsyncImage.ImageCache.Clear();
        }

        public override void Dispose()
        {
            ConfigurationService.QueueCoversChange -= () => OnPropertyChanged(nameof(AreCoversEnabled));
            _queueService.QueueChanged -= QueueService_QueueChanged;
        }

        private void OnCreatePlaylistClosed(bool isCanceled)
        {
            _playlistService.OnCreatePlaylistClosed(isCanceled, _queueService.Queue.Tracks.ToList().ToTrack());
        }
    }
}
