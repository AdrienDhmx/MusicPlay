using AudioHandler;

using MusicFilesProcessor.Helpers;

using MusicPlay.Database.Models;

using MusicPlayUI.Core.Helpers;
using MusicPlayUI.Core.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Threading;
using MusicPlay.Database.Helpers;
using System.Threading.Tasks;

namespace MusicPlayUI.Core.Services
{
    public class AudioTimeService : ObservableObject, IAudioTimeService, ISliderDragTarget
    {
        private readonly IQueueService _queueService;
        private readonly AudioEventManager _audioEventManager;
        private readonly IAudioPlayback _audioPlayback;
        private readonly IHistoryServices _historyServices;

        private int TimerInterval { get; set; } = ConfigurationService.GetPreference(Enums.SettingsEnum.TimerInterval);

        private Track _currentTrack = null;

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
                CurrentPosition = TimeSpan.FromMilliseconds(value).ToShortString();
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
                CurrentQueuePosition = TimeSpan.FromMilliseconds(value).ToShortString();
                SetField(ref _currentQueuePositionMs, value);
            }
        }

        private int TimeUntilPlayingTrack { get; set; }

        private bool sliderIsDragging = false;

        public event Action CurrentPositionChanged;

        public AudioTimeService(IQueueService queueService, IAudioPlayback audioPlayback, IHistoryServices historyServices)
        {
            _queueService = queueService;
            _audioEventManager = new(audioPlayback, TimerInterval);
            _audioPlayback = audioPlayback;
            _historyServices = historyServices;

            _queueService.PlayingTrackChanged += OnPlayingTrackChanged;
            _queueService.QueueChanged += OnQueueChanged;

            _audioPlayback.AutoChangeOutput(ConfigurationService.GetPreference(Enums.SettingsEnum.AutoChangeOutputDevice) == 1);

            _audioEventManager.RegisterOnTickCallback(OnTickCallback);
            _audioEventManager.RegisterOnHalfWayThroughCallback(async () => await UpdatePlayCounts());
            _audioEventManager.RegisterOnStreamChangedCallback(OnStreamChangedCallback);
            _audioEventManager.RegisterOnStreamEndCallback(EndOfTrackCallback);
        }

        private void OnTickCallback()
        {
            if (sliderIsDragging) 
                return;

            CurrentPositionMs = (int)_audioEventManager.StreamPositionMs;
            CurrentQueuePositionMs = TimeUntilPlayingTrack + CurrentPositionMs;
            CurrentPositionChanged?.Invoke(); // trigger event for listener
        }

        private void OnStreamChangedCallback()
        {
            MaxPositionMs = (int)_audioEventManager.MaxStreamDurationMs;
            // update the listen time
            if(_currentTrack.IsNotNull())
                _historyServices.UpdateTodayHistory(_currentTrack, (int)_audioEventManager.StreamListenTimeMs);
            _currentTrack = _queueService.Queue.PlayingQueueTrack.Track;
        }

        private async Task UpdatePlayCounts()
        {
            await _queueService.IncreasePlayCount(1);

            List<int> artistIds = new(); // keep updated artist as to not update them more than once
            foreach (TrackArtistsRole artist in _queueService.Queue.PlayingTrack.TrackArtistRole)
            {
                if(!artistIds.Any(a => a == artist.ArtistRole.Artist.Id))
                {
                    await Artist.UpdatePlayCount(artist.ArtistRole.Artist);
                    artistIds.Add(artist.ArtistRole.Artist.Id);
                }
            }

            Album album = _queueService.Queue.PlayingTrack.Album;
            if (album != null)
            {
                await Album.UpdatePlayCount(album);
                if(!artistIds.Any(a => a == album.PrimaryArtist.Id))
                    await Artist.UpdatePlayCount(album.PrimaryArtist);
            }
        }

        private void EndOfTrackCallback()
        {
            if (!_audioPlayback.IsLooping)
                _queueService.NextTrack();
        }

        /// <summary>
        /// Set the play back position to the position specified
        /// and update the current track jump time for the listen time
        /// </summary>
        /// <param name="position"> The position to go to in milliseconds </param>
        public void SetPlayBackPosition(int position)
        {
            // set playback position
            _audioPlayback.Position = TimeSpan.FromMilliseconds(position);
        }

        private void SetTimeUntilPlayingTrack()
        {
            TimeUntilPlayingTrack = 0;
            int index = _queueService.GetPlayingTrackIndex();

            TimeUntilPlayingTrack = index.GetLengthUntilTrack(_queueService.Queue.Tracks.ToTrack());
        }

        private void OnQueueChanged()
        {
            MaxQueuePositionMs = _queueService.Queue.Length;
            CurrentQueuePositionMs = 0;
            SetTimeUntilPlayingTrack();
        }

        private void OnPlayingTrackChanged()
        {
            if (_queueService.Queue.PlayingTrack is not null)
            {
                SetTimeUntilPlayingTrack();

                // the previous track was looping, keep the new track looping
                if (_audioPlayback.IsLooping)
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
        }

        public void OnDragStart(double value)
        {
            sliderIsDragging = true;
        }

        public void OnDragEnd(double value)
        {
            SetPlayBackPosition((int)value);
            sliderIsDragging = false;
        }
    }
}
