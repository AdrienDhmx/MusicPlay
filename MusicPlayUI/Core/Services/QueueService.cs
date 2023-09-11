using System;
using MusicPlayModels.MusicModels;
using DataBaseConnection.DataAccess;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MusicFilesProcessor.Helpers;
using GongSolutions.Wpf.DragDrop;
using System.Windows;
using MusicPlayModels;
using MusicPlayUI.MVVM.Models;
using MusicPlayUI.Core.Enums;
using System.Collections.ObjectModel;
using MusicPlayUI.Core.Factories;
using System.Runtime.CompilerServices;
using MusicPlayUI.Core.Services.Interfaces;
using MusicPlayUI.Core.Helpers;
using MusicPlay.Language;
using System.Xml.Linq;
using Windows.UI.Composition;
using MessageControl;

namespace MusicPlayUI.Core.Services
{
    public class QueueService : ObservableObject, IDropTarget, IQueueService
    {
        private readonly INavigationService _navigationService;

        private bool _isInit = false;
        public QueueService(INavigationService navigationService)
        {
            _navigationService = navigationService;
            ConfigurationService.QueueCoversChange += InitCoversSettings;

            InitCoversSettings();
            GetPlayingQueue();
        }

        /// <summary>
        /// If queue stored in database retrieve it, otherwise a new empty queue is instantiated
        /// </summary>
        private async void GetPlayingQueue()
        {
            QueueModel queue = await DataAccess.Connection.GetQueue();
            if (queue is not null)
            {
                queue.Tracks = await queue.Tracks.GetAlbumTrackProperties();

                SetNewQueue(queue.Tracks.ToUIOrderedTrackModel(AlbumCoverOnly, AutoCover), new(queue.PlayingFrom, queue.ModelType, queue.PlayingFromId), queue.Cover, queue.PlayingTrack, queue.IsShuffled, queue.IsOnRepeat, true);
            }
        }

        private ObservableCollection<UIOrderedTrackModel> _queueTracks = new();
        public ObservableCollection<UIOrderedTrackModel> QueueTracks
        {
            get { return _queueTracks; }
            set { SetField(ref _queueTracks, value); }
        }

        private UIOrderedTrackModel _playingTrack;
        public UIOrderedTrackModel PlayingTrack
        {
            get { return _playingTrack; }
            set 
            {
                SetField(ref _playingTrack, value);
                if(_isInit)
                    OnPlayingTrackChanged();
                else
                    _isInit = true;
            }
        }

        public event Action PlayingTrackChanged;
        private void OnPlayingTrackChanged()
        {
            PlayingTrackChanged?.Invoke();
        }

        public event Action PlayingTrackInteractionChanged;
        private void OnPlayingTrackInteractionChanged()
        {
            PlayingTrackInteractionChanged?.Invoke();
        }

        public event Action QueueChanged;
        private void OnQueueChanged()
        {
            QueueChanged?.Invoke();
        }

        private string _queueDuration;
        public string QueueDuration
        {
            get { return _queueDuration; }
            set { SetField(ref _queueDuration, value); }
        }

        private int _queueLength;
        public int QueueLength
        {
            get { return _queueLength; }
            set { SetField(ref _queueLength, value); }
        }

        private string _queueCover;
        public string QueueCover
        {
            get { return _queueCover; }
            set { SetField(ref _queueCover, value); }
        }

        private QueuePlayingFromModel _playingFrom;
        public QueuePlayingFromModel PlayingFrom
        {
            get { return _playingFrom; }
            set { SetField(ref _playingFrom, value); }
        }

        private bool _isShuffled;
        public bool IsShuffled
        {
            get { return _isShuffled; }
            private set
            {
                SetField(ref _isShuffled, value);
            }
        }

        private bool _isOnRepeat;
        public bool IsOnRepeat
        {
            get
            {
                return _isOnRepeat;
            }
            private set
            {
                SetField(ref _isOnRepeat, value);
            }
        }

        private bool _areCoversEnabled = true;
        public bool AreCoversEnabled
        {
            get { return _areCoversEnabled; }
            set
            {
                SetField(ref _areCoversEnabled, value);
            }
        }

