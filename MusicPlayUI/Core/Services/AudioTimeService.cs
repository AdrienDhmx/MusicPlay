using AudioHandler;
using DataBaseConnection.DataAccess;
using MusicFilesProcessor.Helpers;
using MusicPlayModels;
using MusicPlayModels.MusicModels;
using MusicPlayUI.Core.Helpers;
using MusicPlayUI.Core.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Threading;

namespace MusicPlayUI.Core.Services
{
    public class AudioTimeService : ObservableObject, IAudioTimeService, ISliderDragTarget
    {
        private readonly IQueueService _queueService;
        private readonly IAudioPlayback _audioPlayback;
        private readonly IHistoryServices _historyServices;
        private readonly DispatcherTimer audioTimer;

        private int TimerInterval { get; set; } = ConfigurationService.GetPreference(Enums.SettingsEnum.TimerInterval);

        private bool _isLooping;
        public bool IsLooping
        {
            get => _isLooping;
            set
            {
                SetField(ref _isLooping, value);
            }
        }

        private string _currentPosition = "00:00";
        public string CurrentPosition
        {
            get => _currentPosition;
            set
            {
                SetField(ref _currentPosition, value);
            }
        }

        private int _currentPositionMs = 0;
        public int CurrentPositionMs
        {
            get
            {
                return _currentPositionMs;
            }
            set
            {
                SetField(ref _currentPositionMs, value);
                CurrentPosition = TimeSpan.FromMilliseconds(value).ToFormattedString();
            }
        }

        private int _maxPositionMs = 100;
        public int MaxPositionMs
        {
            get => _maxPositionMs;
            private set
            {
                SetField(ref _maxPositionMs, value);
            }
        }

        private int _maxQueuePositionMs = 100;
        public int MaxQueuePositionMs
        {
            get => _maxQueuePositionMs;
            private set
            {
                SetField(ref _maxQueuePositionMs, value);
            }
        }

        private string _currentQueuePosition = "00:00";
        public string CurrentQueuePosition
        {
            get => _currentQueuePosition;
            set
            {
                SetField(ref _currentQueuePosition, value);
            }
        }

        private int _currentQueuePositionMs = 0;
        public int CurrentQueuePositionMs
        {
            get => _currentQueuePositionMs;
            private set
            {
                CurrentQueuePosition = TimeSpan.FromMilliseconds(value).ToFormattedString();
                SetField(ref _currentQueuePositionMs, value);
            }
        }

        private int _trackListenTime = 0;
        private int TrackListenTime
        {
            get => _trackListenTime;
            set
            {
                if (value >= 0)
                {
                    SetField(ref _trackListenTime, value);
                }
            }
        }

        private int TrackJumpTime { get; set; } = 0;

        private int TimeUntilPlayingTrack { get; set; }
        private bool playCountIncreased = false;
        private int Lastpos { get; set; } = -1;

        public event Action CurrentPositionChanged;

        public AudioTimeService(IQueueService queueService, IAudioPlayback audioPlayback, IHistoryServices historyServices)
        {
            _queueService = queueService;
            _audioPlayback = audioPlayback;
            _historyServices = historyServices;

            _queueService.PlayingTrackChanged += OnPlayingTrackChanged;
            _queueService.QueueChanged += OnQueueChanged;
            _audioPlayback.IsPlayingChanged += OnIsPlayingChanged;

            audioTimer = new();
            audioTimer.Tick += AudioTimer_Tick;
            audioTimer.Interval = TimeSpan.FromMilliseconds(TimerInterval);

            _audioPlayback.AutoChangeOutput(ConfigurationService.GetPreference(Enums.SettingsEnum.AutoChangeOutputdevice) == 1);
        }

        private void AudioTimer_Tick(object sender, EventArgs e)
        {
            CurrentQueuePositionMs = TimeUntilPlayingTrack + CurrentPositionMs;
            CurrentPositionMs = (int)_audioPlayback.Position.TotalMilliseconds;

            TrackListenTime = CurrentPositionMs - TrackJumpTime;
            if (!playCountIncreased && TrackListenTime > (int)(0.75 * MaxPositionMs))
            {
                UpdatePlayCounts();
            }

            if (CurrentPositionMs >= MaxPositionMs || // max pos reached
                (_audioPlayback.IsLooping && CurrentPositionMs >= MaxPositionMs - TimerInterval) || // when looping the max pos is sometime never reached because playback stop (and timer stop) in between the max pos and the max pos - timer interval
                (Lastpos == CurrentPositionMs && CurrentPositionMs >= 0)) // max pos never reached for some file (mp3, I don't know why) or when audio output changes, then if pos hasn't changed consider end of track
            {
                EndOfTrack();
            }

            Lastpos = CurrentPositionMs;
            CurrentPositionChanged?.Invoke(); // trigger event for listener
        }

