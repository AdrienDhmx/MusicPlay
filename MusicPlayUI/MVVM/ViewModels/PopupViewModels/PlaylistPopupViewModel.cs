using DataBaseConnection.DataAccess;
using MusicPlayModels.MusicModels;
using MusicPlayModels.Enums;
using MusicPlayUI.Core.Enums;
using MusicPlayUI.Core.Factories;
using System.Collections.Generic;
using System.Windows.Input;
using System.IO;
using MusicPlayUI.Core.Commands;
using MusicPlayUI.Core.Services.Interfaces;
using MusicPlayUI.Core.Helpers;
using MusicPlayUI.MVVM.Models;
using MusicPlayUI.Core.Services;
using MessageControl;
using System.Linq;

namespace MusicPlayUI.MVVM.ViewModels.PopupViewModels
{
    public class PlaylistPopupViewModel : TagTargetPopupViewModel
    {
        private readonly IQueueService _queueService;
        private readonly IPlaylistService _playlistService;

        private PlaylistModel _playlist;
        public PlaylistModel Playlist
        {
            get { return _playlist; }
            set
            {
                _playlist = value;
                OnPropertyChanged(nameof(Playlist));
            }
        }

        private string _cover = "";
        public string Cover
        {
            get { return _cover; }
            set
            {
                _cover = value;
                OnPropertyChanged(nameof(Cover));
            }
        }

        private string _pathCover = "";
        public string PathCover
        {
            get { return _pathCover; }
            set
            {
                _pathCover = value;
                OnPropertyChanged(nameof(PathCover));
            }
        }


        private bool _showPathCover = false;
        public bool ShowPathCover
        {
            get { return _showPathCover; }
            set
            {
                _showPathCover = value;
                OnPropertyChanged(nameof(ShowPathCover));
            }
        }

        private bool _isAutoPlaylist;
        public bool IsAutoPlaylist
        {
            get => _isAutoPlaylist;
            set
            {
                _isAutoPlaylist = value;
                OnPropertyChanged(nameof(IsAutoPlaylist));
            }
        }

        public ICommand PlayNextCommand { get; }
        public ICommand AddToQueueCommand { get; }
        public ICommand DeletePlaylistCommand { get; }
        public ICommand EditPlaylistCommand { get; }
        public ICommand AddToTagCommand { get; }
        public ICommand CreateTagCommand { get; }
        public PlaylistPopupViewModel(INavigationService navigationService, IQueueService queueService, IModalService modalService, 
            IPlaylistService playlistService) : base(navigationService, modalService)
        {

            _queueService = queueService;
            _playlistService = playlistService;

            PlayNextCommand = new RelayCommand(() => PlayNext());
            AddToQueueCommand = new RelayCommand(() => PlayNext(true));
            DeletePlaylistCommand = new RelayCommand(()=> _modalService.OpenModal(ViewNameEnum.ConfirmAction, DeletePlaylist, ConfirmActionModelFactory.CreateConfirmDeleteModel(Playlist.Name, ModelTypeEnum.Playlist)));
            EditPlaylistCommand = new RelayCommand(EditPlaylist);
            AddToTagCommand = new RelayCommand<TagModel>((tag) => AddToTag(tag, Playlist));
            CreateTagCommand = new RelayCommand(() => CreateTag(Playlist));

            LoadData();
        }

        public override void Dispose()
        {
            base.Dispose();
        }

        private async void PlayNext(bool end = false)
        {
            List<OrderedTrackModel> tracks;
            if (!IsAutoPlaylist)
                tracks = await DataAccess.Connection.GetTracksFromPlaylist(Playlist.Id);
            else
                tracks = Playlist.Tracks;
            tracks = await tracks.GetAlbumTrackProperties();
            _queueService.AddTracks(tracks.ToTrackModel(), end);
        }

        private async void DeletePlaylist(bool canceled)
        {
            if (!canceled)
            {
                await DataAccess.Connection.DeletePlaylist(Playlist.Id);

                _playlistService.UpdateView(true);
                MessageHelper.PublishMessage(MessageFactory.DataDeleted(Playlist.Name));

                _navigationService.ClosePopup();
            }
        }

        private void EditPlaylist()
        {
            _modalService.OpenModal(ViewNameEnum.CreatePlaylist, OnEditPlaylistClosed, Playlist);
        }

        private void OnEditPlaylistClosed(bool canceled)
        {
            if (!canceled)
            {
                _playlistService.UpdateView(false);
            }
        }

        private async void LoadData()
        {
            Playlist = (PlaylistModel)_navigationService.PopupViewParameter;
            if(Playlist is not null)
            {
                if (Playlist.PlaylistType == PlaylistTypeEnum.LastPlayed ||
                    Playlist.PlaylistType == PlaylistTypeEnum.MostPlayed ||
                    Playlist.PlaylistType == PlaylistTypeEnum.Favorite)
                {
                    IsAutoPlaylist = true;
                    ShowPathCover = true;
                    PathCover = Playlist.Cover;
                }
                else
                {
                    Cover = Playlist.Cover;
                    if (Playlist.PlaylistType == PlaylistTypeEnum.Radio)
                    {
                        IsAutoPlaylist = true;
                    }
                    else
                    {
                        Playlist.Tags = await DataAccess.Connection.GetPlaylistTag(Playlist.Id);
                        await GetTags(Playlist.Tags.Select(t => t.Id));
                    }
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