        private bool _albumCoverOnly = false;
        public bool AlbumCoverOnly
        {
            get => _albumCoverOnly;
            set
            {
                SetField(ref _albumCoverOnly, value);
            }
        }

        private bool _artworkOnly = false;
        public bool ArtworkOnly
        {
            get => _artworkOnly;
            set
            {
                SetField(ref _artworkOnly, value);
            }
        }

        private bool _autoCover = false;
        public bool AutoCover
        {
            get => _autoCover;
            set
            {
                SetField(ref _autoCover, value);
            }
        }

        private List<UIOrderedTrackModel> LastRemovedTrack { get; set; } = new();
        private List<int> LastRemovedTrackIndex { get; set; } = new();

        private void InitCoversSettings()
        {
            AreCoversEnabled = true;
            AlbumCoverOnly = false;
            ArtworkOnly = false;
            AutoCover = false;
            SettingsValueEnum settingsValue = (SettingsValueEnum)ConfigurationService.GetPreference(SettingsEnum.QueueCovers);
            switch (settingsValue)
            {
                case SettingsValueEnum.NoCovers:
                    AreCoversEnabled = false;
                    break;
                case SettingsValueEnum.AlbumCovers:
                    AlbumCoverOnly = true;
                    break;
                case SettingsValueEnum.ArtworkCovers:
                    ArtworkOnly = true;
                    break;
                case SettingsValueEnum.AutoCovers:
                    AutoCover = true;
                    break;
                default:
                    AutoCover = true;
                    break;
            }

            if(QueueTracks is not null)
                QueueTracks = new(QueueTracks.ToList().ToUIOrderedTrackModel(AlbumCoverOnly, AutoCover));

            if(_navigationService.CurrentViewName == ViewNameEnum.SpecificAlbum ||
                _navigationService.CurrentViewName == ViewNameEnum.SpecificArtist ||
                _navigationService.CurrentViewName == ViewNameEnum.SpecificPlaylist)
            {
                _navigationService.CurrentViewModel.Update();
            }
        }

        public async void SetNewQueue(List<UIOrderedTrackModel> tracks, QueuePlayingFromModel playingFrom, string cover, TrackModel playingTrack = null, bool isShuffled = false, bool isOnRepeat = false, bool orderTracks = false)
        {
            QueueDuration = tracks.GetTotalLength(out int length);
            QueueLength = length;

            IsShuffled = isShuffled;
            IsOnRepeat = isOnRepeat;

            PlayingFrom = playingFrom;

            QueueCover = cover;

            ObservableCollection<UIOrderedTrackModel> queueTracks = new(tracks);
            UIOrderedTrackModel playingQueueTrack = null;
            if (playingTrack is not null)
            {
                playingQueueTrack = new(tracks.Find(t => t.Id == playingTrack.Id), AlbumCoverOnly, AutoCover);
            }

            if (IsShuffled)
            {
                QueueTracks = await Task.Run(() => queueTracks.Shuffle(PlayingTrack));
            }
            else  if (orderTracks) // for playlist the tracks can be in a specific order wanted by the user
            {
                QueueTracks = await Task.Run(queueTracks.OrderTracks);
            }
            else
            {
                QueueTracks = queueTracks;
            }

            playingQueueTrack ??= QueueTracks.FirstOrDefault();
            PlayingTrack = playingQueueTrack;
            OnQueueChanged();
        }

        public async void SetNewQueue(List<TrackModel> tracks, QueuePlayingFromModel playingFrom, string cover, TrackModel playingTrack = null,
           bool isShuffled = false, bool isOnRepeat = false, bool orderTracks = false)
        {
            tracks = await tracks.GetAlbumTrackProperties();
            SetNewQueue(tracks.ToUIOrderedTrackModel(AlbumCoverOnly, AutoCover), playingFrom, cover, playingTrack, isShuffled, isOnRepeat, orderTracks);
        }

        public void DeleteQueue()
        {
            QueueTracks.Clear();
            PlayingTrack = null;
            PlayingFrom = null;
            DataAccess.Connection.DeleQueue();
        }

        public void Shuffle()
        {
            if (!IsShuffled) // prop set before this method is called
            {
                QueueTracks = QueueTracks.Shuffle(PlayingTrack);
            }
            else
            {
                 QueueTracks = QueueTracks.OrderTracks();
            }
            IsShuffled = !IsShuffled;
        }

