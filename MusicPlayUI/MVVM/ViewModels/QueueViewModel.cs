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
    public class QueueViewModel : BaseQueueViewModel
    {
        private readonly IModalService _modalService;
        private readonly ICommandsManager _commandsManager;
        private readonly IPlaylistService _playlistService;

        private IVisualizerParameterStore _visualizerParameterService;
        public IVisualizerParameterStore VisualizerParameterService
        {
            get => _visualizerParameterService;
            set { SetField(ref _visualizerParameterService, value); }
        }

        public ICommand RemoveTrackCommand { get; }
        public ICommand PlayTrackCommand { get; }
        public ICommand SaveQueueAsPlaylistCommand { get; }
        public ICommand OpenTrackPopupCommand { get; }
        public ICommand NavigateToPlayingFromCommand { get; }
        public ICommand NavigateToArtistCommand { get; }
        public ICommand NavigateToAlbumCommand { get; }
        public QueueViewModel(IQueueService queueService, IAudioTimeService audioService, IModalService modalService, ICommandsManager commandsManager,
             IPlaylistService playlistService, IVisualizerParameterStore visualizerParameterStore) : base(queueService, audioService)
        {
            _modalService = modalService;
            _commandsManager = commandsManager;
            _playlistService = playlistService;
            _visualizerParameterService = visualizerParameterStore;

            RemoveTrackCommand = new RelayCommand<Track>(QueueService.RemoveTrack);
            PlayTrackCommand = new RelayCommand<Track>(QueueService.PlayTrack);
            OpenTrackPopupCommand = _commandsManager.OpenTrackPopupCommand;
            SaveQueueAsPlaylistCommand = new RelayCommand(() => _modalService.OpenModal(ViewNameEnum.CreatePlaylist, OnCreatePlaylistValidation));
            NavigateToPlayingFromCommand = new RelayCommand(async () => await _queueService.NavigateToPlayingFrom());
            NavigateToArtistCommand = _commandsManager.NavigateToArtistByIdCommand;
            NavigateToAlbumCommand = _commandsManager.NavigateToAlbumByIdCommand;

            base.Init();
        }

        private void OnCreatePlaylistValidation(bool isCanceled)
        {
            _playlistService.OnCreatePlaylistClosed(isCanceled, _queueService.Queue.Tracks.ToList().ToTrack());
        }
    }
}
