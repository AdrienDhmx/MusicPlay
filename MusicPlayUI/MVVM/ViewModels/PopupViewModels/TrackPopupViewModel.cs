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
using MusicPlay.Database.Models;
using MusicPlay.Database.Enums;
using MusicPlay.Database.Models.DataBaseModels;

namespace MusicPlayUI.MVVM.ViewModels.PopupViewModels
{
    public class TrackPopupViewModel : TagTargetPopupViewModel
    {
        private readonly IQueueService _queueService;
        public IQueueService QueueService { get { return _queueService; } }
        
        private readonly IWindowService _windowService;
        private readonly IRadioStationsService _radioStationsService;
        private readonly ICommandsManager _commandsManager;
        private Track _selectedTrack;
        public Track SelectedTrack
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

        private ObservableCollection<Playlist> _userPlaylists;
        public ObservableCollection<Playlist> UserPlaylists
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
        public ICommand StartRadioCommand { get; }
        public TrackPopupViewModel(IQueueService queueService, IModalService modalService, 
             IWindowService windowService, IRadioStationsService radioStationsService, ICommandsManager commandsManager) : base(modalService)
        {
            _queueService = queueService;            
            _windowService = windowService;
            _radioStationsService = radioStationsService;
            _commandsManager = commandsManager;
            
            PlayNextCommand = new RelayCommand(PlayNext);
            AddToQueueCommand = new RelayCommand(AddToQueue);
            AddToPlaylistCommand = new RelayCommand<Playlist>(AddToPlaylist);
            CreatePlaylistCommand = new RelayCommand(CreatePlaylist);
            RemoveFromPlaylistCommand = new RelayCommand(RemoveFromPlaylist);
            ChangeArtworkCommand = new RelayCommand(async () => await ChangeArtwork());
            AddToTagCommand = new RelayCommand<Tag>(async (tag) => await AddToTag(tag, SelectedTrack));
            CreateTagCommand = new RelayCommand(() => CreateTag(SelectedTrack));
            OpenTagWindowCommand = new RelayCommand(() =>
            {
                _windowService.OpenWindow(ViewNameEnum.TrackProperties, SelectedTrack);
            });
            NavigateToArtistCommand = _commandsManager.NavigateToArtistByIdCommand;

            StartRadioCommand = new RelayCommand(async () =>
            {
                Playlist radio = await _radioStationsService.CreateRadioStation(SelectedTrack);
                queueService.SetNewQueue(radio.PlaylistTracks, radio, radio.Name, radio.Cover);
                ClosePopup();
            });
        }

        public override void Dispose()
        {

        }

        private void PlayNext()
        {
            _queueService.AddTrack(SelectedTrack);
            ClosePopup();
        }

        private void AddToQueue()
        {
            _queueService.AddTrack(SelectedTrack, true);
            ClosePopup();
        }

        private void AddToPlaylist(Playlist playlist)
        {
            Playlist.AddTrack(playlist, SelectedTrack);
            MessageHelper.PublishMessage(SelectedTrack.Title.TrackAddedToPlaylist(playlist.Name));

            UserPlaylists.Remove(playlist);

            if(App.State.CurrentView.State.Parameter is Playlist playlistModel && playlistModel.PlaylistType == PlaylistTypeEnum.UserPlaylist)
            {
                App.State.CurrentView.ViewModel.Update();
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
                var playlists = await Playlist.GetAll();
                playlists.ToList().Sort((x, y) => x.CreationDate.CompareTo(y.CreationDate));

                Playlist createdPlaylist = playlists.LastOrDefault();
                AddToPlaylist(createdPlaylist);
            }
        }

        private void RemoveFromPlaylist()
        {
            Playlist playlist = (Playlist)App.State.CurrentPopup.State.Parameter;
            if (playlist is not null)
            {
                //await DataAccess.Connection.RemoveTrackFromPlaylist(playlist, SelectedTrack);
                //List<OrderedTrack> playlistTracks = await DataAccess.Connection.GetTracksFromPlaylist(playlist.Id);

                //playlistTracks = playlistTracks.ToOrderedTrackModel(); // update all index correctly
                //await DataAccess.Connection.UpdatePlaylistTracks(playlist.Id, playlistTracks);

                //MessageHelper.PublishMessage(SelectedTrack.Title.TrackRemovedFromPlaylist(playlist.Name));

                // not in the playlist anymore
                RemoveFromPlaylistVisibility = false;
                UserPlaylists.Add(playlist);

                // if the button to remove from playlist is visible then the view is a the playlist the tracks has been removed from
                App.State.CurrentView.ViewModel.Update();
                ClosePopup();
            }
        }

        private async Task ChangeArtwork()
        {
            ClosePopup();
            await SelectedTrack.ChangeCover();
        }

        private async Task GetUserPlaylists()
        {
            var playlists = await Playlist.GetAll();
            UserPlaylists = new(playlists.ExceptBy(SelectedTrack.PlaylistTracks.Select(pt => pt.PlaylistId), p => p.Id));
        }

        public override void Init()
        {
            LoadData();
        }

        private async void LoadData()
        {
            Track track = (Track)State.Parameter;
            if(track is not null)
            {
                SelectedTrack = new(track);

                await GetUserPlaylists();
                GetTags(SelectedTrack.TrackTags.Select(t => t.Tag.Id));

                // visible if the view is a playlist and the playlist type is a user one
                if (App.State.CurrentView.State.Parameter is Playlist playlist
                    && playlist.PlaylistType == PlaylistTypeEnum.UserPlaylist)
                {
                    RemoveFromPlaylistVisibility = playlist.PlaylistTracks.Any(t => t.Track.Id == SelectedTrack.Id);
                }
                else
                {
                    RemoveFromPlaylistVisibility = false;
                }
            }
            else
            {
                // error
                ClosePopup();
            }
        }
    }
}
