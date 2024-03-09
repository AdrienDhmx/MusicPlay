using MusicFilesProcessor.Helpers;
using MusicPlayUI.Core.Commands;
using System.Collections.Generic;
using System.Windows.Input;
using MusicPlayUI.Core.Enums;
using MusicPlayUI.Core.Services.Interfaces;
using MusicPlayUI.Core.Helpers;
using System.Collections.ObjectModel;
using System.Linq;
using GongSolutions.Wpf.DragDrop;
using System.Windows;
using MusicPlayUI.MVVM.Models;
using System;
using MusicPlay.Database.Models;
using MusicPlay.Database.Enums;
using System.Threading.Tasks;
using MusicPlay.Database.Helpers;
using MusicPlayUI.Core.Services;
using DynamicScrollViewer;
using MusicPlay.Database.Models.DataBaseModels;
using MusicPlay.Database.DatabaseAccess;


namespace MusicPlayUI.MVVM.ViewModels
{
    public class PlaylistViewModel : ViewModel, IDropTarget
    {
        private readonly IModalService _modalService;
        private readonly IPlaylistService _playlistService;
        private readonly ICommandsManager _commandsManager;
        private IQueueService _queueService;
        public IQueueService QueueService
        {
            get => _queueService;
            set
            {
                SetField(ref _queueService, value);
            }
        }

        public bool AreCoversEnabled
        {
            get => ConfigurationService.AreCoversEnabled;
        }

        private Playlist _playlist;
        public Playlist Playlist
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

        private bool trackIndexChanged = false;

        public ICommand OpenTrackPopupCommand { get; }
        public ICommand OpenPlaylistPopupCommand { get; }
        public ICommand PlayPlaylistCommand { get; }
        public ICommand PlayTrackCommand { get; }
        public ICommand PlayShuffledPlaylistCommand { get; }
        public ICommand EditPlaylistCommand { get; }
        public ICommand NavigateToArtistCommand { get; }
        public ICommand NavigateToAlbumByIdCommand { get; }
        public ICommand NavigateToTagCommand { get; }
        public ICommand SaveRadioCommand { get; }
        public PlaylistViewModel(IQueueService queueService, IModalService modalService, IPlaylistService playlistService, ICommandsManager commandsManager)
        {
            QueueService = queueService;
            _modalService = modalService;
            _playlistService = playlistService;
            _commandsManager = commandsManager;

            // play
            PlayPlaylistCommand = _commandsManager.PlayNewQueueCommand;
            PlayShuffledPlaylistCommand = _commandsManager.PlayNewQueueShuffledCommand;
            PlayTrackCommand = new RelayCommand<Track>((track) => PlayPlaylist(false, track));

            // navigate
            NavigateToArtistCommand = _commandsManager.NavigateToArtistByIdCommand;
            NavigateToAlbumByIdCommand = _commandsManager.NavigateToAlbumByIdCommand;
            NavigateToTagCommand = _commandsManager.NavigateToGenreCommand;

            // open popup
            OpenTrackPopupCommand = _commandsManager.OpenTrackPopupCommand;
            OpenPlaylistPopupCommand = _commandsManager.OpenPlaylistPopupCommand;

            // playlist related command
            EditPlaylistCommand = new RelayCommand(() => _modalService.OpenModal(ViewNameEnum.CreatePlaylist, (canceled) => { }, Playlist));
            SaveRadioCommand = new RelayCommand(() => _playlistService.SaveRadio(Playlist, [.. Playlist.PlaylistTracks]));
        }

        public override void Dispose()
        {
            // the view is being disposed => the user navigated to another view
            // if the index of some tracks has been changed, update the playlist
            if (trackIndexChanged)
            {
                using DatabaseContext context = new();
                Playlist.UpdateTrackIndexes(Playlist.PlaylistTracks, Playlist.Id, context);
            }

            GC.SuppressFinalize(this);
        }