        private async void UpdatePlayCounts()
        {
            playCountIncreased = true;

            _queueService.IncreasePlayCount(1);

            List<int> artistIds = new(); // keep updated artist as to not update them more than once
            foreach (ArtistDataRelation artistTrack in _queueService.PlayingTrack.Artists)
            {
                ArtistModel artist = await DataAccess.Connection.GetArtist(artistTrack.ArtistId);
                if (artist != null && !artistIds.Any(id => id == artist.Id))
                {
                    artistIds.Add(artist.Id);
                    int playCount = artist.PlayCount + 1;
                    DataAccess.Connection.UpdateArtistInteraction(playCount, artist.Id);
                }
            }

            AlbumModel album = await DataAccess.Connection.GetAlbum(_queueService.PlayingTrack.AlbumId);
            if (album != null)
            {
                int playCount = album.PlayCount + 1;
                DataAccess.Connection.UpdateAlbumInteraction(playCount, album.Id);

                foreach (ArtistDataRelation artistAlbum in album.Artists)
                {
                    ArtistModel artist = await DataAccess.Connection.GetArtist(artistAlbum.ArtistId);
                    if (artist != null && !artistIds.Any(id => id == artist.Id))
                    {
                        artistIds.Add(artist.Id);
                        playCount = artist.PlayCount + 1;
                        DataAccess.Connection.UpdateArtistInteraction(playCount, artist.Id);
                    }
                }
            }
        }

        private void EndOfTrack()
        {
            audioTimer.Stop(); // stop timer, it will be started when new playing track is set

            // upadte the listen time
            _historyServices.UpdateTodayHistory(TrackListenTime);

            if (_audioPlayback.IsLooping)
            {
                // looping, the timer has to be restart here because the track does not change 
                audioTimer.Start();
                Lastpos = -1;
                playCountIncreased = false;
            }
            else if (_queueService.NextTrack())
            {
                Lastpos = -1;
                playCountIncreased = false;
            }
        }
        /// <summary>
        /// Set the play back position to the position specified
        /// and update the current track jump time for the listen time
        /// </summary>
        /// <param name="position"> The position to go to in milliseconds </param>
        public void SetPlayBackPosition(int position)
        {
            // update track jump time
            if (position > CurrentPositionMs)
            {
                TrackJumpTime += position - TrackListenTime;
            }
            else
            {
                TrackJumpTime += TrackListenTime - position;
            }

            // set playback position
            _audioPlayback.Position = TimeSpan.FromMilliseconds(position);
        }


        private void SetTimeUntilPlayingTrack()
        {
            TimeUntilPlayingTrack = 0;
            int index = _queueService.GetPlayingTrackIndex();

            TimeUntilPlayingTrack = index.GetLengthUntilTrack(_queueService.QueueTracks);
        }

        private void OnQueueChanged()
        {
            MaxQueuePositionMs = _queueService.QueueLength;
            CurrentQueuePositionMs = 0;
            SetTimeUntilPlayingTrack();
        }
        private void OnPlayingTrackChanged()
        {
            if (_queueService.PlayingTrack is not null)
            {
                // update some properties
                MaxPositionMs = _queueService.PlayingTrack.Length;
                SetTimeUntilPlayingTrack();

                // start the playback
                _audioPlayback.LoadAnPlay(_queueService.PlayingTrack.Path);

                // the previous track was looping
                if (IsLooping)
                {
                    _audioPlayback.Loop(true);
                }
            }
        }

        public void PlayPause()
        {
            if(_audioPlayback.Stream != -1)
            {
                if (_audioPlayback.IsPlaying)
                {
                    _audioPlayback.Pause();
                }
                else
                {
                    _audioPlayback.Play();
                }
            }
            else
            {
                // simulate a change in track to load the file and update properties
                OnPlayingTrackChanged();
            }
        }

        public void Loop()
        {
            _audioPlayback.Loop();
            IsLooping = _audioPlayback.IsLooping;
        }

        private void OnIsPlayingChanged(bool isPlaying)
        {
            if (isPlaying)
            {
                audioTimer.Start();
            }
            else
            {
                audioTimer.Stop();
            }
        }

        public void OnDragStart(double value)
        {
            audioTimer.Stop();
        }

        public void OnDragEnd(double value)
        {
            audioTimer.Start();
            SetPlayBackPosition((int)value);
        }
    }
}
