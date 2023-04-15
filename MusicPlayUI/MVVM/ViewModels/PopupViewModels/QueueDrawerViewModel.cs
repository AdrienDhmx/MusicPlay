using MusicPlayUI.Core.Commands;
using MusicPlayUI.Core.Enums;
using MusicPlayUI.MVVM.Models;
using System.Linq;
using System.Windows.Input;
using MusicPlayUI.Core.Services.Interfaces;
using DataBaseConnection.DataAccess;

namespace MusicPlayUI.MVVM.ViewModels.PopupViewModels
{
    public class QueueDrawerViewModel : ViewModel
    {
        private readonly INavigationService _navigationService;
        private readonly IModalService _modalService;
        private readonly IPlaylistService _playlistService;

        private IQueueService _queueService;
        public IQueueService QueueService
        {
            get { return _queueService; }
            set { SetField(ref _queueService, value); }
        }

        public ICommand RemoveTrackCommand { get; }
        public ICommand PlayTrackCommand { get; }
        public ICommand SaveQueueAsPlaylistCommand { get; }
        public ICommand OpenTrackPopupCommand { get; }
        public ICommand NavigateToPlayingFromCommand { get; }
        public ICommand NavigateToArtistCommand { get; }
        public QueueDrawerViewModel(INavigationService navigationService, IQueueService queueService, IModalService modalService, IPlaylistService playlistService)
        {
            _navigationService = navigationService;
            _queueService = queueService;
            _modalService = modalService;
            _playlistService = playlistService;

            RemoveTrackCommand = new RelayCommand<UIOrderedTrackModel>((track) => _queueService.RemoveTrack(track));
            PlayTrackCommand = new RelayCommand<UIOrderedTrackModel>((track) => _queueService.PlayTrack(track));
            OpenTrackPopupCommand = new RelayCommand<UIOrderedTrackModel>((track) => _navigationService.OpenPopup(ViewNameEnum.TrackPopup, track));
            SaveQueueAsPlaylistCommand = new RelayCommand(() =>_modalService.OpenModal(ViewNameEnum.CreatePlaylist, OnCreatePlaylistClosed));
            NavigateToPlayingFromCommand = new RelayCommand(_queueService.NavigateToPlayingFrom);
            NavigateToArtistCommand = new RelayCommand<int>(async (id) => _navigationService.NavigateTo(ViewNameEnum.SpecificArtist, await DataAccess.Connection.GetArtist(id)));
        }

        private void OnCreatePlaylistClosed(bool isCanceled)
        {
            _playlistService.OnCreatePlaylistClosed(isCanceled, _queueService.QueueTracks.ToList().ToTrackModel());
        }
    }
}
