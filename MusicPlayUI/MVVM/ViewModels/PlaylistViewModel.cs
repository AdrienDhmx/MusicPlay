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

        private List<Tag> _tags;
        public List<Tag> Tags
        {
            get => _tags;
            set
            {
                SetField(ref _tags, value);
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
            PlayPlaylistCommand = new RelayCommand(() => PlayPlaylist());
            PlayShuffledPlaylistCommand = new RelayCommand(() => PlayPlaylist(true));
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
            SaveRadioCommand = new RelayCommand(() => _playlistService.SaveRadio(Playlist, [.. Playlist.Tracks]));

            // load data
            Update();
        }

        public override void Dispose()
        {
            // the view is being disposed => the user navigated to another view
            // if the index of some tracks has been changed, update the playlist
            if (trackIndexChanged)
            {
                //DataAccess.Connection.UpdatePlaylistTracks(PlaylistModel.Id, Tracks.ToList().ToOrderedTrackModel());
            }

            GC.SuppressFinalize(this);
        }

        private void PlayPlaylist(bool shuffle = false, Track track = null)
        {
            if (Playlist.Tracks.IsNullOrEmpty()) return;

            _queueService.SetNewQueue(Playlist.Tracks, Playlist, Playlist.Name, Playlist.Cover, track, shuffle, false, false);
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

            if(Playlist is not null)
            {
                DescriptionVisibility = !string.IsNullOrWhiteSpace(Playlist.Description);

                IsAutoPlaylist = false;
                IsRadio = false;
                if (IsFavoritePlaylist || IsLastPlayedPlaylist || IsMostPlayedPlaylist)
                {
                    IsAutoPlaylist = true;
                    PathCover = Playlist.Cover;
                    Playlist.Tracks = new(); //new((await Playlist.Tracks.GetAlbumTrackProperties()).ToUIOrderedTrackModel(QueueService.AlbumCoverOnly, QueueService.AutoCover));
                }
                else if (Playlist.PlaylistType == PlaylistTypeEnum.Radio)
                {
                    IsRadio = true;
                    Cover = Playlist.Cover;
                    Playlist.Tracks = new(); // new((await Playlist.Tracks.GetAlbumTrackProperties()).ToUIOrderedTrackModel(QueueService.AlbumCoverOnly, QueueService.AutoCover));
                }
                else // user playlists
                {
                    Cover = Playlist.Cover;
                }               
            }
        }

        public void MoveTrack(int originalIndex, int targetIndex)
        {
            if (targetIndex >= Playlist.Tracks.Count) 
                targetIndex = Playlist.Tracks.Count - 1;

            Playlist.Tracks.Move(originalIndex, targetIndex);
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
            for (int i = 0; i < Playlist.Tracks.Count; i++)
            {
                Playlist.Tracks[i].TrackIndex = i + 1; // update all indexes
            }
            trackIndexChanged = true; // playlist will be updated when disposed
        }

        public void DragOver(IDropInfo dropInfo)
        {
            if (dropInfo.Data is Track)
            {
                dropInfo.DropTargetAdorner = DropTargetAdorners.Insert;
                dropInfo.Effects = DragDropEffects.Move;
            }
        }

        public async void Drop(IDropInfo dropInfo)
        {
            if(dropInfo.Data is Track track)
            {
                if (!Playlist.Tracks.Any(t => t.TrackId == track.Id)) // new track inserted
                {
                    Track sourceItem = dropInfo.Data as Track;

                    await InsertTrack(sourceItem, dropInfo.InsertIndex);
                }
                else if (dropInfo.Data is Track) // track moved
                {
                    int originalIndex = Playlist.Tracks.IndexOf(new() { Track = track });

                    // when the original index is lower than the insert one, the track gets inserted one index to high
                    MoveTrack(originalIndex, dropInfo.InsertIndex > originalIndex ? dropInfo.InsertIndex - 1 : dropInfo.InsertIndex);
                }
            }
        }
    }
}
