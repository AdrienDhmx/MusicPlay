using System.Linq;
using System.Windows.Input;
using MusicPlayUI.MVVM.Models;
using MusicPlayUI.Core.Enums;
using MusicPlayUI.Core.Commands;
using MusicPlayUI.Core.Services.Interfaces;


using MusicPlay.Database.Models;
using MusicPlayUI.Core.Helpers;

namespace MusicPlayUI.MVVM.ViewModels
{
    public class QueueViewModel : ViewModel
    {
        private readonly IModalService _modalService;
        private readonly ICommandsManager _commandsManager;
        private readonly IPlaylistService _playlistService;

        private IQueueService _queueService;
        public IQueueService QueueService
        {
            get { return _queueService; }
            set { SetField(ref _queueService, value); }
        }

        private IAudioTimeService _audioService;
        public IAudioTimeService AudioService
        {
            get => _audioService;
            set { SetField(ref _audioService, value); }
        }

        public ICommand RemoveTrackCommand { get; }
        public ICommand PlayTrackCommand { get; }
        public ICommand SaveQueueAsPlaylistCommand { get; }
        public ICommand OpenTrackPopupCommand { get; }
        public ICommand NavigateToPlayingFromCommand { get; }
        public ICommand NavigateToArtistCommand { get; }
        public ICommand NavigateToAlbumCommand { get; }
        public QueueViewModel(IQueueService queueService, IAudioTimeService audioService, IModalService modalService, ICommandsManager commandsManager,
             IPlaylistService playlistService)
        {
            QueueService = queueService;
            AudioService = audioService;
            _modalService = modalService;
            _commandsManager = commandsManager;
            _playlistService = playlistService;

            RemoveTrackCommand = new RelayCommand<Track>((track) => QueueService.RemoveTrack(track));
            PlayTrackCommand = new RelayCommand<Track>((track) =>  QueueService.PlayTrack(track));
            OpenTrackPopupCommand = _commandsManager.OpenTrackPopupCommand;
            SaveQueueAsPlaylistCommand = new RelayCommand(() => _modalService.OpenModal(ViewNameEnum.CreatePlaylist, OnCreatePlaylistValidation));
            NavigateToPlayingFromCommand = new RelayCommand(async () => await _queueService.NavigateToPlayingFrom());
            NavigateToArtistCommand = _commandsManager.NavigateToArtistByIdCommand;
            NavigateToAlbumCommand = _commandsManager.NavigateToAlbumByIdCommand;
        }

        private void OnCreatePlaylistValidation(bool isCanceled)
        {
            _playlistService.OnCreatePlaylistClosed(isCanceled, _queueService.Queue.Tracks.ToList().ToTrack());
        }
    }
}
