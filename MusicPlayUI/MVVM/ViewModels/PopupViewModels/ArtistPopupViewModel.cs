using DataBaseConnection.DataAccess;
using MusicPlayModels.MusicModels;
using MusicPlayUI.Core.Commands;
using MusicPlayUI.Core.Enums;
using System;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using MusicPlayUI.Core.Services;
using MusicPlayUI.Core.Services.Interfaces;
using MusicPlayUI.Core.Helpers;
using System.Collections.ObjectModel;
using MusicPlayUI.Core.Factories;
using MusicPlayModels.Enums;

namespace MusicPlayUI.MVVM.ViewModels.PopupViewModels
{
    public class ArtistPopupViewModel : ViewModel
    {
        private readonly INavigationService _navigationService;
        private readonly IQueueService _queueService;
        private readonly IModalService _modalService;
        private readonly IPlaylistService _playlistService;

        private ArtistModel _artist;
        public ArtistModel Artist
        {
            get { return _artist; }
            set
            {
                _artist = value;
                OnPropertyChanged(nameof(Artist));
            }
        }

        private ObservableCollection<PlaylistModel> _userPlaylists;
        public ObservableCollection<PlaylistModel> UserPlaylists
        {
            get => _userPlaylists;
            set
            {
                _userPlaylists = value;
                OnPropertyChanged(nameof(UserPlaylists));
            }
        }

        public ICommand PlayNextCommand { get; }
        public ICommand AddToQueueCommand { get; }
        public ICommand AddToPlaylistCommand { get; }
        public ICommand ChangeCoverCommand { get; }
        public ICommand CreatePlaylistCommand { get; }

        public ArtistPopupViewModel(INavigationService navigationService, IQueueService queueService, IModalService modalService, IPlaylistService playlistService)
        {
            _navigationService = navigationService;
            _queueService = queueService;
            _modalService = modalService;
            _playlistService = playlistService;

            LoadData();

            PlayNextCommand = new RelayCommand(() => PlayNext());
            AddToQueueCommand = new RelayCommand(() => PlayNext(true));
            AddToPlaylistCommand = new RelayCommand<PlaylistModel>((playlist) => AddToPlaylist(playlist));
            ChangeCoverCommand = new RelayCommand(ChangeCover);
            CreatePlaylistCommand = new RelayCommand(() => _modalService.OpenModal(ViewNameEnum.CreatePlaylist, OnCreatePlaylistClosed));
        }

        public override void Dispose()
        {
        }

        private async void PlayNext(bool end = false)
        {
            _queueService.AddTracks(await (await ArtistServices.GetArtistTracks(Artist.Id)).GetAlbumTrackProperties(), end, false, Artist.Name);
        }

        private async void AddToPlaylist(PlaylistModel playlist)
        {
            _playlistService.AddToPlaylist(await ArtistServices.GetArtistTracks(Artist.Id), playlist);

            UserPlaylists.Remove(playlist);

            if (_navigationService.CurrentViewParameter is PlaylistModel playlistModel  && playlistModel.PlaylistType == PlaylistTypeEnum.UserPlaylist)
            {
                _navigationService.CurrentViewModel.Update();
            }
        }

        private async void OnCreatePlaylistClosed(bool isCanceled)
        {
            if (!_modalService.IsModalOpen && !isCanceled)
            {
                await Task.Delay(500);

                var playlists = await DataAccess.Connection.GetAllPlaylists();
                playlists.ToList().Sort((x, y) => x.CreationDate.CompareTo(y.CreationDate));
                PlaylistModel createdPlaylist = playlists.LastOrDefault();
                AddToPlaylist(createdPlaylist);
            }
        }

        private void ChangeCover()
        {
            bool result = Artist.ChangeCover();
            if (result)
            {
                UpdateView();
            }
        }

        private void UpdateView()
        {
            if(_navigationService.CurrentViewName == ViewNameEnum.Artists || _navigationService.CurrentViewName == ViewNameEnum.SpecificArtist)
            {
                _navigationService.CurrentViewModel.Update();
            }
        }

        private async Task GetUserPlaylists()
        {
            UserPlaylists = new(await DataAccess.Connection.GetAllPlaylists());
        }

        private async void LoadData()
        {
            Artist = (ArtistModel)_navigationService.PopupViewParameter;
            await GetUserPlaylists();
        }
    }
}