        private void PlayPlaylist(bool shuffle = false, Track track = null)
        {
            if (Playlist.PlaylistTracks.IsNullOrEmpty()) return;

            _queueService.SetNewQueue(Playlist.PlaylistTracks, Playlist, Playlist.Name, Playlist.Cover, track, shuffle, false, false);
        }

        public override void OnScrollEvent(OnScrollEvent e)
        {
            AppBar.AnimateElevation(e.VerticalOffset, true);
            base.OnScrollEvent(e);
        }

        public override void UpdateAppBarStyle()
        {
            AppBar.SetStyle(AppTheme.Palette.PrimaryContainer, 0, 0);
        }

        public override void Init()
        {
            base.Init();
            Update();
        }

        public override void Update(BaseModel parameter = null)
        {
            if(parameter is null)
            {
                Playlist = (Playlist)App.State.CurrentView.State.Parameter;
            }
            else
            {
                Playlist = (Playlist)parameter;
            }

            if(Playlist is null)
            {
                App.State.NavigateBack();
            }

            AppBar.SetData(Playlist.Name, string.Empty);
            DescriptionVisibility = !string.IsNullOrWhiteSpace(Playlist.Description);

            IsAutoPlaylist = false;
            IsRadio = false;
            if (IsFavoritePlaylist || IsLastPlayedPlaylist || IsMostPlayedPlaylist)
            {
                IsAutoPlaylist = true;
                PathCover = Playlist.Cover;
            }
            else if (Playlist.PlaylistType == PlaylistTypeEnum.Radio)
            {
                IsRadio = true;
                Cover = Playlist.Cover;
            }
            else // user playlists
            {
                Cover = Playlist.Cover;
            }
        }

        public void MoveTrack(int originalIndex, int targetIndex)
        {
            if (targetIndex >= Playlist.PlaylistTracks.Count)
                targetIndex = Playlist.PlaylistTracks.Count - 1;
            else if (originalIndex < 0)
                originalIndex = 0;

            Playlist.PlaylistTracks.Move(originalIndex, targetIndex);
            UpdateTrackIndex();
        }

        private async Task InsertTrack(Track track, int index)
        {
            await Playlist.InsertTrack(Playlist, track, index + 1);
            //Tracks.Insert(index, playlistTrack);
            //UpdateTrackIndex();
        }

        private void UpdateTrackIndex()
        {
            for (int i = 0; i < Playlist.PlaylistTracks.Count; i++)
            {
                Playlist.PlaylistTracks[i].TrackIndex = i + 1; // update all indexes
            }
            trackIndexChanged = true; // playlist will be updated when disposed
        }

        public void DragOver(IDropInfo dropInfo)
        {
            if (dropInfo.Data is Track || dropInfo.Data is OrderedTrack || dropInfo.Data is PlaylistTrack)
            {
                dropInfo.DropTargetAdorner = DropTargetAdorners.Insert;
                dropInfo.Effects = DragDropEffects.Move;
            }
        }

        private async void DropTrack(IDropInfo dropInfo, Track track)
        {
            PlaylistTrack FoundPlaylistTrack = Playlist.PlaylistTracks.Where(pt => pt.TrackId == track.Id).FirstOrDefault();
            if (FoundPlaylistTrack.IsNull()) // new track inserted
            {
                await InsertTrack(track, dropInfo.InsertIndex);
            }
            else // track moved
            {
                int originalIndex = FoundPlaylistTrack.TrackIndex - 1;

                // when the original index is lower than the insert one, the track gets inserted one index to high
                MoveTrack(originalIndex, dropInfo.InsertIndex > originalIndex ? dropInfo.InsertIndex - 1 : dropInfo.InsertIndex);
            }
        }

        public void Drop(IDropInfo dropInfo)
        {
            if(dropInfo.Data is Track track)
            {
                DropTrack(dropInfo, track);
            }
            else if(dropInfo.Data is PlaylistTrack playlistTrack)
            {
                DropTrack(dropInfo, playlistTrack.Track);
            }
            else if (dropInfo.Data is OrderedTrack orderedTrack)
            {
                DropTrack(dropInfo, orderedTrack.Track);
            }
        }
    }
}
