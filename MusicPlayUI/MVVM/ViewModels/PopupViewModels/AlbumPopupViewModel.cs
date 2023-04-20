using DataBaseConnection.DataAccess;
using MusicPlayModels.Enums;
using MusicPlayModels.MusicModels;
using MusicPlayUI.Core.Commands;
using MusicPlayUI.Core.Services;
using MusicPlayUI.Core.Enums;
using MusicPlayUI.Core.Factories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using MusicPlayUI.Core.Services.Interfaces;
using MusicPlayUI.Core.Helpers;
using System.Collections.ObjectModel;
using MessageControl;
using TagLib;
using MusicPlayUI.MVVM.Models;
using Humanizer.Localisation;

namespace MusicPlayUI.MVVM.ViewModels.PopupViewModels
{
    public class AlbumPopupViewModel : ViewModel
    {
        private readonly INavigationService _navigationService;
        private readonly IQueueService _queueService;
        private readonly IModalService _modalService;
        private readonly IPlaylistService _playlistService;
        private readonly ICommandsManager _commandsManager;
        private AlbumModel _album;
        public AlbumModel Album
        {
            get { return _album; }
            set
            {
                _album = value;
                OnPropertyChanged(nameof(Album));
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

        private bool _canRemoveFromGenre = false;
        public bool CanRemoveFromGenre
        {
            get => _canRemoveFromGenre;
            set
            {
                SetField(ref _canRemoveFromGenre, value);
            }
        }

        private GenreModel _genre;
        public GenreModel Genre
        {
            get => _genre;
            set
            {
                SetField(ref _genre, value);
            }
        }

        private MessageCancelClosedModel<GenreModel> _genreMessageCancelClosedModel { get; set; }
        private int RemovedAlbumGenreIndex { get; set; }

        public ICommand PlayNextCommand { get; }
        public ICommand AddToQueueCommand { get; }
        public ICommand AddToPlaylistCommand { get; }
        public ICommand RemoveAlbumGenreCommand { get; }
        public ICommand ChangeCoverCommand { get; }
        public ICommand CreatePlaylistCommand { get; }
        public ICommand NavigateToArtistCommand { get; }
        public AlbumPopupViewModel(INavigationService navigationService, IQueueService queueService, IModalService modalService, IPlaylistService playlistService, ICommandsManager commandsManager)
        {
            _navigationService = navigationService;
            _queueService = queueService;
            _modalService = modalService;
            _playlistService = playlistService;
            _commandsManager = commandsManager;
            LoadData();

            PlayNextCommand = new RelayCommand(() => PlayNext());
            AddToQueueCommand = new RelayCommand(() => PlayNext(true));
            AddToPlaylistCommand = new RelayCommand<PlaylistModel>((playlist) => AddToPlaylist(playlist));
            CreatePlaylistCommand = new RelayCommand(CreatePlaylist);
            ChangeCoverCommand = new RelayCommand(ChangeCover);
            NavigateToArtistCommand = _commandsManager.NavigateToArtistByIdCommand;
            RemoveAlbumGenreCommand = new RelayCommand(() =>
            {
                if (_navigationService.CurrentViewModel is GenreViewModel genreViewModel)
                {
                    RemovedAlbumGenreIndex = genreViewModel.RemoveAlbum(Album.Id);
                }

                _genreMessageCancelClosedModel = new(Genre, RestoreGenre);
                MessageHelper.PublishMessage(MessageFactory.AlbumRemovedFromGenre(Album.Name, Genre.Name, _genreMessageCancelClosedModel.Cancel, RemoveFromGenreCloseCallBack));
            });
        }

        private bool RestoreGenre()
        {
            if(_navigationService.CurrentViewModel is GenreViewModel genreViewModel &&
                RemovedAlbumGenreIndex != -1)
            {
                genreViewModel.Albums.Insert(RemovedAlbumGenreIndex, Album);
            }
            return true;
        }

        private async void RemoveFromGenreCloseCallBack()
        {
            if (!_genreMessageCancelClosedModel.IsCanceled)
            { 
                // real deletion
                await DataAccess.Connection.RemoveAlbumGenre(Album.Id, _genreMessageCancelClosedModel.Data.Id);
            }
        }

        private async void PlayNext(bool end = false)
        {
            _queueService.AddTracks(await (await DataAccess.Connection.GetTracksFromAlbum(Album.Id)).GetAlbumTrackProperties(), end, true, Album.Name);
        }

        private async void AddToPlaylist(PlaylistModel playlist)
        {
            _playlistService.AddToPlaylist(await DataAccess.Connection.GetTracksFromAlbum(Album.Id), playlist);

            UserPlaylists.Remove(playlist);

            if (_navigationService.CurrentViewParameter is PlaylistModel playlistModel && playlistModel.PlaylistType == PlaylistTypeEnum.UserPlaylist)
            {
                _navigationService.CurrentViewModel.Update();
            }
        }

        private void CreatePlaylist()
        {
            _modalService.OpenModal(ViewNameEnum.CreatePlaylist, OnCreatePlaylistClosed);
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
            bool result = Album.ChangeCover();
            if (result)
            {
                UpdateView();
            }
        }

        private void UpdateView()
        {
            if (_navigationService.CurrentViewName == ViewNameEnum.Albums || _navigationService.CurrentViewName == ViewNameEnum.SpecificAlbum)
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
            Album = (AlbumModel)_navigationService.PopupViewParameter;

            if (_navigationService.CurrentViewModel is GenreViewModel genreViewModel)
            {
                CanRemoveFromGenre = true;
                Genre = genreViewModel.Genre;
            }

            await GetUserPlaylists();
        }
    }
}
