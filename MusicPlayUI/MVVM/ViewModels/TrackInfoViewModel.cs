
using MusicFilesProcessor;
using MusicPlay.Language;
using System;
using System.Collections.Generic;
using System.Timers;
using System.Windows.Input;
using MusicPlayUI.Core.Commands;
using MusicPlayUI.MVVM.Models;
using MusicPlayUI.Core.Services;
using MusicPlayUI.Core.Services.Interfaces;
using MusicPlay.Database.Models;

using MusicPlay.Database.Helpers;
using System.Threading.Tasks;
using System.Linq;

namespace MusicPlayUI.MVVM.ViewModels
{
    public class TrackInfoViewModel : ViewModel
    {
        private readonly IQueueService _queueService;

        private MusicFilePropertiesProcessor _musicFileProperties { get; set; } = new("");

        public Track PlayingTrack
        {
            get => _queueService.Queue.PlayingTrack;
        }

        private Album _album;
        public Album Album
        {
            get => _album;
            set
            {
                _album = value;
                OnPropertyChanged(nameof(Album));
            }
        }

        private List<TrackInfoModel> _mainTracksInfo;
        public List<TrackInfoModel> MainTracksInfo
        {
            get { return _mainTracksInfo; }
            set
            {
                _mainTracksInfo = value;
                OnPropertyChanged(nameof(MainTracksInfo));
            }
        }

        private List<TrackInfoModel> _fileProperties;
        public List<TrackInfoModel> FileProperties
        {
            get { return _fileProperties; }
            set
            {
                _fileProperties = value;
                OnPropertyChanged(nameof(FileProperties));
            }
        }

        public string FileName
        {
            get => _musicFileProperties.FileName;
        }

        public string AudioBitrate
        {
            get => _musicFileProperties.AudioBitrate + " Kbit/s";
        }

        public string AudioChannels
        {
            get
            {
                if (_musicFileProperties.AudioChannels == 1)
                {
                    return _musicFileProperties.AudioChannels + " (Mono)";
                }
                else if (_musicFileProperties.AudioChannels == 2)
                {
                    return _musicFileProperties.AudioChannels + " (Stereo)";
                }
                else
                {
                    return _musicFileProperties.AudioChannels + " (Dolby Atmos)";
                }
            }
        }

        public string AudioSampleRate
        {
            get => _musicFileProperties.AudioSampleRate + " kHz";
        }

        public string BitsPerSample
        {
            get => _musicFileProperties.BitsPerSample + " Bits";
        }

        public string FileExt
        {
            get => _musicFileProperties.FileExt;
        }

        public string PlayCount
        {
            get => Resources.PlayCount + ": " + PlayingTrack.PlayCount.ToString();
        }

        public int Rating
        {
            get
            {

                if (PlayingTrack != null)
                    return (int)PlayingTrack.Rating;
                else
                    return 0;
            }
            set
            {
                SaveRating(value);
            }
        }

        private bool _isFavorite;
        public bool IsFavorite
        {
            get { return _isFavorite; }
            set
            {
                _isFavorite = value;
                OnPropertyChanged(nameof(IsFavorite));
            }
        }

        public string LastPlayed
        {
            get
            {
                if (PlayingTrack.LastPlayed == DateTime.MinValue)
                {
                    return Resources.Fisrt_Time_Playing;
                }
                else
                {
                    return Resources.Last_Played + ": " + PlayingTrack.LastPlayed.ToShortDateString();
                }
            }
        }

        public ICommand IsFavoriteCommand { get; }

        public TrackInfoViewModel(IQueueService queueService)
        {
            _queueService = queueService;

            _queueService.PlayingTrackChanged += OnPlayingTrackChanged;
            _queueService.PlayingTrackInteractionChanged += OnPlayingTrackInteractionChanged;

            LoadData();

            IsFavoriteCommand = new RelayCommand(async () =>
            {
                IsFavorite = !IsFavorite;
                await _queueService.UpdateFavorite();
            });
        }

        private void OnPlayingTrackInteractionChanged()
        {
            OnPropertyChanged(nameof(Rating));
            IsFavorite = PlayingTrack.IsFavorite;
        }

        private void SaveRating(int value)
        {
            _queueService.UpdateRating(value);
        }

        public override void Dispose()
        {
            _queueService.PlayingTrackChanged -= OnPlayingTrackChanged;
            _queueService.PlayingTrackInteractionChanged -= OnPlayingTrackInteractionChanged;
            GC.SuppressFinalize(this);
        }

        private async Task LoadData()
        {
            OnPropertyChanged(nameof(PlayingTrack));
            Album = await Album.Get(PlayingTrack.Album.Id);

            OnPropertyChanged(nameof(Rating));
            IsFavorite = PlayingTrack.IsFavorite;
            OnPropertyChanged(nameof(LastPlayed));
            OnPropertyChanged(nameof(PlayCount));

            _musicFileProperties = new(PlayingTrack.Path);

            CreateLists();
        }

        private void CreateLists()
        {
            List<TrackInfoModel> trackInfo = new()
            {
                new(Resources.Title, PlayingTrack.Title),
                new(Resources.Duration, PlayingTrack.Duration),
                new(Resources.Album, PlayingTrack.Album.Name),
                new(Resources.Artists_View, ArtistsToString(PlayingTrack.TrackArtistRole.ToList())),
                new(Resources.Year, Album.Release),
                new(Resources.CopyRight, Album.Copyright)
            };
            MainTracksInfo = new(trackInfo);

            trackInfo = new()
            {
                new(Resources.FileName, FileName),
                new(Resources.File_Path, PlayingTrack.Path),
                new(Resources.Codec, FileExt),
                new(Resources.Audio_Bitrate, AudioBitrate),
                new(Resources.Audio_Channels, AudioChannels),
                new(Resources.Audio_Sample_Rate, AudioSampleRate),
                new(Resources.Bits_per_Sample, BitsPerSample)
            };
            FileProperties = new(trackInfo);
        }

        public static string ArtistsToString(List<TrackArtistsRole> artists)
        {
            return "";
        }

        private async void OnPlayingTrackChanged()
        {
            await LoadData();

            OnPlayingTrackInteractionChanged();
        }
    }
}
