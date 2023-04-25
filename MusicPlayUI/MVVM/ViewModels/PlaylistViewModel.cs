using DataBaseConnection.DataAccess;
using MusicPlayModels.Enums;
using MusicPlayModels.MusicModels;
using MusicFilesProcessor.Helpers;
using MusicPlayUI.Core.Commands;
using System.Collections.Generic;
using System.Windows.Input;
using MusicPlayUI.Core.Enums;
using MusicPlayModels;
using MusicPlayUI.Core.Services.Interfaces;
using MusicPlayUI.Core.Helpers;
using System.Collections.ObjectModel;
using System.Linq;
using GongSolutions.Wpf.DragDrop;
using System.Windows;
using MusicPlayUI.MVVM.Models;
using MusicPlayUI.Core.Services;
using DataBaseConnection.Model;
using System;

namespace MusicPlayUI.MVVM.ViewModels
{
    public class PlaylistViewModel : ViewModel, IDropTarget
    {
        private readonly INavigationService _navigationService;
        private readonly IModalService _modalService;
        private readonly IPlaylistService _playlistService;

        private IQueueService _queueService;
        public IQueueService QueueService
        {
            get => _queueService;
            set
            {
                SetField(ref _queueService, value);
            }
        }

        private PlaylistModel _playlist;
        public PlaylistModel Playlist
        {
            get => _playlist;
            set 
            {
                SetField(ref _playlist, value);
                OnPropertyChanged(nameof(IsFavoritePlaylist));
                OnPropertyChanged(nameof(IsLastPlayedPlaylist));
                OnPropertyChanged(nameof(IsMostPlayedPlaylist));
            }
        }

