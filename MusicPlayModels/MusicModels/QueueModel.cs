using MusicPlayUI.Core.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MusicPlayModels.MusicModels
{
    public class QueueModel : BaseModel
    {
        private List<OrderedTrackModel> _tracks = new();
        private TrackModel _playingTrack = new();
        private bool _isShuffled = false;
        private bool _isOnRepeat = false;
        private string _playingFrom = "";
        private string _cover = "";
        private int _playingFromId;

        public int Id { get; set; }
        public bool IsShuffled
        {
            get { return _isShuffled; }
            set
            {
                _isShuffled = value;
                OnPropertyChanged(nameof(IsShuffled));
            }
        }
        public bool IsOnRepeat
        {
            get { return _isOnRepeat; }
            set
            {
                _isOnRepeat = value;
                OnPropertyChanged(nameof(IsOnRepeat));
            }
        }
        public int Length { get; set; } = 0;
        public string Duration { get; set; } = "";
        public string PlayingFrom
        {
            get { return _playingFrom; }
            set
            {
                _playingFrom = value;
                OnPropertyChanged(nameof(PlayingFrom));
            }
        }

        public int PlayingFromId
        {
            get => _playingFromId;
            set
            {
                SetField(ref _playingFromId, value);
            }
        }

        public string Cover
        {
            get { return _cover; }
            set
            {
                _cover = value;
                OnPropertyChanged(nameof(Cover));
            }
        }

        public int PlayingTrackId { get; set; }

        public TrackModel PlayingTrack
        {
            get => _playingTrack;
            set
            {
                _playingTrack = value;
                OnPropertyChanged(nameof(PlayingTrack));
            }
        }

        public List<OrderedTrackModel> Tracks
        {
            get => _tracks;
            set
            {
                _tracks = value;
                OnPropertyChanged(nameof(Tracks));
            }
        }

        public ModelTypeEnum ModelType { get; set; }

        public QueueModel(int id, bool isShuffled, bool isOnRepeat, int length, string duration, int playingTrackId, string from, string cover)
        {
            Id = id;
            IsShuffled = isShuffled;
            IsOnRepeat = isOnRepeat;
            Length = length;
            Duration = duration;
            PlayingTrackId = playingTrackId;
            PlayingFrom = from;
            Cover = cover;
        }

        public QueueModel(bool isShuffled, bool isOnRepeat, int length, string duration, int playingTrackId, string from, string cover)
        {
            IsShuffled = isShuffled;
            IsOnRepeat = isOnRepeat;
            Length = length;
            Duration = duration;
            PlayingTrackId = playingTrackId;
            PlayingFrom = from;
            Cover = cover;
        }

        public QueueModel(bool isShuffled, bool isOnRepeat, int length, string duration, TrackModel playingTrack, List<TrackModel> tracks)
        {
            IsShuffled = isShuffled;
            IsOnRepeat = isOnRepeat;
            Length = length;
            Duration = duration;
            PlayingTrack = playingTrack;
            Tracks = tracks.ToOrderedTrackModel();
        }

        public QueueModel(bool isShuffled, bool isOnRepeat, int length, string duration, TrackModel playingTrack, List<OrderedTrackModel> tracks)
        {
            IsShuffled = isShuffled;
            IsOnRepeat = isOnRepeat;
            Length = length;
            Duration = duration;
            PlayingTrack = playingTrack;
            Tracks = tracks;
        }

        public QueueModel()
        {

        }
    }
}
