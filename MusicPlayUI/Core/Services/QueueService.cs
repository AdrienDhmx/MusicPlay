using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MusicFilesProcessor.Helpers;
using GongSolutions.Wpf.DragDrop;
using System.Windows;
using MusicPlayUI.MVVM.Models;
using MusicPlayUI.Core.Enums;
using System.Collections.ObjectModel;
using MusicPlayUI.Core.Factories;
using MusicPlayUI.Core.Services.Interfaces;
using MusicPlayUI.Core.Helpers;
using MusicPlay.Language;
using MessageControl;
using MusicPlay.Database.Models;
using MusicPlay.Database.Enums;
using MusicPlay.Database.Helpers;
using MusicPlay.Database.Models.DataBaseModels;
using AudioHandler;
using MusicPlayUI.MVVM.ViewModels;

namespace MusicPlayUI.Core.Services
{
    public class QueueService : ObservableObject, IDropTarget, IQueueService
    {
        private readonly IAudioPlayback _audioPlayback;

        public QueueService(IAudioPlayback audioPlayback)
        {
            _audioPlayback = audioPlayback;
            Queue = Queue.Get() ?? new();
            if(Queue.PlayingTrack.IsNotNull())
            {
                // load without starting the playback
                _audioPlayback.Load(Queue.PlayingTrack.Path);
            }
        }

        private Queue _queue;
        public Queue Queue
        {
            get => _queue;
            private set => SetField(ref  _queue, value);
        }

        public event Action<int> PreviewPlayingTrackChanged;
        private void OnPreviewPlayingTrackChanged(int newPlayingTrachIndex)
        {
            PreviewPlayingTrackChanged?.Invoke(newPlayingTrachIndex);
        }

