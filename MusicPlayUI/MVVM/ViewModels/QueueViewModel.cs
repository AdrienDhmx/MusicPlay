using MusicPlayModels.MusicModels;
using System.Linq;
using System.Windows.Input;
using MusicPlayUI.MVVM.Models;
using MusicPlayUI.Core.Enums;
using MusicPlayUI.Core.Commands;
using MusicPlayUI.Core.Services.Interfaces;
using DataBaseConnection.DataAccess;

namespace MusicPlayUI.MVVM.ViewModels
{
    public class QueueViewModel : ViewModel
    {
        private readonly IModalService _modalService;
        private readonly IPlaylistService _playlistService;

        private INavigationService _navigationService;
        public INavigationService NavigationService
        {
            get { return _navigationService; }
            set { SetField(ref _navigationService, value); }
        }

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
        public QueueViewModel(INavigationService navigationService, IQueueService queueService, IAudioTimeService audioService, IModalService modalService, 
             IPlaylistService playlistService)
        {
            NavigationService = navigationService;
            QueueService = queueService;
            AudioService = audioService;
            _modalService = modalService;
            _playlistService = playlistService;

            RemoveTrackCommand = new RelayCommand<UIOrderedTrackModel>((track) => QueueService.RemoveTrack(track));
            PlayTrackCommand = new RelayCommand<UIOrderedTrackModel>((track) =>  QueueService.PlayTrack(track));
            OpenTrackPopupCommand = new RelayCommand<UIOrderedTrackModel>((track) => NavigationService.OpenPopup(ViewNameEnum.TrackPopup, track));
            SaveQueueAsPlaylistCommand = new RelayCommand(() => _modalService.OpenModal(ViewNameEnum.CreatePlaylist, OnCreatePlaylistValidation));
            NavigateToPlayingFromCommand = new RelayCommand(_queueService.NavigateToPlayingFrom);
            NavigateToArtistCommand = new RelayCommand<int>(async (id) => _navigationService.NavigateTo(ViewNameEnum.SpecificArtist, await DataAccess.Connection.GetArtist(id)));
            NavigateToAlbumCommand = new RelayCommand<int>(async (id) => _navigationService.NavigateTo(ViewNameEnum.SpecificAlbum, await DataAccess.Connection.GetAlbum(id)));

        }

        private void OnCreatePlaylistValidation(bool isCanceled)
        {
            _playlistService.OnCreatePlaylistClosed(isCanceled, _queueService.QueueTracks.ToList().ToTrackModel());
        }
    }
}
