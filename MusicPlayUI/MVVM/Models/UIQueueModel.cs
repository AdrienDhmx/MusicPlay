using MusicPlayModels;
using MusicPlayModels.MusicModels;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;

namespace MusicPlayUI.MVVM.Models
{
    public class UIQueueModel : BaseModel
    {
        private ObservableCollection<UIOrderedTrackModel> _tracks = new();

        private UIOrderedTrackModel _playingTrack = new();

        private bool _isShuffled = false;
        private bool _isOnRepeat = false;
        private string _playingFrom = "";
        private string _cover = "";

        public int Id { get; set; }
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
        public int Length { get; set; } = 0;
        public string Duration { get; set; } = "";
        public string PlayingFrom
        {
            get { return _playingFrom; }
            set
            {
                SetField(ref _playingFrom, value);
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

        public int PlayingTrackId { get; set; }

        public UIOrderedTrackModel PlayingTrack
        {
            get => _playingTrack;
            set
            {
                SetField(ref _playingTrack, value);
            }
        }

        public ObservableCollection<UIOrderedTrackModel> Tracks
        {
            get => _tracks;
            set
            {
                SetField(ref _tracks, value);
            }
        }

        public UIQueueModel(int id, bool isShuffled, bool isOnRepeat, int length, string duration, int playingTrackId, string from, string cover)
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

        public UIQueueModel(bool isShuffled, bool isOnRepeat, int length, string duration, int playingTrackId, string from, string cover)
        {
            IsShuffled = isShuffled;
            IsOnRepeat = isOnRepeat;
            Length = length;
            Duration = duration;
            PlayingTrackId = playingTrackId;
            PlayingFrom = from;
            Cover = cover;
        }

        public UIQueueModel(bool isShuffled, bool isOnRepeat, int length, string duration, int playingTrackId, string from, string cover, TrackModel playingTrack, List<TrackModel> tracks, bool albumCover, bool autoCover = false)
        {
            IsShuffled = isShuffled;
            IsOnRepeat = isOnRepeat;
            Length = length;
            Duration = duration;
            PlayingFrom = from;
            Cover = cover;
            PlayingTrackId = playingTrackId;
            PlayingTrack = new(playingTrack, albumCover, autoCover);
            Tracks = new(tracks.ToUIOrderedTrackModel(albumCover, autoCover));
        }

        public UIQueueModel(QueueModel queueModel, bool albumCover, bool autoCover = false)
        {
            IsShuffled = queueModel.IsShuffled;
            IsOnRepeat = queueModel.IsOnRepeat;
            Length = queueModel.Length;
            Duration = queueModel.Duration;
            PlayingTrack = new(queueModel.PlayingTrack, albumCover, autoCover);
            Tracks = new(queueModel.Tracks.ToUIOrderedTrackModel(albumCover, autoCover));
        }

        public UIQueueModel()
        {

        }

    }

    public static class UIQueueModelExt
    {
        public static QueueModel ToQueueModel(this UIQueueModel uIQueueModel)
        {
            return new(uIQueueModel.IsShuffled, uIQueueModel.IsOnRepeat, uIQueueModel.Length, uIQueueModel.Duration, uIQueueModel.PlayingTrack, uIQueueModel.Tracks.ToList().ToTrackModel())
            {
                PlayingFrom = uIQueueModel.PlayingFrom,
                Id = uIQueueModel.Id,
                Cover = uIQueueModel.Cover,
                PlayingTrackId = uIQueueModel.PlayingTrackId,
            };
        }
    }

}