        public event Action PlayingTrackChanged;
        private void OnPlayingTrackChanged()
        {
            if(Queue.IsNotNull() && Queue.PlayingTrack.IsNotNull())
                _audioPlayback.LoadAnPlay(Queue.PlayingTrack.Path);
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

        public string QueueDuration => Queue.Duration;
        public int QueueLength => Queue.Length;

        public string QueueCover => Queue.Cover;

        public string PlayingFromName => Queue.PlayingFromName;
        public PlayableModel PlayingFrom => Queue.PlayingFrom;

        private List<Track> LastRemovedTrack { get; set; } = new();
        private List<int> LastRemovedTrackIndex { get; set; } = new();

        public async void SetNewQueue(IEnumerable<OrderedTrack> tracks, PlayableModel playingFrom, string playingFromName, string cover, Track playingTrack = null, bool isShuffled = false, bool isOnRepeat = false, bool orderTracks = false)
        {            
            ObservableCollection<QueueTrack> queueTracks = new(tracks.ToQueueTrack());
            QueueTrack playingQueueTrack = null;
            if (playingTrack is not null)
            {
                playingQueueTrack = queueTracks.ToList().Find(t => t.TrackId == playingTrack.Id);
            }

            if (isShuffled)
            {
                queueTracks = await Task.Run(() => queueTracks.Shuffle<QueueTrack>(playingQueueTrack));
            }
            else if (orderTracks) // for playlist the tracks can be in a specific order wanted by the user
            {
                queueTracks = await Task.Run(queueTracks.Order);
            }

            playingQueueTrack ??= queueTracks.FirstOrDefault();

            if (playingQueueTrack is null)
                return;

            Queue.Length = tracks.GetTracksTotalLength();
            Queue.IsShuffled = isShuffled;
            Queue.IsOnRepeat = isOnRepeat;
            Queue.Cover = cover;
            Queue.PlayingFrom = playingFrom;
            Queue.PlayingFromId = playingFrom.Id;
            Queue.PlayingFromName = playingFromName;

            Queue.PlayingQueueTrack = playingQueueTrack;
            Queue.PlayingTrack = playingQueueTrack.Track;
            Queue.Tracks = queueTracks;

            App.Current.Dispatcher.Invoke(() =>
            {
                OnQueueChanged();
                OnPlayingTrackChanged();
            });
        }

        public void SetNewQueue(IEnumerable<Track> tracks, PlayableModel playingFrom, string playingFromName, string cover, Track playingTrack = null, bool isShuffled = false, bool isOnRepeat = false, bool orderTracks = false)
        {
            SetNewQueue(tracks.ToList().ToOrderedTracks<OrderedTrack>(), playingFrom, playingFromName, cover, playingTrack, isShuffled, isOnRepeat);
        }

        public void DeleteQueue()
        {
            //DataAccess.Connection.DeleteOne(Queue);
        }

        public async Task Shuffle()
        {
            Queue.IsShuffled = !Queue.IsShuffled;
            if (Queue.IsShuffled)
            {
                Queue.Tracks = await Task.Run(() => Queue.Tracks.Shuffle(new QueueTrack() { Track = Queue.PlayingTrack }));
            }
            else
            {
                Queue.Tracks = Queue.Tracks.Order();
            }
            OnQueueChanged();
        }

        public void Repeat()
        {
            Queue.IsOnRepeat = !Queue.IsOnRepeat;
        }

        public void PlayTrack(Track track)
        {
            if (Queue.Tracks.Any(t => t.TrackId == track.Id))
            {
                SetPlayingTrackIndex(GetTrackIndex(track));
            }
        }

        /// <summary>
        /// Remove the track from the queue, also update the queue Length and duration
        /// </summary>
        /// <param name="track"></param>
        public async void RemoveTrack(Track track)
        {
            if (Queue.Tracks.Any(t => t.TrackId == track.Id))
            {
                // the track to remove is the playing track, but the playing track can't be changed
                if (track.Id == Queue.PlayingTrack.Id && !NextTrack() && !PreviousTrack())
                {
                    MessageHelper.PublishMessage(MessageFactory.ErrorMessage(ErrorEnum.RemoveTrackFromQueueError));
                    return;
                }

                int index = GetTrackIndex(track);

                // bug with the drag drop library when removing an item from the list while the 'drag' (click in reality) is processed
                await Task.Delay(200);

                LastRemovedTrack.Insert(0, Queue.Tracks[index].Track);
                LastRemovedTrackIndex.Insert(0, index); // keeping the index and not using the TrackIndex property of the track is needed in case the queue is shuffled
                Queue.Tracks.RemoveAt(index);

                Queue.Length -= track.Length;
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
            return ChangeTrack(1);
        }

        /// <summary>
        /// Get the index of the currently playing track in the queue and decrease it by one.
        /// If the index is inferior to 0 the index is the index corresponding to the last track in the queue (go back to the last track in the queue) if the queue is on repeat,
        /// else the index is not decreased.
        /// </summary>
        /// <returns> The new (not if the track is already the first one and the queue is not on repeat) playing track in the queue </returns>
        public bool PreviousTrack()
        {
            return ChangeTrack(-1);
        }

        private bool ChangeTrack(int offset)
        {
            if (Queue.Tracks.IsNullOrEmpty())
                return false;

            int currentIndex = GetPlayingTrackIndex();
            return SetPlayingTrackIndex(currentIndex + offset);
        }

        public async Task IncreasePlayCount(int increaseValue)
        {
            await Track.UpdatePlayCount(Queue.PlayingQueueTrack.Track);
        }

        public async Task UpdateRating(int rating)
        {
            await Track.UpdateRating(Queue.PlayingTrack, rating);
            OnPlayingTrackInteractionChanged();
        }

        public async Task UpdateFavorite()
        {
            await Track.UpdateIsFavorite(Queue.PlayingTrack);
            Queue.PlayingTrack.IsFavorite = !Queue.PlayingTrack.IsFavorite;
            OnPlayingTrackInteractionChanged();
        }

        /// <summary>
        /// return the index in the queue of the track if found, otherwise it returns -1
        /// </summary>
        /// <param name="track"></param>
        /// <returns></returns>
        public int GetTrackIndex(Track track)
        {
            return Queue.Tracks.IndexOf(new QueueTrack() { Track = track }); // BaseModel override the Equals(obj) to only compare the Id
        }

        /// <summary>
        /// return the currently playing track index in the queue
        /// </summary>
        /// <returns></returns>
        public int GetPlayingTrackIndex()
        {
            return GetTrackIndex(Queue.PlayingTrack);
        }

        private bool SetPlayingTrackIndex(int index)
        {
            if (index >= 0 && index < Queue.Tracks.Count)
            {
                OnPreviewPlayingTrackChanged(index);
                Queue.PlayingQueueTrack = Queue.Tracks[index];
                OnPlayingTrackChanged();
                return true;
            }
            if (Queue.IsOnRepeat)
            {
                index = index < 0 ? Queue.Tracks.Count - 1 : 0;
                return SetPlayingTrackIndex(index); // Call this method with the corrected index
            }
            return false;
        }

        /// <summary>
        /// Add a track to the queue, either right after the currently playing track or at the end (bool end = true)
        /// It also update the queue length and duration properties
        /// </summary>
        /// <param name="track"></param>
        /// <param name="end"></param>
        public void AddTrack(Track track, bool end = false, bool showMsg = true)
        {
            if (track is not null)
            {
                if (end)
                {
                    Queue.Tracks.Add(new QueueTrack(track, Queue.Tracks.Count));
                }
                else
                {
                    int index = GetPlayingTrackIndex() + 1;
                    Queue.Tracks.Insert(index, new(track, index + 1));
                }

                Queue.Length += track.Length;

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
        public void AddTracks(List<Track> tracks, bool end = false, bool album = true, string name = "")
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
                        Queue.Tracks.Insert(index, new(track, index + 1));

                        Queue.Length += track.Length;
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
        /// Publish a message to notify the user if <paramref name="msg"/> is true.
        /// </summary>
        /// <param name="index">The index to insert to</param>
        /// <param name="track">The track to insert</param>
        /// <param name="msg">Whether to publish a message to notify the user or not</param>
        private void InsertTrack(int index, Track track, bool msg = true)
        {
            if(track is not null)
            {
                if(Queue.Tracks.IsNotNullOrEmpty())
                {
                    Queue.Tracks =
                    [
                        new(track, 1)
                    ];
                }
                else
                {
                    Queue.Tracks.Insert(index, new(track, index));
                    UpdateTracksIndexes();
                }

                Queue.Length += track.Length;

                if (msg)
                    MessageHelper.PublishMessage(MessageFactory.QueueChanged(track.Title));
            }
        }

        public void MoveTrack(int originalIndex, int targetIndex)
        {
            if(targetIndex >= Queue.Tracks.Count) targetIndex = Queue.Tracks.Count - 1;

            Queue.Tracks.Move(originalIndex, targetIndex);
            UpdateTracksIndexes();
        }

        private void UpdateTracksIndexes()
        {
            // if the list is shuffled can't update the track indexes based on their order in the list
            if (!Queue.IsShuffled) 
            {
                for (int i = 0; i < Queue.Tracks.Count; i++)
                {
                    Queue.Tracks[i].TrackIndex = i + 1;
                }
            }
        }

        public void ClearQueue()
        {
            Queue.PlayingQueueTrack = null;
            Queue.PlayingTrack = null;
            Queue.Tracks.Clear();
            Queue = new();
            _audioPlayback.Stop();
            // notify all listeners
            OnPropertyChanged(nameof(PlayingFrom));
            OnPropertyChanged(nameof(PlayingFromName));
            OnQueueChanged();
            OnPlayingTrackChanged();
        }

        public void SaveQueue()
        {
            if (Queue.Tracks.IsNullOrEmpty() || Queue.PlayingFrom.IsNull()) return;

            Queue.Insert(Queue);
        }

        public async Task NavigateToPlayingFrom()
        {
            if (Queue.Tracks.IsNotNullOrEmpty() && PlayingFrom is not null)
            {
                Type viewModel;
                BaseModel parameter;
                int id = Queue.PlayingFromId;
                switch (Queue.PlayingFromModelType)
                {
                    case ModelTypeEnum.Album:
                        viewModel = typeof(AlbumViewModel);
                        parameter = await Album.Get(id);
                        break;
                    case ModelTypeEnum.Artist:
                        viewModel = typeof(ArtistViewModel);
                        parameter = await Artist.Get(id);
                        break;
                    case ModelTypeEnum.Playlist:
                        viewModel = typeof(PlaylistViewModel);
                        if (Queue.PlayingFromName.EndsWith("Radio"))
                        {
                            parameter = PlaylistsFactory.CreateRadioPlaylist(Queue.PlayingFromName, "", QueueCover, Queue.Tracks.ToList().ToTrack());
                        }
                        else if (Queue.PlayingFromName == Resources.Most_Played)
                        {
                            parameter = PlaylistsFactory.CreateMostPlayedPlaylist();
                        }
                        else if (Queue.PlayingFromName == Resources.Favorite)
                        {
                            parameter = PlaylistsFactory.CreateFavoritePlaylist();
                        }
                        else if (Queue.PlayingFromName == Resources.Last_Played)
                        {
                            parameter = PlaylistsFactory.CreateLastPlayedPlaylist();
                        }
                        else // user playlist
                        {
                            parameter = Playlist.Get(id);
                        }
                        break;
                    case ModelTypeEnum.Tag:
                        viewModel = typeof(GenreViewModel);
                        parameter = await Tag.Get(id);
                        break;
                    default:
                        viewModel = typeof(EmptyViewModel);
                        parameter = null;
                        break;
                }

                if (viewModel != typeof(EmptyViewModel) && parameter is not null)
                {
                    App.State.NavigateTo(viewModel, parameter);
                    return;
                }
            }
            MessageHelper.PublishMessage(MessageFactory.ErrorMessage(ErrorEnum.PlayingFromNotFound));
        }

        public void DragOver(IDropInfo dropInfo)
        {
            if (dropInfo.Data is Track || dropInfo.Data is QueueTrack)
            {
                dropInfo.DropTargetAdorner = DropTargetAdorners.Insert;
                dropInfo.Effects = DragDropEffects.Move;
            }
        }

        public void Drop(IDropInfo dropInfo)
        {
            if(dropInfo.Data is QueueTrack sourceItem && Queue.Tracks.Contains(sourceItem)) // track has been moved
            {
                int originalIndex = Queue.Tracks.IndexOf(sourceItem);

                MoveTrack(originalIndex, dropInfo.InsertIndex);
            }
            else if(dropInfo.Data is Track) // new inserted track
            {
                Track track = dropInfo.Data as Track;
                InsertTrack(dropInfo.InsertIndex, track);
            }
            // TODO: add album, artist and playlist support
        }
    }
}
