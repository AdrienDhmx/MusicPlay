using MusicPlayUI.Core.Enums;
using System;
using System.Collections;
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
        private string _playingFromName = "";
        private string _cover = "";
        private ModelTypeEnum _playingFromModelType = ModelTypeEnum.UNKNOWN;
        private int _playingFromId = -1;
        private PlayableModel _playingFrom;
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

        public PlayableModel PlayingFrom
        {
            get => _playingFrom;
            set => SetField(ref _playingFrom, value);
        }

        public string PlayingFromName
        {
            get { return _playingFromName; }
            set
            {
                SetField(ref _playingFromName, value);
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

        public ModelTypeEnum PlayingFromModelType
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
            PlayingFromName = from;
            Cover = cover;
        }

        public QueueModel(bool isShuffled, bool isOnRepeat, int length, string duration, int playingTrackId, string from, string cover)
        {
            IsShuffled = isShuffled;
            IsOnRepeat = isOnRepeat;
            Length = length;
            Duration = duration;
            PlayingTrackId = playingTrackId;
            PlayingFromName = from;
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

        public override Dictionary<string, object> CreateTable()
        {
            Dictionary<string, object> keyValues = new Dictionary<string, object>
            {
                { nameof(PlayingTrackId), PlayingTrackId },
                { nameof(IsShuffled), IsShuffled.ToInt() },
                { nameof(IsOnRepeat), IsOnRepeat.ToInt() },
                { nameof(Cover), Cover },
                { nameof(PlayingFromName), PlayingFromName },
                { nameof(PlayingFromModelType), (int)PlayingFromModelType },
                { nameof(PlayingFromId), PlayingFromId },
            };

            return keyValues;
        }
    }
}