        public void Repeat()
        {
            IsOnRepeat = !IsOnRepeat;
        }

        public void PlayTrack(TrackModel track)
        {
            if (QueueTracks.Any(t => t.Id == track.Id))
            {
                SetPlayingTrackIndex(GetTrackIndex(track));
            }
        }

        /// <summary>
        /// Remove the track from the queue, also update the queue Length and duration
        /// </summary>
        /// <param name="track"></param>
        public async void RemoveTrack(TrackModel track)
        {
            if (QueueTracks.Any(t => t.Id == track.Id))
            {
                // the track to remove is the playing track, but the playing track can't be changed
                if (track.Id == PlayingTrack.Id && !NextTrack() && !PreviousTrack())
                {
                    MessageHelper.PublishMessage(MessageFactory.ErrorMessage(ErrorEnum.RemoveTrackFromQueueError));
                    return;
                }

                int index = GetTrackIndex(track);

                // bug with the drag drop library when removing an item from the list while the 'drag' (click in reality) is processed
                await Task.Delay(200);

                LastRemovedTrack.Insert(0, QueueTracks[index]);
                LastRemovedTrackIndex.Insert(0, index); // keeping the index and not using the TrackIndex property of the track is needed in case the queue is shuffled
                QueueTracks.RemoveAt(index);

                QueueLength -= track.Length;
                QueueDuration = TagHelper.ToFormattedString(TimeSpan.FromMilliseconds(QueueLength));
                MessageHelper.PublishMessage(track.Title.TrackRemovedFromQueueWithUndo(RestoreLastRemovedTrack));
            }
        }

        private bool RestoreLastRemovedTrack()
        {
            if(LastRemovedTrack == null || LastRemovedTrack.Count == 0)
                return false;

            InsertTrack(LastRemovedTrackIndex.First(), LastRemovedTrack.First(), false);

            LastRemovedTrack.RemoveAt(0);
            LastRemovedTrackIndex.RemoveAt(0);
            return true;
        }

        /// <summary>
        /// Get the index of the currently playing track in the queue and increment it by one.
        /// If the index exceed the number of track in the queue the index is either 0 (go  back to the first track in the queue) if the queue is on repeat,
        /// else the index is not incremented.
        /// </summary>
        /// <returns> The new (not if the track is already the last one and the queue is not on repeat) playing track in the queue </returns>
        public bool NextTrack()
        {
            if (QueueTracks is null || QueueTracks.Count == 0)
                return false;

            int currentIndex = GetPlayingTrackIndex();
            return SetPlayingTrackIndex(currentIndex + 1);
        }

        /// <summary>
        /// Get the index of the currently playing track in the queue and decrease it by one.
        /// If the index is inferior to 0 the index is the index corresponding to the last track in the queue (go back to the last track in the queue) if the queue is on repeat,
        /// else the index is not decreased.
        /// </summary>
        /// <returns> The new (not if the track is already the first one and the queue is not on repeat) playing track in the queue </returns>
        public bool PreviousTrack()
        {
            if (QueueTracks is null || QueueTracks.Count == 0)
                return false;

            int currentIndex = GetPlayingTrackIndex();
            return SetPlayingTrackIndex(currentIndex - 1);
        }

        public void IncreasePlayCount(int increaseValue)
        {
            PlayingTrack.PlayCount += increaseValue;
            PlayingTrack.LastPlayed = DateTime.Now;
            DataAccess.Connection.UpdateTrackPlayCount(PlayingTrack);
        }

        public void UpdateRating(int rating)
        {
            PlayingTrack.Rating = rating;
            DataAccess.Connection.UpdateTrackRating(PlayingTrack);
            OnPlayingTrackInteractionChanged();
        }

        public async Task UpdateFavorite(bool isFavorite)
        {
            PlayingTrack.IsFavorite = isFavorite;
            OnPlayingTrackInteractionChanged();
            await DataAccess.Connection.UpdateTrackIsFavorite(PlayingTrack);
        }

