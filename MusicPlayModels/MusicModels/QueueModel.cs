using MusicPlayUI.Core.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MusicPlayModels.MusicModels
{
    public class QueueModel : PlayableModel
    {
        private bool _isShuffled = false;
        private bool _isOnRepeat = false;
        private string _playingFrom = "";
        private string _cover = "";
        private ModelTypeEnum _playingFromModelType = ModelTypeEnum.UNKNOWN;
        private int _playingFromId = -1;
        private List<OrderedTrackModel> _tracks = new();
        private TrackModel _playingTrack = new();

        public bool IsShuffled
        {
            get { return _isShuffled; }
            set
            {
                SetField(ref _isShuffled, value);
            }
        }
        public bool IsOnRepeat
        {
            get { return _isOnRepeat; }
            set
            {
                SetField(ref _isOnRepeat, value);
            }
        }

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
                SetField(ref _cover, value);
            }
        }

        public TrackModel PlayingTrack
        {
            get => _playingTrack;
            set
            {
                SetField(ref _playingTrack, value);
            }
        }

        public List<OrderedTrackModel> Tracks
        {
            get => _tracks;
            set
            {
                SetField(ref _tracks, value);
            }
        }

        public int PlayingTrackId { get; set; }

        public ModelTypeEnum ModelType
        {
            get => _playingFromModelType;
            set => SetField(ref _playingFromModelType, value);
        }

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