        private string _playlistDuration;
        public string PlaylistDuration
        {
            get => _playlistDuration;
            set { SetField(ref _playlistDuration, value); }
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

        private bool _isRadio = false;
        public bool IsRadio
        {
            get { return _isRadio; }
            set
            {
                _isRadio = value;
                OnPropertyChanged(nameof(IsRadio));
            }
        }

        public bool IsFavoritePlaylist
        {
            get => Playlist.PlaylistType == PlaylistTypeEnum.Favorite;
        }

        public bool IsLastPlayedPlaylist
        {
            get => Playlist.PlaylistType == PlaylistTypeEnum.LastPlayed;
        }

        public bool IsMostPlayedPlaylist
        {
            get => Playlist.PlaylistType == PlaylistTypeEnum.MostPlayed;
        }

        private bool _descriptionVisibility = true;
        public bool DescriptionVisibility
        {
            get { return _descriptionVisibility; }
            set
            {
                _descriptionVisibility = value;
                OnPropertyChanged(nameof(DescriptionVisibility));
            }
        }

        private ObservableCollection<UIOrderedTrackModel> _playlistTracks;
        public ObservableCollection<UIOrderedTrackModel> PlaylistTracks
        {
            get { return _playlistTracks; }
            set
            {
                SetField(ref _playlistTracks, value);
            }
        }

        private bool trackIndexChanged = false;

        public ICommand OpenTrackPopupCommand { get; }
        public ICommand OpenPlaylistPopupCommand { get; }
        public ICommand PlayPlaylistCommand { get; }
        public ICommand PlayTrackCommand { get; }
        public ICommand PlayShuffledPlaylistCommand { get; }
        public ICommand EditPlaylistCommand { get; }
        public ICommand NavigateBackCommand { get; }
        public ICommand NavigateToArtistCommand { get; }
        public ICommand NavigateToAlbumByIdCommand { get; }
        public ICommand SaveRadioCommand { get; }
        public PlaylistViewModel(INavigationService navigationService, IQueueService queueService, IModalService modalService, IPlaylistService playlistService)
        {
            _navigationService = navigationService;
            QueueService = queueService;
            _modalService = modalService;
            _playlistService = playlistService;

            // play
            PlayPlaylistCommand = new RelayCommand(() => PlayPlaylist());
            PlayShuffledPlaylistCommand = new RelayCommand(() => PlayPlaylist(true));
            PlayTrackCommand = new RelayCommand<UIOrderedTrackModel>((track) => PlayPlaylist(false, track));

            // navigate
            NavigateBackCommand = new RelayCommand(_navigationService.NavigateBack);
            NavigateToArtistCommand = new RelayCommand<int>(async id => _navigationService.NavigateTo(ViewNameEnum.SpecificArtist, await DataAccess.Connection.GetArtist(id)));
            NavigateToAlbumByIdCommand = new RelayCommand<int>(async id => _navigationService.NavigateTo(ViewNameEnum.SpecificAlbum, await DataAccess.Connection.GetAlbum(id)));

            // open popup
            OpenTrackPopupCommand = new RelayCommand<UIOrderedTrackModel>((track) => _navigationService.OpenPopup(ViewNameEnum.TrackPopup, track));
            OpenPlaylistPopupCommand = new RelayCommand(() => _navigationService.OpenPopup(ViewNameEnum.PlaylistPopup, Playlist));

            // playlist related command
            EditPlaylistCommand = new RelayCommand(() => _modalService.OpenModal(ViewNameEnum.CreatePlaylist, OnModalClosed, Playlist));
            SaveRadioCommand = new RelayCommand(() => _playlistService.SaveRadio(Playlist, PlaylistTracks.ToList().ToOrderedTrackModel()));

            // load data
            Update();
        }

        public override void Dispose()
        {
            // the view is being disposed => the user navigated to another view
            // if the index of some tracks has been changed, update the playlist
            if (trackIndexChanged)
            {
                DataAccess.Connection.UpdatePlaylistTracks(Playlist.Id, PlaylistTracks.ToList().ToOrderedTrackModel());
            }

            GC.SuppressFinalize(this);
        }

        private void PlayPlaylist(bool shuffle = false, TrackModel track = null)
        {
            if (PlaylistTracks is null || PlaylistTracks.Count == 0) return;

            if (IsAutoPlaylist)
            {
                _queueService.SetNewQueue(PlaylistTracks.ToTrackModel(), new(Playlist.Name, ModelTypeEnum.Playlist, Playlist.Id), "", track, shuffle, false, false);
            }
            else
            {
                _queueService.SetNewQueue(PlaylistTracks.ToTrackModel(), new(Playlist.Name, ModelTypeEnum.Playlist, Playlist.Id), Playlist.Cover, track, shuffle, false, false);
            }
        }

        private async void OnModalClosed(bool canceled)
        {
            if (!canceled)
            {
                // get the updated playlist
                PlaylistModel playlist = await DataAccess.Connection.GetPlaylist(Playlist.Id);
                Update(playlist);
            }
        }

        public async override void Update(BaseModel parameter = null)
        {
            if(parameter is null)
            {
                Playlist = (PlaylistModel)_navigationService.CurrentViewParameter;
            }
            else
            {
                Playlist = (PlaylistModel)parameter;
            }

            if(Playlist is not null)
            {
                DescriptionVisibility = !string.IsNullOrWhiteSpace(Playlist.Description);

                IsAutoPlaylist = false;
                IsRadio = false;
                if (IsFavoritePlaylist || IsLastPlayedPlaylist || IsMostPlayedPlaylist)
                {
                    IsAutoPlaylist = true;
                    PathCover = Playlist.Cover;
                    PlaylistTracks = new((await Playlist.Tracks.GetAlbumTrackProperties()).ToUIOrderedTrackModel(QueueService.AlbumCoverOnly, QueueService.AutoCover));
                }
                else if (Playlist.PlaylistType == PlaylistTypeEnum.Radio)
                {
                    IsRadio = true;
                    Cover = Playlist.Cover;
                    PlaylistTracks = new((await Playlist.Tracks.GetAlbumTrackProperties()).ToUIOrderedTrackModel(QueueService.AlbumCoverOnly, QueueService.AutoCover));
                }
                else // user playlistService
                {
                    Cover = Playlist.Cover;
                    List<OrderedTrackModel> tracks = await DataAccess.Connection.GetTracksFromPlaylist(Playlist.Id);
                    PlaylistTracks = new((await tracks.GetAlbumTrackProperties()).ToUIOrderedTrackModel(QueueService.AlbumCoverOnly, QueueService.AutoCover));
                }

                PlaylistDuration = PlaylistTracks.GetTotalLength(out int _);
            }
        }

        public void MoveTrack(int originalIndex, int targetIndex)
        {
            if (targetIndex >= PlaylistTracks.Count) 
                targetIndex = PlaylistTracks.Count - 1;

            PlaylistTracks.Move(originalIndex, targetIndex);
            UpdateTrackIndex();
        }

        private void InsertTrack(TrackModel track, int index)
        {
            UIOrderedTrackModel playlistTrack = new(track, index + 1, QueueService.AlbumCoverOnly, QueueService.AutoCover);

            DataAccess.Connection.AddTrackToPlaylist(Playlist, track, index + 1);

            PlaylistTracks.Insert(index, playlistTrack);
            UpdateTrackIndex();
        }

        private void UpdateTrackIndex()
        {
            for (int i = 0; i < PlaylistTracks.Count; i++)
            {
                PlaylistTracks[i].TrackIndex = i + 1; // update all indexes
            }
            trackIndexChanged = true; // playlist will be updated when disposed
        }

        public void DragOver(IDropInfo dropInfo)
        {
            if (dropInfo.Data is TrackModel)
            {
                dropInfo.DropTargetAdorner = DropTargetAdorners.Insert;
                dropInfo.Effects = DragDropEffects.Move;
            }
        }

        public void Drop(IDropInfo dropInfo)
        {
            if(dropInfo.Data is UIOrderedTrackModel track)
            {
                if (!PlaylistTracks.Any(t => t.Id == track.Id)) // new track inserted
                {
                    UIOrderedTrackModel sourceItem = dropInfo.Data as UIOrderedTrackModel;

                    InsertTrack(sourceItem, dropInfo.InsertIndex);
                }
                else if (dropInfo.Data is UIOrderedTrackModel) // track moved
                {
                    int originalIndex = PlaylistTracks.IndexOf(track);

                    // when the original index is lower than the insert one, the track gets inserted one index to high
                    MoveTrack(originalIndex, dropInfo.InsertIndex > originalIndex ? dropInfo.InsertIndex - 1 : dropInfo.InsertIndex);
                }
            }
        }
    }
}