        /// <summary>
        /// return the index in the queue of the track if found, otherwise it returns -1
        /// </summary>
        /// <param name="track"></param>
        /// <returns></returns>
        public int GetTrackIndex(TrackModel track)
        {
            int index = 0;
            // select id in case the track is in the queue but has been updated and has a prop dif from the one in the queue
            index = QueueTracks.Select(t => t.Id).ToList().IndexOf(track.Id);
            return index;
        }

        /// <summary>
        /// return the currently playing track index in the queue
        /// </summary>
        /// <returns></returns>
        public int GetPlayingTrackIndex()
        {
            return QueueTracks.Select(t => t.Id).ToList().IndexOf(PlayingTrack.Id);
        }

        private bool SetPlayingTrackIndex(int index)
        {
            if (index >= 0 && index < QueueTracks.Count)
            {
                PlayingTrack = QueueTracks[index];
                return true;
            }
            else
            {
                if (IsOnRepeat)
                {
                    if (index < 0) // Go to the last track in the queue
                    {
                        index = QueueTracks.Count - 1;
                    }
                    else // Go to the first track in queue
                    {
                        index = 0;
                    }
                    return SetPlayingTrackIndex(index); // Call this method with the corrected index
                }
                else
                {
                    return false;
                }
            }
        }

        /// <summary>
        /// Add a track to the queue, either right after the currently playing track or at the end (bool end = true)
        /// It also update the queue length and duration properties
        /// </summary>
        /// <param name="track"></param>
        /// <param name="end"></param>
        public void AddTrack(TrackModel track, bool end = false, bool showMsg = true)
        {
            if (track is not null)
            {
                if (end)
                {
                    QueueTracks.Add(new(track, QueueTracks.Count, AlbumCoverOnly, AutoCover));
                }
                else
                {
                    int index = GetPlayingTrackIndex() + 1;
                    QueueTracks.Insert(index, new(track, index + 1, AlbumCoverOnly, AutoCover));
                }

                QueueLength += track.Length;
                QueueDuration = TagHelper.ToFormattedString(TimeSpan.FromMilliseconds(QueueLength));

                if (showMsg)
                    MessageHelper.PublishMessage(MessageFactory.QueueChanged(track.Title));
            }
        }

        /// <summary>
        /// Add all the tracks in received order to the queue, either right after the playing track or at the end (bool end = true)
        /// It also update the queue length and duration properties.
        /// </summary>
        /// <param name="tracks"></param>
        /// <param name="end"></param>
        public void AddTracks(List<TrackModel> tracks, bool end = false, bool album = true, string name = "")
        {
            if (tracks is not null)
            {
                if (end)
                {
                    foreach (var track in tracks)
                    {
                        AddTrack(track, true, false);
                    }
                }
                else
                {
                    int index = GetPlayingTrackIndex() + 1;
                    foreach (var track in tracks)
                    {
                        QueueTracks.Insert(index, new(track, index + 1, AlbumCoverOnly, AutoCover));

                        QueueLength += track.Length;
                        QueueDuration = TagHelper.ToFormattedString(TimeSpan.FromMilliseconds(QueueLength));
                        index++;
                    }
                    UpdateTracksIndexes();
                }

                if (!string.IsNullOrWhiteSpace(name))
                {
                    MessageHelper.PublishMessage(name.QueueChanged(true, album, !album));
                }
            }
        }

        /// <summary>
        /// Insert a track in the queue and update all the properties of the queue (duration and length)
        /// Publich a message to notify the user if <paramref name="msg"/> is true.
        /// </summary>
        /// <param name="index">The index to insert to</param>
        /// <param name="track">The track to insert</param>
        /// <param name="msg">Wether to publish a message to notify the user or not</param>
        private void InsertTrack(int index, TrackModel track, bool msg = true)
        {
            if(track is not null)
            {
                if(QueueTracks is null || QueueTracks.Count == 0)
                {
                    QueueTracks = new()
                    {
                        new(track, 1, AlbumCoverOnly, AutoCover)
                    };
                }
                else
                {
                    QueueTracks.Insert(index, new(track, AlbumCoverOnly, AutoCover));
                    UpdateTracksIndexes();
                }

                QueueLength += track.Length;
                QueueDuration = TagHelper.ToFormattedString(TimeSpan.FromMilliseconds(QueueLength));

                if(msg)
                    MessageHelper.PublishMessage(MessageFactory.QueueChanged(track.Title));
            }
        }

