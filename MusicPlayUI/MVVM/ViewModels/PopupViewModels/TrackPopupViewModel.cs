using DataBaseConnection.DataAccess;
using MusicPlayModels.MusicModels;
using MusicPlayModels.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using MusicPlayUI.Core.Services;
using MusicPlayUI.Core.Factories;
using MusicPlayUI.Core.Commands;
using MusicPlayUI.Core.Enums;
using System.Collections.ObjectModel;
using MusicPlayUI.Core.Services.Interfaces;
using MessageControl;
using MusicPlayUI.MVVM.Models;
using MusicPlayUI.Core.Helpers;
using Windows.Media.Playlists;

namespace MusicPlayUI.MVVM.ViewModels.PopupViewModels
{
    public class TrackPopupViewModel : TagTargetPopupViewModel
    {
        private readonly IQueueService _queueService;
        public IQueueService QueueService { get { return _queueService; } }
        
        private readonly IWindowService _windowService;

        private UIOrderedTrackModel _selectedTrack;
        public UIOrderedTrackModel SelectedTrack
        {
            get { return _selectedTrack; }
            set
            {
                _selectedTrack = value;
                OnPropertyChanged(nameof(SelectedTrack));
            }
        }

        private string _cover;
        public string Cover
        {
            get { return _cover; }
            set
            {
                _cover = value;
                OnPropertyChanged(nameof(Cover));
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

        private string _album;
        public string Album
        {
            get { return _album; }
            set
            {
                _album = value;
                OnPropertyChanged(nameof(Album));
            }
        }

        private bool _isPlaylistPopupOpen = false;
        public bool IsPlaylistPopupOpen
        {
            get => _isPlaylistPopupOpen;
            set
            {
                _isPlaylistPopupOpen = value;
                OnPropertyChanged(nameof(IsPlaylistPopupOpen));
            }
        }

        private bool _removeFromPlaylistVisibility = false;
        public bool RemoveFromPlaylistVisibility
        {
            get { return _removeFromPlaylistVisibility; }
            set
            {
                _removeFromPlaylistVisibility = value;
                OnPropertyChanged(nameof(RemoveFromPlaylistVisibility));
            }
        }

        public ICommand PlayNextCommand { get; }
        public ICommand AddToQueueCommand { get; }
        public ICommand AddToPlaylistCommand { get; }
        public ICommand CreatePlaylistCommand { get; }
        public ICommand RemoveFromPlaylistCommand { get; }
        public ICommand ChangeArtworkCommand { get; }
        public ICommand OpenTagWindowCommand { get; }
        public ICommand NavigateToArtistCommand { get; }
        public ICommand AddToTagCommand { get; }
        public ICommand CreateTagCommand { get; }
        public TrackPopupViewModel(INavigationService navigationService, IQueueService queueService, IModalService modalService, 
             IWindowService windowService) : base(navigationService, modalService)
        {
            _queueService = queueService;            
            _windowService = windowService;

            PlayNextCommand = new RelayCommand(PlayNext);
            AddToQueueCommand = new RelayCommand(AddToQueue);
            AddToPlaylistCommand = new RelayCommand<PlaylistModel>((playlist) => AddToPlaylist(playlist));
            CreatePlaylistCommand = new RelayCommand(CreatePlaylist);
            RemoveFromPlaylistCommand = new RelayCommand(RemoveFromPlaylist);
            ChangeArtworkCommand = new RelayCommand(ChangeArtwork);
            AddToTagCommand = new RelayCommand<TagModel>((tag) => AddToTag(tag, SelectedTrack));
            CreateTagCommand = new RelayCommand(() => CreateTag(SelectedTrack));
            OpenTagWindowCommand = new RelayCommand(() =>
            {
                _windowService.OpenWindow(ViewNameEnum.TrackProperties, SelectedTrack);
            });
            NavigateToArtistCommand = new RelayCommand<int>(async (id) => _navigationService.NavigateTo(ViewNameEnum.SpecificArtist, await DataAccess.Connection.GetArtist(id)));

            Task.Run(LoadData);
        }

        public override void Dispose()
        {

        }

        private void PlayNext()
        {
            _queueService.AddTrack(SelectedTrack);
        }

        private void AddToQueue()
        {
            _queueService.AddTrack(SelectedTrack, true);
        }

        private async void AddToPlaylist(PlaylistModel playlist)
        {
            int index = (await DataAccess.Connection.GetTracksFromPlaylist(playlist.Id)).Count + 1;
            await DataAccess.Connection.AddTrackToPlaylist(playlist, SelectedTrack, index);
            MessageHelper.PublishMessage(SelectedTrack.Title.TrackAddedToPlaylist(playlist.Name));

            UserPlaylists.Remove(playlist);

            if(_navigationService.CurrentViewParameter is PlaylistModel playlistModel && playlistModel.PlaylistType == PlaylistTypeEnum.UserPlaylist)
            {
                _navigationService.CurrentViewModel.Update();
            }
        }

        private void CreatePlaylist()
        {
            IsPlaylistPopupOpen = false;
            _modalService.OpenModal(ViewNameEnum.CreatePlaylist, OnCreatePlaylistValidation);
        }

        private async void OnCreatePlaylistValidation(bool isCanceled)
        {
            if (!isCanceled)
            {
                var playlists = await DataAccess.Connection.GetAllPlaylists();
                playlists.ToList().Sort((x, y) => x.CreationDate.CompareTo(y.CreationDate));

                PlaylistModel createdPlaylist = playlists.LastOrDefault();
                AddToPlaylist(createdPlaylist);
            }
        }

        private async void RemoveFromPlaylist()
        {
            PlaylistModel playlist = (PlaylistModel)_navigationService.CurrentViewParameter;
            if (playlist is not null)
            {
                await DataAccess.Connection.RemoveTrackFromPlaylist(playlist, SelectedTrack);
                List<OrderedTrackModel> playlistTracks = await DataAccess.Connection.GetTracksFromPlaylist(playlist.Id);

                playlistTracks = playlistTracks.ToOrderedTrackModel(); // update all index correctly
                await DataAccess.Connection.UpdatePlaylistTracks(playlist.Id, playlistTracks);

                MessageHelper.PublishMessage(SelectedTrack.Title.TrackRemovedFromPlaylist(playlist.Name));

                // not in the playlist anymore
                RemoveFromPlaylistVisibility = false;
                UserPlaylists.Add(playlist);

                // if the button to remove from playlist is visible then the view is a the playlist the tracks has been removed from
                _navigationService.CurrentViewModel.Update();
            }
        }

        private void ChangeArtwork()
        {
            SelectedTrack.ChangeCover();
        }

        private async Task GetUserPlaylists()
        {
            var playlists = await DataAccess.Connection.GetAllPlaylists();
            var trackPlaylists = await DataAccess.Connection.GetTrackPlaylistsIds(SelectedTrack.Id);

            UserPlaylists = new(playlists.ToList().ExceptBy(trackPlaylists, p => p.Id));
        }

        private async void LoadData()
        {
            TrackModel track = (TrackModel)_navigationService.PopupViewParameter;
            if(track is not null)
            {
                track = await track.GetAlbumTrackProperties();
                SelectedTrack = new(track, _queueService.AlbumCoverOnly, _queueService.AutoCover);
                SelectedTrack.Tags = await DataAccess.Connection.GetTrackTag(SelectedTrack.Id);

                await GetUserPlaylists();
                await GetTags(SelectedTrack.Tags.Select(t => t.Id));

                // visible if the view is a playlist and the playlist type is a user one
                if (_navigationService.CurrentViewParameter is PlaylistModel playlist
                    && playlist.PlaylistType == PlaylistTypeEnum.UserPlaylist)
                {
                    List<OrderedTrackModel> tracks = await DataAccess.Connection.GetTracksFromPlaylist(playlist.Id);
                    RemoveFromPlaylistVisibility = tracks.Any(t => t.Id == SelectedTrack.Id);
                }
                else
                {
                    RemoveFromPlaylistVisibility = false;
                }
            }
            else
            {
                // error
                _navigationService.ClosePopup();
            }
        }
    }
}