        public void MoveTrack(int originalIndex, int targetIndex)
        {
            if(targetIndex >= QueueTracks.Count) targetIndex = QueueTracks.Count - 1;

            QueueTracks.Move(originalIndex, targetIndex);
            UpdateTracksIndexes();
        }

        private void UpdateTracksIndexes()
        {
            if(!IsShuffled)
            {
                for (int i = 0; i < QueueTracks.Count; i++)
                {
                    QueueTracks[i].TrackIndex = i + 1;
                }
            }
        }

        public async Task SaveQueue()
        {
            if (QueueTracks is null || PlayingFrom is null) return;

            QueueModel queue = new()
            {
                Cover = QueueCover,
                Duration = QueueDuration,
                IsOnRepeat = IsOnRepeat,
                IsShuffled = IsShuffled,
                Length = QueueLength,
                PlayingFrom = PlayingFrom.PlayingFrom,
                ModelType = PlayingFrom.ModelType,
                PlayingTrack = PlayingTrack,
                PlayingTrackId = PlayingTrack.Id,
                Tracks = QueueTracks.ToList().ToOrderedTrackModel(),
            };

            await DataAccess.Connection.InsertQueue(queue);
        }

        public async void NavigateToPlayingFrom()
        {
            if (QueueTracks is not null && PlayingFrom is not null && PlayingFrom.DataId != -1)
            {
                ViewNameEnum viewName;
                BaseModel parameter;
                int id = PlayingFrom.DataId;
                switch (PlayingFrom.ModelType)
                {
                    case ModelTypeEnum.Album:
                        viewName = ViewNameEnum.SpecificAlbum;
                        parameter = await DataAccess.Connection.GetAlbum(id);
                        break;
                    case ModelTypeEnum.Artist:
                        viewName = ViewNameEnum.SpecificArtist;
                        parameter = await DataAccess.Connection.GetArtist(id);
                        break;
                    case ModelTypeEnum.Playlist:
                        viewName = ViewNameEnum.SpecificPlaylist;
                        if (PlayingFrom.PlayingFrom.Contains("Radio"))
                        {
                            parameter = PlaylistsFactory.CreateRadioPlaylist(PlayingFrom.PlayingFrom, "", QueueCover, QueueTracks.ToList().ToTrackModel());
                        }
                        else if (PlayingFrom.PlayingFrom == Resources.Most_Played)
                        {
                            parameter = await PlaylistsFactory.CreateMostPlayedPlaylist();
                        }
                        else if (PlayingFrom.PlayingFrom == Resources.Favorite)
                        {
                            parameter = await PlaylistsFactory.CreateFavoritePlaylist();
                        }
                        else if (PlayingFrom.PlayingFrom == Resources.Last_Played)
                        {
                            parameter = await PlaylistsFactory.CreateLastPlayedPlaylist();
                        }
                        else // user playlist
                        {
                            parameter = await DataAccess.Connection.GetPlaylist(id);
                        }
                        break;
                    case ModelTypeEnum.Tag:
                        viewName = ViewNameEnum.SpecificGenre;
                        parameter = await DataAccess.Connection.GetTag(id);
                        break;
                    default:
                        viewName = ViewNameEnum.Empty;
                        parameter = null;
                        break;
                }

                if (viewName is not ViewNameEnum.Empty && parameter is not null)
                {
                    _navigationService.NavigateTo(viewName, parameter);
                    return;
                }
            }
            MessageHelper.PublishMessage(MessageFactory.ErrorMessage(ErrorEnum.PlayingFromNotFound));
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
            if(dropInfo.Data is UIOrderedTrackModel sourceItem && QueueTracks.Contains(sourceItem)) // track has been moved
            {
                int originalIndex = QueueTracks.IndexOf(sourceItem);

                MoveTrack(originalIndex, dropInfo.InsertIndex);
            }
            else if(dropInfo.Data is TrackModel) // new inserted track
            {
                TrackModel track = dropInfo.Data as TrackModel;
                InsertTrack(dropInfo.InsertIndex, track);
            }
            // TODO: add album, artist and playlist support
        }
    }
}
